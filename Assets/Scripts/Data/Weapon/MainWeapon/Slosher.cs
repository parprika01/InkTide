using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/Slosher", order = 0)]
public class Slosher : ScriptableObject
{
    [Header("Damage")]
    public int BaseDamage;
    public float DamagePerCollision;
}
