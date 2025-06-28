using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public struct PaintCommand
{
    public Vector4 pos;
    public Vector4 color;
    public float scale;
    public int texIndex;
    public Vector3 normal;
    public float impactDirection;
}

public class PaintDataPool{
    static PaintDataPool instance;
    static public PaintDataPool Instance
    {
        get
        {
            if (instance == null)
                instance = new PaintDataPool();
            return instance;
        }
    }
    public List<Renderer> renderers;

    public void AddRenderer(Renderer renderer){
        renderers.Add(renderer);
    }

    public List<PaintCommand> commands;

    public void AddCommands(float angle, Vector4 pos, Vector4 color, float scale, Vector3 normal, float impactDirection){
        PaintCommand command = new PaintCommand();
        command.pos = pos;
        command.color = color;
        command.scale = scale;
        command.texIndex = InkTexProvider.Instance.GetInkTex(angle);
        command.normal = normal;
        command.impactDirection = impactDirection;
        commands.Add(command);
    }

    public PaintDataPool(){
        renderers = new List<Renderer>();
        commands = new List<PaintCommand>();
    }
}

public class InkManager : MonoBehaviour
{
    public static InkManager Instance;
    [Header("Reference")]
    public Shader unwrapWorldShader;
    public Shader unwrapWorldTopShader;
    public ComputeShader inkPaintCS;
    public ComputeShader topCullingCS;
    public ComputeShader CheckInkCoverageCS;

    [Header("Data")]
    public int currentInkPoint = 0;
    public Vector2Int RTsize = new Vector2Int(512, 512);
    public const int numThreads = 8;
    public Vector3Int numThreadGroups;
    public RenderTexture inkRT;
    public RenderTexture effectInkRT;
    public RenderTexture worldPosRT;
    public RenderTexture worldPosTopRT;
    public float strength = 0.7f;
    private ComputeBuffer counterBuffer;
    private int[] counterData;

    private Camera RTCamera;
    private ComputeBuffer computeBuffer;

    void Awake()
    {
        Instance = this;
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCamera();
        UnwrapWorldPos();
        UnwrapWorldPosTop();
    }

    // Update is called once per frame
    void Update()
    {
        Paint();
    }

    void SetCamera(){
        RTCamera = new GameObject("rtCameraObject").AddComponent<Camera>();
        RTCamera.transform.position = Vector3.zero;
        RTCamera.transform.rotation = Quaternion.identity;
        RTCamera.transform.localScale = Vector3.one;
        RTCamera.renderingPath = RenderingPath.Forward;
        RTCamera.clearFlags = CameraClearFlags.SolidColor;
        RTCamera.backgroundColor = new Color(0, 0, 0, 0);
        RTCamera.orthographic = true;
        RTCamera.nearClipPlane = 0.0f;
        RTCamera.farClipPlane = 1.0f;
        RTCamera.orthographicSize = 1.0f;
        RTCamera.aspect = 1.0f;
        RTCamera.useOcclusionCulling = false;
        RTCamera.enabled = false;        
    }

    void UnwrapWorldPos(){
        //Material unwrapWorldMat = new Material(unwrapWorldShader);
        List<Renderer> renderers = PaintDataPool.Instance.renderers;
        CommandBuffer cb = new CommandBuffer();
        cb.SetRenderTarget(worldPosRT);

        Debug.Log("renderers.Count: " + renderers.Count);
        for(int i=0; i<renderers.Count; i++){
            Vector4 lightmapST = renderers[i].lightmapScaleOffset;
            Material unwrapWorldMat = new Material(unwrapWorldShader);
            unwrapWorldMat.SetVector("_LightmapST", lightmapST);
            cb.DrawRenderer(renderers[i], unwrapWorldMat, 0, 0);
        }

        RTCamera.AddCommandBuffer(CameraEvent.AfterEverything, cb);
        RTCamera.Render();
        cb.Release();        
    }

    void UnwrapWorldPosTop()
    {
        //Material unwrapWorldMat = new Material(unwrapWorldShader);
        List<Renderer> renderers = PaintDataPool.Instance.renderers;
        CommandBuffer cb = new CommandBuffer();
        cb.SetRenderTarget(worldPosTopRT);

        Debug.Log("renderers.Count: " + renderers.Count);
        for (int i = 0; i < renderers.Count; i++)
        {
            Vector4 lightmapST = renderers[i].lightmapScaleOffset;
            Material unwrapWorldMat = new Material(unwrapWorldTopShader);
            unwrapWorldMat.SetVector("_LightmapST", lightmapST);
            cb.DrawRenderer(renderers[i], unwrapWorldMat, 0, 0);
        }

        RTCamera.AddCommandBuffer(CameraEvent.AfterEverything, cb);
        RTCamera.Render();
        cb.Release();
        TopOcclusionCuling(worldPosTopRT);        
    }

    // 提出顶面中被遮挡的部分
    void TopOcclusionCuling(RenderTexture src)
    {
        RenderTexture temp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        temp.enableRandomWrite = true;
        Graphics.Blit(src, temp);

        topCullingCS.SetTexture(0, "_Result", src);
        topCullingCS.SetTexture(0, "_Input", temp);
        topCullingCS.Dispatch(0, numThreadGroups.x, numThreadGroups.y, numThreadGroups.z);

        RenderTexture.ReleaseTemporary(temp);
    }    

    void Paint(){
        List<PaintCommand> commands = PaintDataPool.Instance.commands;
        if(commands.Count > 0){
            computeBuffer = new ComputeBuffer(commands.Count, 56);
            computeBuffer.SetData(commands);
            inkPaintCS.SetTexture(0, "_PaintTexArray", InkTexProvider.Instance.GetAllTex());
            inkPaintCS.SetBuffer(0, "_PaintCommands", computeBuffer);
            inkPaintCS.SetBuffer(0, "_InkPointCounter", counterBuffer);
            inkPaintCS.SetInt("_CommandCount", commands.Count);
            inkPaintCS.SetTexture(0, "_WorldPosTex", worldPosRT);
            inkPaintCS.SetTexture(0,"_WorldPosTopTex", worldPosTopRT);
            inkPaintCS.SetTexture(0, "_InkTex", inkRT);
            inkPaintCS.SetTexture(0, "_EffectInkTex", effectInkRT);
            inkPaintCS.SetFloat("_Strength", strength);
            inkPaintCS.Dispatch(0, numThreadGroups.x, numThreadGroups.y, numThreadGroups.z);

            counterBuffer.GetData(counterData);
            currentInkPoint = counterData[0];

            computeBuffer.Release();
            commands.Clear();
        }
    }

    void ClearRT(params RenderTexture[] rts){
        CommandBuffer cb = new CommandBuffer();
        for(int i=0; i<rts.Length; i++){
            cb.SetRenderTarget(rts[i]);
            cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
        }
        Graphics.ExecuteCommandBuffer(cb);
        cb.Release();
    }

    void Initialize(){
        worldPosRT = new RenderTexture(RTsize.x, RTsize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        worldPosRT.enableRandomWrite = true;
        worldPosRT.Create();

        worldPosTopRT = new RenderTexture(RTsize.x, RTsize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        worldPosTopRT.enableRandomWrite = true;
        worldPosTopRT.Create();

        inkRT = new RenderTexture(RTsize.x, RTsize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        inkRT.enableRandomWrite = true;
        inkRT.Create();

        effectInkRT = new RenderTexture(RTsize.x, RTsize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        effectInkRT.enableRandomWrite = true;
        effectInkRT.Create();

        counterBuffer = new ComputeBuffer(1, sizeof(int));
        counterData = new int[]{0};
        counterBuffer.SetData(counterData);

        ClearRT(worldPosRT, inkRT, worldPosTopRT, effectInkRT);

        numThreadGroups = new Vector3Int(RTsize.x / numThreads, RTsize.y / numThreads, 1);
    }

    public bool IsInInkArea(Vector3 pos)
    {
        // 1. 使用 int 代替 bool（sizeof(int)=4，满足对齐要求）
        var resultBuffer = new ComputeBuffer(1, sizeof(int));
    
        // 2. 初始化数据（0表示false，1表示true）
        int[] initData = new int[] { 0 }; // 0 = false
        resultBuffer.SetData(initData);
    
        int kernelHandle = CheckInkCoverageCS.FindKernel("CSMain");
    
        // 3. 设置参数
        CheckInkCoverageCS.SetTexture(kernelHandle, "_InkTex", inkRT);
        CheckInkCoverageCS.SetTexture(kernelHandle, "_WorldPosTex", worldPosRT);
        CheckInkCoverageCS.SetVector("_WorldPos", pos);
        CheckInkCoverageCS.SetBuffer(kernelHandle, "_IsCovered", resultBuffer);
    
        // 4. 分派计算
        CheckInkCoverageCS.Dispatch(kernelHandle, 
            Mathf.CeilToInt(inkRT.width / 8f), 
            Mathf.CeilToInt(inkRT.height / 8f), 
            1);
    
        // 5. 读取结果（int类型）
        int[] result = new int[1];
        resultBuffer.GetData(result);
        resultBuffer.Release();
    
        return result[0] == 1; // 转换为bool
    }
}
