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
    [Header("数据")]
    #region data
    // 总之这边之后会有一个SpecialWeapon的数据，用以获取该大招下是否可以变为鱿鱼形态
    public GameObject squid;
    public GameObject Shuman;
    private SkinnedMeshRenderer[] humanRenderers;
    private SkinnedMeshRenderer[] squidRenderers;
    private SkinnedMeshRenderer[] ShumanRenderers;
    #endregion
    private enum FormState{
        Humam,
        SHuman,
        Squid
    }

    private FormState formState = FormState.Humam;

    void Awake()
    {
        squidRenderers =  squid.GetComponentsInChildren<SkinnedMeshRenderer>();
        humanRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        ShumanRenderers = Shuman.GetComponentsInChildren<SkinnedMeshRenderer>();        
    }

    void OnEnable()
    {
        squidEvent.OnEventRaised += HandleBoolEvent;
        specialEvent.OnEventRaised += HandleBoolEvent;
        fireEvent.OnEventRaised += HandleBoolEvent;
        fireTriggerEvent.OnEventRaised += HandleTriggerEvent;
        holdEvent.OnEventRaised += HandleBoolEvent;
    }

    void OnDisable()
    {
        squidEvent.OnEventRaised -= HandleBoolEvent;
        specialEvent.OnEventRaised -= HandleBoolEvent;
        fireEvent.OnEventRaised -= HandleBoolEvent;
        fireTriggerEvent.OnEventRaised -= HandleTriggerEvent;
        holdEvent.OnEventRaised -= HandleBoolEvent;
    }

    async void HandleBoolEvent(InputType inputType, bool value)
    {
        switch (formState) {
            case FormState.Humam:
                if(inputType == InputType.Squid && value){
                    //此时还需要等待那个ToSquid动画播放完毕
                    await WaitSignalCome(ToSquidAnimDoneEvent);
                    DisableAllRenderer(humanRenderers);
                    EnableAllRenderer(squidRenderers);
                    formState = FormState.Squid;
                }
                break;
            case FormState.SHuman:
                if(inputType == InputType.Squid && value){
                    await WaitSignalCome(ToSquidAnimDoneEvent);
                    DisableAllRenderer(ShumanRenderers);
                    EnableAllRenderer(squidRenderers);
                    formState = FormState.Squid;
                } 

                if(inputType != InputType.Squid && value){
                    InterruptEvent.Raise();
                }

                break;
            case FormState.Squid:
                if(inputType != InputType.Squid && value){
                    EnableAllRenderer(humanRenderers);
                    DisableAllRenderer(squidRenderers);
                    formState = FormState.Humam;
                }

                if(inputType == InputType.Squid && false){
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
        switch (formState) {
            case FormState.Humam:
                HumanController(inputType, true);
                break;
            case FormState.SHuman:
                SHumanController(inputType, true);
                break;
            case FormState.Squid:
                SquidController(inputType, true);
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

    private async void HumanController(InputType inputType, bool value)
    {
        if(inputType == InputType.Squid && value){
            //此时还需要等待那个ToSquid动画播放完毕
            await WaitSignalCome(ToSquidAnimDoneEvent);
            DisableAllRenderer(humanRenderers);
            EnableAllRenderer(squidRenderers);
            formState = FormState.Squid;
        }
    }

    private async void SquidController(InputType inputType, bool value)
    {
        if(inputType != InputType.Squid && value){
            EnableAllRenderer(humanRenderers);
            DisableAllRenderer(squidRenderers);
            formState = FormState.Humam;
        }

        if(inputType == InputType.Squid && false){
            DisableAllRenderer(squidRenderers);
            EnableAllRenderer(ShumanRenderers);
            formState = FormState.SHuman;
            await WaitToHumanInterrupt();
        }
    }

    private async void SHumanController(InputType inputType, bool value)
    {
        if(inputType == InputType.Squid && value){
            await WaitSignalCome(ToSquidAnimDoneEvent);
            DisableAllRenderer(ShumanRenderers);
            EnableAllRenderer(squidRenderers);
            formState = FormState.Squid;
        } 

        if(inputType != InputType.Squid && value){
            InterruptEvent.Raise();
        }
    }

    private UniTask WaitSignalCome(EventChannel eventChannel)
    {
        var completionSource = new UniTaskCompletionSource();
        void Handler(){
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
    }

}
