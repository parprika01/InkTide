using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public struct UserInfoRequestMessage : NetworkMessage
{
    public string account;
}

public struct UserInfoResponseMessage : NetworkMessage
{
    public string playerName;
    public string nameBackground;
    public string mainWeapon;
    public string eyebrow;
    public string hair;
    public string top;
    public string bottom;
    public string shoes;
    public string head;
    public string victoryAction;
    public string color;
    public string colorIndex;
}

public struct ModifyUserInfoRequestMessage : NetworkMessage
{
    public string account;
    public string settingType;
    public string settingValue;
}

public struct ModifyUserInfoResponseMessage : NetworkMessage
{
    public bool modifyResult;
}