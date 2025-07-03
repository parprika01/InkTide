using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculation : MonoBehaviour 
{
    [Header("事件")]
    [SerializeField] private BoolEventChannel fireEvent; 
    [SerializeField] private BoolEventChannel groundEvent;
    [SerializeField] private EventChannel fireTriggerEvent;
    [SerializeField] private Shooter shooter;
    [SerializeField] private Slosher slosher;
    [SerializeField] private Roller roller;
    [SerializeField] private Charger charger;
    [SerializeField] private Splatling splatling;
    [SerializeField] private SubWeaponData thrower;
    [SerializeField] private SubWeaponData burst;
    private bool isFire_bool;
    private bool isFire_trigger;
    private bool isGround;
    private int damage;
    private string particleName;
    private float chargeTime;

    private void Awake()
    {
        isFire_bool = false;
        isFire_trigger = false;
        damage = 0;
        isGround = true;
        particleName = "";
        chargeTime = 0;
    }
    public void OnEnable()
    {
        fireEvent.OnEventRaised += HandleFire;
        groundEvent.OnEventRaised += HandleGround;
        fireTriggerEvent.OnEventRaised += HandleFireTrigger;
    }
    public void OnDisable()
    {
        fireEvent.OnEventRaised -= HandleFire;
        groundEvent.OnEventRaised -= HandleGround;
        fireTriggerEvent.OnEventRaised -= HandleFireTrigger;
    }

    private void FixedUpdate()
    {
        if (particleName.Contains("chrg") || particleName.Contains("splt"))
        {
            chargeTime += Time.deltaTime;
        }
    }

    #region EventHandles
    private void HandleFire(bool fireInput)
    {
        isFire_bool = fireInput;
    }
    private void HandleGround(bool groundInput)
    {
        isGround = groundInput;
    }
    private void HandleFireTrigger()
    {
        isFire_trigger = true;
    }
    #endregion

    private int Damage(int collisionNum, string parName)
    {
        particleName = parName;
        if (parName.Contains("shtr"))
        {
            damage = Mathf.FloorToInt(collisionNum * shooter.DamagePerCollision);
        }
        else if (parName.Contains("slsh"))
        {
            damage = Mathf.FloorToInt(collisionNum * slosher.DamagePerCollision);
        }
        else if (parName.Contains("rllr"))
        {
            if (isFire_bool)
            {
                damage = roller.RollDamage;
            }
            else if (isFire_trigger && isGround)
            {
                damage = Mathf.FloorToInt(collisionNum * roller.GroundDamagePerCollision);
                isFire_trigger = false;
            }
            else
            {
                damage = Mathf.FloorToInt(collisionNum * roller.MidairDamagePerCollision);
                isFire_trigger = false;
            }
        }
        else if (parName.Contains("chrg"))
        {
            if (chargeTime >= 1f)
            {
                damage = charger.ChargeDamage;
            }
            else
            {
                float low = charger.NotChargeDamage.x;
                float high = charger.NotChargeDamage.y;
                damage = Mathf.FloorToInt(low + (high - low) * chargeTime);
            }
        }
        else if (parName.Contains("splt"))
        {
            if (chargeTime >= 0.45f)
            {
                damage = collisionNum * splatling.ChargeDamagePerCollision;
            }
            else
            {
                damage = collisionNum * splatling.NotChargeDamagePerCollision;
            }
        }
        else if (parName.Contains("throw") || parName.Contains("suction") || parName.Contains("curling"))
        {
            damage = collisionNum * thrower.DamagePerCollision;
        }
        else
        {
            damage = collisionNum * burst.DamagePerCollision;
        }
        return damage;
    }
}
