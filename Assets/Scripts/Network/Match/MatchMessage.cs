using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public struct MatchRequestMessage : NetworkMessage
{
    public string playerName;
    public string mainWeapon;
    public string subWeapon;
    public string specialWeapon;
}

public struct MatchResponseMessage : NetworkMessage
{
    
}
