using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialWeaponData", menuName = "Data/SpecialWeapon", order = 0)]
public class SpecialWeaponData : ScriptableObject
{
    // 分别写明了gameObject名字和位置
    public List<(string, string)> partsInfo;
}