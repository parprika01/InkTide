using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkTexProvider : MonoBehaviour
{
    public static InkTexProvider Instance;
    public List<Texture2D> inkTextures = new List<Texture2D>();
    //public List<Texture2D> inkTextures2 = new List<Texture2D>();
    public Texture2DArray paintTexArray; // 新增：统一尺寸后的Texture2DArray
    public string inkType;
    private Dictionary<int, List<int>> ShooterTex;

    public int targetWidth = 512;
    public int targetHeight = 512;

    private void Awake()
    {
        Instance = this;
        InitTexture2DArray();
        ShooterTexInit();
    }

    public Texture2DArray GetAllTex()
    {
        return paintTexArray;
    }

    public int GetInkTex(float angle)
    {
        return ShooterTexSelect(angle);
    }

    private int ShooterTexSelect(float angle)
    {
        int level = (int)angle / 18;
        List<int> texs = ShooterTex[level];
        int num = Random.Range(0, texs.Count);
        //Debug.Log("当前选中的Tex为:" + inkTextures[texs[num]].name + " 次序为：" + texs[num]);
        return texs[num];
    }

    private void ShooterTexInit()
    {
        ShooterTex = new Dictionary<int, List<int>>();
        for (int i = 0; i < inkTextures.Count; i++)
        {
            string name = inkTextures[i].name;
            string weapon = name.Substring(0, 4);
            //Debug.Log("weapon:" +weapon);
            if (weapon == "Shot")
            {
                int level = name[5] - '0';
                if (!ShooterTex.ContainsKey(level))
                    ShooterTex[level] = new List<int>();
                //Debug.Log("level: " + level +"index:" + i);
                ShooterTex[level].Add(i);
            }
        }
    }

    /// <summary>
    /// 将所有 inkTextures 转为统一尺寸并放入 paintTexArray
    /// </summary>
    private void InitTexture2DArray()
    {
        if (inkTextures == null || inkTextures.Count == 0)
        {
            Debug.LogWarning("InkTextures 为空，无法生成 Texture2DArray");
            return;
        }

        paintTexArray = new Texture2DArray(targetWidth, targetHeight, inkTextures.Count, TextureFormat.RGBA32, false);

        for (int i = 0; i < inkTextures.Count; i++)
        {
            Texture2D paddedTex = ResizeTextureWithPadding(inkTextures[i], targetWidth, targetHeight);
            //inkTextures2.Add(paddedTex);
            Graphics.CopyTexture(paddedTex, 0, 0, paintTexArray, i, 0);
        }

        paintTexArray.Apply();
        Debug.Log("Texture2DArray 构建完成！");
    }

    /// <summary>
    /// 缩放贴图到统一大小
    /// </summary>
    private Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);

        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.ReleaseTemporary(rt);
        RenderTexture.active = null;

        return result;
    }

    private Texture2D ResizeTextureWithPadding(Texture2D source, int targetWidth, int targetHeight)
    {
        // 先计算目标画布的比例
        float targetRatio = (float)targetWidth / targetHeight;
        float sourceRatio = (float)source.width / source.height;

        int paddedWidth, paddedHeight;

        // 计算缩放后原图的尺寸，保持比例
        if (sourceRatio > targetRatio)
        {
            // 原图更宽，宽度占满，计算高度
            paddedWidth = targetWidth;
            paddedHeight = Mathf.RoundToInt(targetWidth / sourceRatio);
        }
        else
        {
            // 原图更高，高度占满，计算宽度
            paddedHeight = targetHeight;
            paddedWidth = Mathf.RoundToInt(targetHeight * sourceRatio);
        }

        // 创建空白画布（RGBA32，透明背景）
        Texture2D paddedTex = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
        Color clear = new Color(0, 0, 0, 0);
        Color[] fillColorArray = paddedTex.GetPixels();
        for (int i = 0; i < fillColorArray.Length; i++)
            fillColorArray[i] = clear;
        paddedTex.SetPixels(fillColorArray);

        // 缩放原图到 paddedWidth x paddedHeight
        Texture2D resizedSource = ResizeTexture(source, paddedWidth, paddedHeight);

        // 计算左下角起点坐标，使其居中
        int xOffset = (targetWidth - paddedWidth) / 2;
        int yOffset = (targetHeight - paddedHeight) / 2;

        // 把 resizedSource 像素复制到 paddedTex 中心
        Color[] sourcePixels = resizedSource.GetPixels();
        for (int y = 0; y < paddedHeight; y++)
        {
            for (int x = 0; x < paddedWidth; x++)
            {
                Color pixel = sourcePixels[y * paddedWidth + x];
                paddedTex.SetPixel(x + xOffset, y + yOffset, pixel);
            }
        }

        paddedTex.Apply();
        return paddedTex;
    }
        
}
