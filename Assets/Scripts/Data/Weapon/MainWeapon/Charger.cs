using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/Charger", order = 0)]
public class Charger : ScriptableObject
{
    [Header("Damage")]
    public int ChargeDamage;
    public Vector2 NotChargeDamage;
}
