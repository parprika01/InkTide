using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
