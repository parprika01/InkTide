using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 此类用于提供安装装备的两种方法


public class EquipSystem : MonoBehaviour
{
    #region event
    [Header("event")]
    [SerializeField] private EventChannel InitializeEvent;
    [SerializeField] private SettingChangeEventChannel settingChangeEvent;
    [SerializeField] private EventChannel EquipLoadDoneEvent;
    #endregion
    public DataDictionary equipDict;
    public GameObject root;
    public Material material;
    private SkinnedMeshRenderer[] renderers;
    #region data
    private Dictionary<string, Transform> bones;
    private PlayerInfo playerInfo;
    private List<GameObject> equipList;
    private List<GameObject> boneEquipsList;
    #endregion
    #region equip models
    [Header("equip pos")]
    [SerializeField] private Transform headPos;
    [SerializeField] private Transform leftHandPos;
    [SerializeField] private Transform rightHandPos;
    [SerializeField] private Transform boneEquipPos;
    [Header("equip models")]
    [SerializeField] private GameObject hair;
    [SerializeField] private GameObject eyebrows;
    [SerializeField] private GameObject bottom;
    [SerializeField] private GameObject cloth;
    [SerializeField] private GameObject leftShoe;
    [SerializeField] private GameObject rightShoe;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject weapon;
    #endregion    
    #region equip dicts
    private Dictionary<string, GameObject> hairDict;
    private Dictionary<string, GameObject> eyebrowsDict;
    private Dictionary<string, GameObject> bottomDict;
    private Dictionary<string, GameObject> clothDict;
    private Dictionary<string, GameObject> headDict;
    private Dictionary<string, (GameObject, GameObject)> shoeDict;
    #endregion
    #region equip names
    private string hairName;
    private string eyebrowsName;
    private string bottomName;
    private string clothName;
    private string headName;
    private string shoesName;
    #endregion
    private void Awake()
    {
        InitializeEvent.OnEventRaised += SettingInitialize;
        settingChangeEvent.OnEventRaised += HandleSettingChangeEvent;
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }
    private void Start()
    {
    }
    void ReadBones()
    {
        if (root == null)
        {
            Debug.Log("root bone is null");
            return;
        }
        bones = new Dictionary<string, Transform>();
        foreach (Transform bone in root.GetComponentsInChildren<Transform>())
        {
            bones[bone.name] = bone;
        }
    }
    void EquipListInit()
    {
        equipList = new List<GameObject> { hair, eyebrows, bottom, cloth, leftShoe, rightShoe, head };
        boneEquipsList = new List<GameObject> { eyebrows, bottom, cloth, leftShoe, rightShoe };
    }
    void EquipDictInit()
    {
        hairDict = equipDict.GetDict(equipDict.hairs);
        eyebrowsDict = equipDict.GetDict(equipDict.eyebrows);
        bottomDict = equipDict.GetDict(equipDict.bottoms);
        clothDict = equipDict.GetDict(equipDict.clothes);
        headDict = equipDict.GetDict(equipDict.heads);
        shoeDict = equipDict.GetShoeDict(equipDict.shoes);
    }
    void EquipNameInit()
    {
        hairName = playerInfo.hair;
        eyebrowsName = playerInfo.eyebrows;
        bottomName = playerInfo.bottom;
        clothName = playerInfo.cloth;
        headName = playerInfo.head;
        shoesName = playerInfo.shoes;
    }
    void LoadAllEquip()
    {
        // 先实例化所有的预制体
        // 基础部分
        hair = Instantiate(hairDict[hairName]);
        eyebrows = Instantiate(eyebrowsDict[eyebrowsName]);
        bottom = Instantiate(bottomDict[bottomName]);
        cloth = Instantiate(clothDict[clothName]);
        head = Instantiate(headDict[headName]);
        (leftShoe, rightShoe) = shoeDict[shoesName];
        leftShoe = Instantiate(leftShoe);
        rightShoe = Instantiate(rightShoe);
        EquipListInit();
        // 武器等：主武器、副武器、墨囊、特殊武器
        // 将模型放置在对应的位置上也就是以玩家对象为亲物体，并且将position归0
        hair.transform.SetParent(headPos);
        head.transform.SetParent(headPos);
        foreach (GameObject equip in boneEquipsList)
        {
            equip.transform.SetParent(boneEquipPos);
        }

        foreach (GameObject equip in equipList)
        {
            equip.transform.localPosition = Vector3.zero;
            equip.transform.localRotation = Quaternion.identity;
        }

        // 对需要绑定骨骼的模型进行绑骨
        foreach (GameObject equip in boneEquipsList)
        {
            ReplaceBones(equip);
        }
        EquipLoadDoneEvent.Raise();
    }
    void ReplaceBones(GameObject equip)
    {
        SkinnedMeshRenderer smr = equip.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.Log("equip has no skinned mesh renderer");
            return;
        }

        Transform[] newBones = new Transform[smr.bones.Length];
        for (int i = 0; i < smr.bones.Length; i++)
        {
            string boneName = smr.bones[i].name;

            if (bones.TryGetValue(boneName, out Transform newBone))
            {
                newBones[i] = newBone;
            }
            else
            {
                Debug.Log("bone " + boneName + " not found");
                newBones[i] = smr.bones[i];
            }
        }

        smr.bones = newBones;
        smr.rootBone = root.transform;
        Debug.Log("Wear");
    }
    void SettingInitialize()
    {
        playerInfo = PlayerInfo.Instance;
        ReadBones();
        EquipDictInit();
        EquipNameInit();
        LoadAllEquip();
    }
    void HandleSettingChangeEvent((SettingType, string) settingData)
    {
        switch (settingData.Item1)
        {
            case SettingType.MainWeapon:
                break;
            case SettingType.HeadEquip:
                ChangeEquip(ref head, settingData.Item2, headPos, headDict, false);
                break;
            case SettingType.Cloth:
                ChangeEquip(ref cloth, settingData.Item2, boneEquipPos, clothDict);
                break;
            case SettingType.Shoes:
                break;
            case SettingType.Hair:
                ChangeEquip(ref hair, settingData.Item2, headPos, hairDict, false);
                break;
            case SettingType.Eyebrows:
                ChangeEquip(ref eyebrows, settingData.Item2, boneEquipPos, eyebrowsDict);
                break;
            case SettingType.Bottom:
                ChangeEquip(ref bottom, settingData.Item2, boneEquipPos, bottomDict);
                break;
        }
    }
    void ChangeEquip(ref GameObject equip, string equipName, Transform equipPos, Dictionary<string, GameObject> equipDict, bool isBone = true)
    {
        if (equipDict.TryGetValue(equipName, out GameObject newEquipPrefab))
        {
            if (equip != null)
            {
                Destroy(equip);
            }

            equip = Instantiate(newEquipPrefab);
            equip.transform.SetParent(equipPos);
            equip.transform.localPosition = Vector3.zero;

            if (isBone)
                ReplaceBones(equip);
            EquipListInit();
        }
    }
    void Combine()
    {
        foreach (GameObject equip in boneEquipsList)
        {
            renderers.AddRange(equip.GetComponentsInChildren<SkinnedMeshRenderer>());
        }

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        Transform[] transforms = root.GetComponentsInChildren<Transform>();
        List<Transform> boneList = new List<Transform>();
        List<Texture2D> textures = new List<Texture2D>();

        int width = 0;
        int height = 0;
        int uvCount = 0;

        List<Vector2[]> uvList = new List<Vector2[]>();

        foreach (SkinnedMeshRenderer smr in renderers)
        {
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }

            uvList.Add(smr.sharedMesh.uv);
            uvCount += smr.sharedMesh.uv.Length;

            if (smr.material.mainTexture != null)
            {
                textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);
                width += smr.GetComponent<Renderer>().material.mainTexture.width;
                height += smr.GetComponent<Renderer>().material.mainTexture.height;
            }

            foreach (Transform bone in smr.bones)
            {
                foreach (Transform item in transforms)
                {
                    if (item.name != bone.name) continue;
                    boneList.Add(item);
                    break;
                }
            }          
        }
        // 获取并配置角色所有的SkinnedMeshRenderer
        SkinnedMeshRenderer tempRenderer = root.gameObject.GetComponent<SkinnedMeshRenderer>();
        if (!tempRenderer)
        {
            tempRenderer = root.gameObject.AddComponent<SkinnedMeshRenderer>();
        }

        tempRenderer.sharedMesh = new Mesh();

        // 合并网格，刷新骨骼，附加材质
        tempRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
        tempRenderer.bones = boneList.ToArray();
        tempRenderer.material = material;

        Texture2D skinnedMeshAtlas = new Texture2D(get2Pow(width), get2Pow(height));
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        Vector2[] atlasUVs = new Vector2[uvCount];

        // 因为将贴图都整合到了一张图片上，所以需要重新计算UV
        int j = 0;
        for (int i = 0; i < uvList.Count; i++)
        {
            foreach (Vector2 uv in uvList[i])
            {
                atlasUVs[j].x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, uv.x);
                atlasUVs[j].y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, uv.y);
                j++;
            }
        }

        // 设置贴图和UV
        tempRenderer.material.mainTexture = skinnedMeshAtlas;
        tempRenderer.sharedMesh.uv = atlasUVs;

        // 销毁所有部件
        foreach (GameObject goTemp in equipList)
        {
            if (goTemp)
            {
                Destroy(goTemp);
            }
        } 


    }
    private int get2Pow(int into)
    {
        int outo = 1;
        for (int i = 0; i < 10; i++)
        {
            outo *= 2;
            if (outo > into)
            {
                break;
            }
        }

        return outo;
    }    
}
