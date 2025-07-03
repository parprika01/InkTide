using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkTest : MonoBehaviour
{
    [Header("事件")]
    [SerializeField] private IntEventChannel inkConsumptionEvent;

    private void OnEnable()
    {
        inkConsumptionEvent.OnEventRaised += HandleInk;
    }
    private void OnDisable()
    {
        inkConsumptionEvent.OnEventRaised -= HandleInk;
    }
    private void HandleInk(int ink)
    {
        Debug.Log("墨水消耗: " + ink);
    }
}
