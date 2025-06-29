using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MatchClient
{
    public WeaponCollocation weaponCollocation;   
    public MatchClient()
    {
        
    }

    public void GameMatch()
    {
        Dictionary<string, (string, string)> weaponDict = weaponCollocation.GetWeaponRelations();
        string main = PlayerInfo.Instance.weapon;
        MatchRequestMessage msg = new MatchRequestMessage
        {
            playerName = PlayerInfo.Instance.playerName,
            mainWeapon = main,
            subWeapon = weaponDict[main].Item1,
            specialWeapon = weaponDict[main].Item2
        };
        
        NetworkClient.Send(msg);
    }
}
