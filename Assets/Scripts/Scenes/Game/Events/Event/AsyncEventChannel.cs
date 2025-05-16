using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(menuName = "Events/AsyncEventChannel")]

public class AsyncEventChannel : ScriptableObject
{
    public event Action<InputType> OnEventRaised;
    public event Action<string> OnEventReady;
    public event Action OnEventCanceled;
    public SignalType signalType = SignalType.Trigger;
    public InputType inputType;    
    public List<string> systems = new List<string>{"animation"};
    public void Raise(InputType inputType) => OnEventRaised?.Invoke(inputType);
    public void Ready(string system) => OnEventReady?.Invoke(system);
    public void Cancel() => OnEventCanceled?.Invoke();

    public UniTask WaitForEventAsync(CancellationToken token)
    {
        var completionSource = new UniTaskCompletionSource();
        var allSystems = new HashSet<string>(systems);

        void Handler(string system){
            if(allSystems.Contains(system)){
                allSystems.Remove(system);
                if(allSystems.Count == 0){
                    OnEventReady -= Handler;
                    completionSource.TrySetResult();
                }
            }
        }
        var registration = token.Register(() => {
            OnEventReady -= Handler;
            //Debug.Log(inputType +"的Wait被打断了");
            completionSource.TrySetCanceled(token);
        });      

        OnEventReady += Handler;
        return completionSource.Task;
    }
}

public class AsyncEventChannel<T> : ScriptableObject
{
    public event Action<InputType,T> OnEventRaised;
    public event Action<string> OnEventReady;
    public event Action OnEventCanceled;
    public SignalType signalType = SignalType.Bool;
    public InputType inputType;
    public List<string> systems = new List<string>{"animation"};
    public void Raise(InputType inputType, T value) => OnEventRaised?.Invoke(inputType, value);
    public void Ready(string system)
    {
        //Debug.Log(inputType + "animation_bool_false的通知");
        OnEventReady?.Invoke(system);
    }
    public void Cancel() => OnEventCanceled?.Invoke();

    public UniTask WaitForEventAsync(CancellationToken token)
    {
        var completionSource = new UniTaskCompletionSource();
        var allSystems = new HashSet<string>(systems);
        OnEventReady += Handler;
        //Debug.Log("OnEventReady subscribers: " + OnEventReady?.GetInvocationList().Length);
        void Handler(string system){
            //Debug.Log(inputType + "接收到系统处理完毕通知，在列表中" + allSystems.Contains(system));
            if(allSystems.Contains(system)){
                allSystems.Remove(system);
                if(allSystems.Count == 0){
                    OnEventReady -= Handler;
                    //Debug.Log("各系统处理完毕！");
                    completionSource.TrySetResult();
                }
            }
        }

        var registration = token.Register(() => {
            //Debug.Log(inputType + "中断执行停止等待");
            OnEventReady -= Handler;
            completionSource.TrySetCanceled(token);
        });      

        
        return completionSource.Task;
    }
} 
