using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/Shooter", order = 0)]
public class Shooter : ScriptableObject
{
    [Header("Damage")]
    public int BaseDamage;
    public float DamagePerCollision;
}
