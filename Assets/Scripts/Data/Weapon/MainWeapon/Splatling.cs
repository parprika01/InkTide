using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/Splatling", order = 0)]
public class Splatling : ScriptableObject
{
    [Header("Damage")]
    public int ChargeDamagePerCollision;
    public int NotChargeDamagePerCollision;
    public int ChargeDamage;
    public int NotChargeDamage;
}
