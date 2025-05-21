using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NormalDict
{
    public string key;
    public GameObject value;
}

[System.Serializable]
public class ShoesDict
{
    public string key;
    public GameObject leftShoe;
    public GameObject rightShoe;
}

[CreateAssetMenu(menuName = "Dictionary")]
public class DataDictionary : ScriptableObject
{
    [Header("other")]
    public List<NormalDict> hairs;
    public List<NormalDict> eyebrows;
    public List<NormalDict> bottoms;
    [Header("cloth")]
    public List<NormalDict> clothes;
    public List<NormalDict> heads;
    public List<ShoesDict> shoes;

    public Dictionary<string, GameObject> GetDict(List<NormalDict> dict)
    {
        Dictionary<string, GameObject> res = new Dictionary<string, GameObject>();
        foreach (NormalDict m in dict)
        {
            res[m.key] = m.value;
        }
        return res;
    }

    public Dictionary<string, (GameObject, GameObject)> GetShoeDict(List<ShoesDict> dict)
    {
        Dictionary<string, (GameObject, GameObject)> res = new Dictionary<string, (GameObject, GameObject)>();
        foreach (ShoesDict m in dict)
        {
            res[m.key] = (m.leftShoe, m.rightShoe);
        }
        return res;
    }
}
