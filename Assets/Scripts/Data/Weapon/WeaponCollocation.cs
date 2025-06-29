using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponDict
{
    public string mainWeapon;
    public string subWeapon;
    public string specialWeapon;
}

[CreateAssetMenu(menuName = "WeaponCollocation")]
public class WeaponCollocation : ScriptableObject
{
    public List<WeaponDict> weapons;

    public Dictionary<string, (string, string)> GetWeaponRelations()
    {
        Dictionary<string, (string, string)> relations = new Dictionary<string, (string, string)>();
        foreach (WeaponDict w in weapons)
        {
            relations[w.mainWeapon] = (w.subWeapon, w.specialWeapon);
        }
        return relations;
    }
}
