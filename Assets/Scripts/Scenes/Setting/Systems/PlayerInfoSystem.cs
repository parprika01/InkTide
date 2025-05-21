using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoSystem : MonoBehaviour
{
    [SerializeField] private SettingChangeEventChannel settingChangeEventChannel;
    private PlayerInfo playerInfo;

    void Start()
    {
        playerInfo = PlayerInfo.Instance;
        settingChangeEventChannel.OnEventRaised += HandleSettingChangeEvent;
    }

    void HandleSettingChangeEvent((SettingType, string) settingData)
    {
        switch (settingData.Item1)
        {
            case SettingType.MainWeapon:
                playerInfo.weapon = settingData.Item2;
                break;
            case SettingType.Hair:
                playerInfo.hair = settingData.Item2;
                break;
            case SettingType.HeadEquip:
                playerInfo.head = settingData.Item2;
                break;
            case SettingType.Cloth:
                playerInfo.cloth = settingData.Item2;
                break;
            case SettingType.Bottom:
                playerInfo.bottom = settingData.Item2;
                break;
            case SettingType.Eyebrows:
                playerInfo.bottom = settingData.Item2;
                break;
            case SettingType.Shoes:
                playerInfo.shoes = settingData.Item2;
                break;                
        }
    }
    

}
