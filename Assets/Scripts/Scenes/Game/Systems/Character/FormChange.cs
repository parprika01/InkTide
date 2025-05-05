using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 此脚本用于人形和鱿鱼形态之间的切换
public class FormChange : MonoBehaviour
{
    [Header("参数")]
    public float stayTime;
    [Header("事件")]
    [SerializeField] private EventChannel<bool> squidEvent;
    [SerializeField] private EventChannel<bool> specialEvent;
    [SerializeField] private EventChannel<bool> fireEvent;
    [SerializeField] private EventChannel fireTriggerEvent;
    [SerializeField] private EventChannel<bool> holdEvent;
    [SerializeField] private EventChannel throwEvent;
    #region data
    public GameObject squid;
    public GameObject Shuman;
    private SkinnedMeshRenderer[] humanRenderers;
    private SkinnedMeshRenderer[] squidRenderers;
    private SkinnedMeshRenderer[] ShumanRenderers;
    #endregion
    //private Stack<> actionStack;
    enum Task
    {
        Fire_Bool,
        Fire_Trigger,
        Hold,
        Throw,
        Special
    }

    void Awake()
    {
        squidRenderers =  squid.GetComponentsInChildren<SkinnedMeshRenderer>();
        humanRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        ShumanRenderers = Shuman.GetComponentsInChildren<SkinnedMeshRenderer>();        
    }

    private void HandleSquid(bool squid)
    {
        
    }

    private void HandleSpecialCondition()
    {
        
    }

    // 用于禁用指定gameObject下所有的renderer
    private void DisableAllRenderer(SkinnedMeshRenderer[] renderers)
    {
        foreach (SkinnedMeshRenderer renderer in renderers)
            renderer.enabled = false;
    }

    private void EnableAllRenderer(SkinnedMeshRenderer[] renderers)
    {
        foreach (SkinnedMeshRenderer renderer in renderers)
            renderer.enabled = true;
    }

    private void HumanToSquid()
    {

    }

    private void SquidToHuman()
    {

    }

    private void SquidToSHToHuman()
    {

    }
}
