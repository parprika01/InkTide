using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

[CreateAssetMenu(menuName = "Events/EventChannels/SettingChange")]
public class SettingChangeEventChannel : EventChannel<(SettingType, string)> {}
