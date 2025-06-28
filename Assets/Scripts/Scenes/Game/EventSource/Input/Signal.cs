using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public enum InputType
{
    Fire,
    FireTrigger,
    Nice,
    ComeOn,
    Hold,
    Squid,
    Special,
} 

public abstract class Signal
{
    public SignalType signalType { get; protected set; }
    public InputType inputType{ get; protected set; }
    public bool Value = true;
    public abstract object GetRawEventChannel();
}

public class TriggerSignal : Signal
{
    public AsyncEventChannel EventChannel { get; }

    public TriggerSignal(AsyncEventChannel eventChannel, InputType inputType)
    {
        signalType = SignalType.Trigger;
        EventChannel = eventChannel;
        this.inputType = inputType;
    }
    public override object GetRawEventChannel() => EventChannel;
}

public class BoolSignal : Signal
{
    public AsyncBoolEventChannel EventChannel { get; }

    public BoolSignal(AsyncBoolEventChannel eventChannel, InputType inputType, bool value)
    {
        signalType = SignalType.Bool;
        Value = value;
        this.inputType = inputType;
        EventChannel = eventChannel;
    }
    public override object GetRawEventChannel() => EventChannel;
}

public class SignalManager
{
    public  List<Signal> signals;
    private Signal currentSignal;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    public SignalManager(){
        signals = new List<Signal>();
    }

    public bool Empty()
    {
        return signals.Count == 0;
    }

    public async void Add(Signal signal){
        //Debug.Log(signal.inputType + "信号加入中~值为：" + signal.Value);
        if(signals.Count > 0){
            cancellationTokenSource.Cancel();
            if (currentSignal.signalType == SignalType.Trigger){
                signals.Remove(currentSignal);
            } else if(signal.signalType == SignalType.Bool && signal.Value == false) {
                signals.RemoveAll(s => s.inputType == signal.inputType && s.Value == true);
                //Debug.Log("我们开删了");
            }
        }

        signals.Add(signal);
        //Debug.Log("当前信号数量：" + signals.Count);
        currentSignal = signal;
        await SignalProcess();
    }

    public async UniTask SignalProcess()
    {
        while(signals.Count > 0){
            //Debug.Log("处理中~当前信号数：" + signals.Count);
            //PrintSignals();
            currentSignal = signals[signals.Count - 1];
            try {
                await Processing(cancellationTokenSource.Token);
                //Debug.Log(currentSignal.inputType + "信号处理完毕" + currentSignal.Value);
                signals.Remove(currentSignal);
            } catch (OperationCanceledException) {
                //Debug.Log("有新信号进入");
                if(currentSignal.signalType == SignalType.Bool && currentSignal.Value == false)
                    signals.Remove(currentSignal); 
                return;

            } finally {
                //Debug.Log("开始销毁cts");
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }             
        }              
    }

    public async UniTask Processing(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        //Debug.Log("现在处理的信号是：" + currentSignal.inputType + "状态值为：" + currentSignal.Value);
        switch(currentSignal.signalType) {
            case SignalType.Bool:
                var boolEventChannel = currentSignal.GetRawEventChannel() as AsyncEventChannel<bool>;
                boolEventChannel.Raise(currentSignal.inputType, currentSignal.Value);                
                await boolEventChannel.WaitForEventAsync(token);                
                break;
            case SignalType.Trigger:
                var eventChannel = currentSignal.GetRawEventChannel() as AsyncEventChannel;
                eventChannel.Raise(currentSignal.inputType);                
                await eventChannel.WaitForEventAsync(token);                
                break;
        }               
    }

    private void PrintSignals()
    {
        var signalInfos = signals.Select(signal => $"变量类型：{signal.inputType} 变量值：{signal.Value}");
        var output = string.Join(" | ", signalInfos);
        //Debug.Log(output);
    }
}
