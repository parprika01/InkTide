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
    VictoryAction,
    PlayerName,
    NameBackground,
    Color,
    ColorIndex
}

[CreateAssetMenu(menuName = "Events/EventChannels/SettingChange")]
public class SettingChangeEventChannel : EventChannel<(SettingType, string)> {}
