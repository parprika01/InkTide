using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum EventType
{
    Fire,
    FireTrigger,
    Throw,
    Hold,
    Nice,
    ComeOn,
    Squid
}

public enum SignalType
{
    Bool,
    Trigger
}

[CreateAssetMenu(menuName = "Events/EventChannel")]
public class EventChannel : ScriptableObject
{
    public event Action OnEventRaised;
    public void Raise() => OnEventRaised?.Invoke();
}

public class EventChannel<T> : ScriptableObject
{
    public event Action<T> OnEventRaised;
    public void Raise(T value) => OnEventRaised?.Invoke(value);
}


public class AsyncEventChannel<EventType, T> : ScriptableObject
{
    public event Action<T> OnEventRaised;
    public SignalType signalType = SignalType.Bool;
}
