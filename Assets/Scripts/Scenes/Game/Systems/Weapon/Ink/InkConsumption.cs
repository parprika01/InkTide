using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkConsumption : MonoBehaviour
{
    [Header("事件")]
    [SerializeField] private IntEventChannel inkConsumptionEvent;
    [SerializeField] private BoolEventChannel syncFireEvent;
    [SerializeField] private EventChannel syncFireTriggerEvent;
    [SerializeField] private BoolEventChannel syncHoldEvent;
    private string tag;
    private bool isFireBool;
    private float lastTime;
    private float fireTime;

    private void Awake()
    {
        fireTime = 0;
        lastTime = 0;
        tag = transform.GetChild(0).tag;
        Debug.Log("ink tag: " + tag);
        isFireBool = false;
    }
    private void Update()
    {
        if (isFireBool)
        {
            fireTime += Time.deltaTime;
        }
        Shooter();
    }
    private void OnEnable()
    {
        syncFireEvent.OnEventRaised += HandleFire;
        syncFireTriggerEvent.OnEventRaised += HandleFireTrigger;
        syncHoldEvent.OnEventRaised += HandleHold;

    }
    private void OnDisable()
    {
        syncFireEvent.OnEventRaised -= HandleFire;
        syncFireTriggerEvent.OnEventRaised -= HandleFireTrigger;
        syncHoldEvent.OnEventRaised -= HandleHold;
    }

    #region EventHandles
    private void HandleFire(bool fireInput)
    {
        isFireBool = fireInput;
        if (fireInput)
        {
            lastTime = Time.time; // 记录 true 的时间点
        }
        else if (fireTime > 0)
        {
            fireTime = Time.time - lastTime;
            Debug.Log($"按键按住时长: {fireTime}秒");
            lastTime = -1;
            if (tag == "rllr")
            {
                inkConsumptionEvent.Raise(Mathf.FloorToInt(fireTime * 6));
            }
            else if (tag == "chrg")
            {
                inkConsumptionEvent.Raise(Mathf.FloorToInt(Mathf.Clamp(fireTime * 15.6f + 2.4f, 2.4f, 18f)));
            }
            else if (tag == "splt")
            {
                inkConsumptionEvent.Raise(15);
            }
        }
    }
    private void HandleFireTrigger()
    {
        if (tag == "rllr")
        {
            inkConsumptionEvent.Raise(9);
        }
        else if (tag == "slsh")
        {
            inkConsumptionEvent.Raise(7);
        }
    }
    private void HandleHold(bool holdInput)
    {
        if (holdInput)
        {
            if (tag == "slsh" || tag == "chrg")
            {
                inkConsumptionEvent.Raise(70);
            }
            else if (tag == "shtr")
            {
                inkConsumptionEvent.Raise(70);
            }
            else if (tag == "splt")
            {
                inkConsumptionEvent.Raise(45);
            }
            else
            {
                inkConsumptionEvent.Raise(65);
            }
        }
    }
    #endregion

    private void Shooter()
    {
        if (tag == "shtr" && isFireBool)
        {
            inkConsumptionEvent.Raise(1);
        }
    }
}
