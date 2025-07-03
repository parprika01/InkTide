using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubWeaponData", menuName = "Data/SubWeapon", order = 0)]
public class SubWeaponData : ScriptableObject
{
    [Header("Damage")]
    public int DamagePerCollision;
}