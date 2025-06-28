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

[CreateAssetMenu(fileName = "EventChannel", menuName = "Events/EventChannel")]
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

/*

[CreateAssetMenu(fileName = "BoolEventChannel", menuName = "Events/EventChannels/Bool")]
public class BoolEventChannel : EventChannel<bool> { }


[CreateAssetMenu(fileName = "Vector2EventChannel", menuName = "Events/EventChannels/Vector2")]
public class Vector2EventChannel : EventChannel<Vector2> { }

public enum SettingType
{
    MainWeapon,
    HeadEquip,
    Cloth,
    Shoes,
    Hair,
    Eyebrows,
    Bottom,
    VictoryAction
}

[CreateAssetMenu(fileName = "SettingChangeEventChannel", menuName = "Events/EventChannels/SettingChange")]
public class SettingChangeEventChannel : EventChannel<(SettingType, string)> { }


[CreateAssetMenu(fileName = "SettingAnimStateChangeEventChannel", menuName = "Events/EventChannels/SettingAnimStateChange")]
public class SettingAnimStateChangeChannel : EventChannel<SettingAnimState> {}
*/


