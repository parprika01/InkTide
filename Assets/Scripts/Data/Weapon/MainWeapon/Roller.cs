using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/Roller", order = 0)]
public class Roller : ScriptableObject
{
    [Header("Damage")]
    public int GroundDamagePerCollision;
    public int MidairDamagePerCollision;
    public int RollDamage;
}
