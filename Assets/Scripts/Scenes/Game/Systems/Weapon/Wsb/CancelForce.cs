using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelForce : MonoBehaviour
{
    [Header("事件")]
    [SerializeField] private BoolEventChannel syncHoldEvent;
    private bool isThrow;
    private bool hold;
    // Start is called before the first frame update
    void Start()
    {
        isThrow = true;
    }
    private void FixedUpdate()
    {
        if (hold == false && isThrow == false)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero; // 立即设置移动速度为0
            Debug.Log("设置玩家立即速度为0");
            isThrow = true;
        }
    }

    public void OnEnable()
    {
        syncHoldEvent.OnEventRaised += HandleThrow;
    }
    public void OnDisable()
    {
        syncHoldEvent.OnEventRaised -= HandleThrow;
    }

    #region EventHandles
    private void HandleThrow(bool throwInput)
    {
        hold = throwInput;
        if (throwInput)
        {
            isThrow = false;
        }
    }
    #endregion
}
