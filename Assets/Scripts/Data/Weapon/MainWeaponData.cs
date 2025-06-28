using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basic weapon data
public abstract class WeaponData : ScriptableObject
{
    public SubWeaponData subWeapon;
    public SpecialWeaponData specialWeapon;
    public string Name;
    public int SpecialPoint;
    public int Range;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/Shooter", order = 0)]
public class Shooter : WeaponData
{
    public int Damage;
    public int FireRate;
    [Header("Damage")]
    public int BaseDamage;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon/Roller", order = 0)]
public class Roller : WeaponData
{
    public int InkSpeed;
    public int Handling;
    [Header("Damage")]
    public Vector2 GroundDamage;
    public Vector2 MidairDamage;
    public Vector2 RollDamage;
}