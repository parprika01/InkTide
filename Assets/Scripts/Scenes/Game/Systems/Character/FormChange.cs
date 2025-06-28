using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

// 此脚本用于人形和鱿鱼形态之间的切换
public class FormChange : MonoBehaviour
{
    [Header("参数")]
    public float stayTime;
    public string systemName;
    [Header("事件")]
    [SerializeField] private AsyncBoolEventChannel squidEvent;
    [SerializeField] private AsyncBoolEventChannel specialEvent;
    [SerializeField] private AsyncBoolEventChannel fireEvent;
    [SerializeField] private AsyncEventChannel fireTriggerEvent;
    [SerializeField] private AsyncBoolEventChannel holdEvent;
    [SerializeField] private EventChannel ToSquidAnimDoneEvent;
    [SerializeField] private EventChannel InterruptEvent;
    [SerializeField] private EventChannel SHumanExitEvent;
    [SerializeField] private BoolEventChannel inkAreaEvent;
    [SerializeField] private EventChannel EquipLoadDoneEvent;
    [Header("数据")]
    #region data
    // 总之这边之后会有一个SpecialWeapon的数据，用以获取该大招下是否可以变为鱿鱼形态
    public GameObject squid;
    public GameObject Shuman;
    private SkinnedMeshRenderer[] humanRenderers;
    private SkinnedMeshRenderer[] squidRenderers;
    private SkinnedMeshRenderer[] ShumanRenderers;
    #endregion
    private enum FormState
    {
        Humam,
        SHuman,
        Squid,
    }

    private FormState formState = FormState.Humam;
    private bool _isInInkArea;    
    
    void OnEnable()
    {
        squidEvent.OnEventRaised += HandleBoolEvent;
        specialEvent.OnEventRaised += HandleBoolEvent;
        fireEvent.OnEventRaised += HandleBoolEvent;
        fireTriggerEvent.OnEventRaised += HandleTriggerEvent;
        holdEvent.OnEventRaised += HandleBoolEvent;
        SHumanExitEvent.OnEventRaised += HandleSHumanExitEvent;
        inkAreaEvent.OnEventRaised += HandleInkAreaEvent;
        EquipLoadDoneEvent.OnEventRaised += RendererInitialize;
    }

    void OnDisable()
    {
        squidEvent.OnEventRaised -= HandleBoolEvent;
        specialEvent.OnEventRaised -= HandleBoolEvent;
        fireEvent.OnEventRaised -= HandleBoolEvent;
        fireTriggerEvent.OnEventRaised -= HandleTriggerEvent;
        holdEvent.OnEventRaised -= HandleBoolEvent;
        SHumanExitEvent.OnEventRaised -= HandleSHumanExitEvent;
        inkAreaEvent.OnEventRaised -= HandleInkAreaEvent;
        EquipLoadDoneEvent.OnEventRaised -= RendererInitialize;
    }

    async void HandleBoolEvent(InputType inputType, bool value)
    {
        switch (formState)
        {
            case FormState.Humam:
                if (inputType == InputType.Squid && value)
                {
                    await WaitSignalCome(ToSquidAnimDoneEvent);
                    DisableAllRenderer(humanRenderers);
                    if(!_isInInkArea) EnableAllRenderer(squidRenderers);
                    formState = FormState.Squid;
                    
                }
                break;
            case FormState.SHuman:
                if (inputType == InputType.Squid && value)
                {
                    await WaitSignalCome(ToSquidAnimDoneEvent);
                    DisableAllRenderer(humanRenderers);
                    if(!_isInInkArea) EnableAllRenderer(squidRenderers);
                    formState = FormState.Squid;
                }
                
                if (inputType != InputType.Squid && value)
                {
                    InterruptEvent.Raise();
                }

                break;
            case FormState.Squid:
                if (inputType != InputType.Squid && value)
                {
                    EnableAllRenderer(humanRenderers);
                    DisableAllRenderer(squidRenderers);
                    DisableAllRenderer(ShumanRenderers);
                    formState = FormState.Humam;
                }

                if (inputType == InputType.Squid && !value)
                {
                    DisableAllRenderer(squidRenderers);
                    EnableAllRenderer(ShumanRenderers);
                    formState = FormState.SHuman;
                    await WaitToHumanInterrupt();
                }
                break;
        }
    }

    async void HandleTriggerEvent(InputType inputType)
    {
        switch (formState)
        {
            case FormState.Humam:
                await HumanController(inputType, true);
                break;
            case FormState.SHuman:
                await SHumanController(inputType, true);
                break;
            case FormState.Squid:
                await SquidController(inputType, true);
                break;
        }
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

    private async UniTask HumanController(InputType inputType, bool value)
    {
        if (inputType == InputType.Squid && value)
        {
            //此时还需要等待那个ToSquid动画播放完毕
            await WaitSignalCome(ToSquidAnimDoneEvent);
            DisableAllRenderer(humanRenderers);
            EnableAllRenderer(squidRenderers);
            formState = FormState.Squid;
        }
    }

    private async UniTask SquidController(InputType inputType, bool value)
    {
        if (inputType != InputType.Squid && value)
        {
            EnableAllRenderer(humanRenderers);
            DisableAllRenderer(squidRenderers);
            formState = FormState.Humam;
        }

        if (inputType == InputType.Squid && value == false)
        {
            DisableAllRenderer(squidRenderers);
            EnableAllRenderer(ShumanRenderers);
            formState = FormState.SHuman;
            await WaitToHumanInterrupt();
        }
    }

    private async UniTask SHumanController(InputType inputType, bool value)
    {
        if (inputType == InputType.Squid && value)
        {
            await WaitSignalCome(ToSquidAnimDoneEvent);
            DisableAllRenderer(ShumanRenderers);
            EnableAllRenderer(squidRenderers);
            formState = FormState.Squid;
        }

        if (inputType != InputType.Squid && value)
        {
            InterruptEvent.Raise();
        }
    }

    private UniTask WaitSignalCome(EventChannel eventChannel)
    {
        var completionSource = new UniTaskCompletionSource();
        void Handler()
        {
            completionSource.TrySetResult();
            eventChannel.OnEventRaised -= Handler;
        }
        eventChannel.OnEventRaised += Handler;
        return completionSource.Task;
    }

    private async UniTask WaitToHumanInterrupt()
    {
        UniTask InterruptTask = WaitSignalCome(InterruptEvent);
        UniTask timeOutTask = UniTask.Delay((int)(stayTime * 1000));
        await UniTask.WhenAny(timeOutTask, InterruptTask);
        SHumanExitEvent.Raise();
    }

    private void HandleSHumanExitEvent()
    {
        EnableAllRenderer(humanRenderers);
        DisableAllRenderer(ShumanRenderers);
        DisableAllRenderer(squidRenderers);
        formState = FormState.Humam;
    }

    private void HandleInkAreaEvent(bool value)
    {
        _isInInkArea = value;
        if (value && formState == FormState.Squid)
        {
            DisableAllRenderer(squidRenderers);
        }
        else if (!value && formState == FormState.Squid)
        {
            EnableAllRenderer(squidRenderers);
        }
    }

    private void RendererInitialize()
    {
        squidRenderers = squid.GetComponentsInChildren<SkinnedMeshRenderer>();
        humanRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        ShumanRenderers = Shuman.GetComponentsInChildren<SkinnedMeshRenderer>();
        DisableAllRenderer(squidRenderers);
        DisableAllRenderer(ShumanRenderers);        
    }

}
