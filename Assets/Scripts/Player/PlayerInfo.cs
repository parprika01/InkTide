using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance {get; private set;}
    [Header("Event")]
    [SerializeField] private BoolEventChannel loginEvent;
    [SerializeField] private SettingChangeEventChannel settingChangeEvent;
    // 采用字符串的形式记录玩家信息便于与数据库做交互。
    [Header("Player Info")]
    // other
    public string hair;
    public string eyebrows;
    public string bottom;
    // clothes
    public string cloth;
    public string shoes;
    public string head;
    // weapon
    public string weapon;
    // victory action
    public string action;
    // color
    public string color;
    public string colorIndex;
    // name
    public string playerName;
    public string nameBackground;
    
    private string account;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        loginEvent.OnEventRaised += HandleLogin;
        settingChangeEvent.OnEventRaised += HandleSettingChange;
    }

    private void OnDisable()
    {
        loginEvent.OnEventRaised -= HandleLogin;
        settingChangeEvent.OnEventRaised -= HandleSettingChange;
    }

    private void Start()
    {
        NetworkClient.RegisterHandler<UserInfoResponseMessage>(OnUserInfoResponse);
    }

    public void SetAccount(string account)
    {
        this.account = account;
    }    
    
    private void HandleLogin(bool success)
    {
        if (success)
        {
            UserInfoRequestMessage msg = new UserInfoRequestMessage
            {
                account = account
            };
            
            NetworkClient.Send(msg);
        }
    }

    private void HandleSettingChange((SettingType, string) settingData)
    {
        string settingType = "";
        string settingValue = settingData.Item2;
        switch (settingData.Item1)
        {
            case SettingType.Hair:
                hair = settingData.Item2;
                settingType = "hair";
                break;
            case SettingType.Eyebrows:
                eyebrows = settingData.Item2;
                settingType = "eyebrows";
                break;
            case SettingType.Bottom:
                bottom = settingData.Item2;
                settingType = "bottom";
                break;
            case SettingType.Cloth:
                cloth = settingData.Item2;
                settingType = "top";
                break;
            case SettingType.Shoes:
                shoes = settingData.Item2;
                settingType = "shoes";
                break;
            case SettingType.HeadEquip:
                head = settingData.Item2;
                settingType = "head";
                break;
            case SettingType.VictoryAction:
                action = settingData.Item2;
                settingType = "victoryAction";
                break;
            case SettingType.PlayerName:
                playerName = settingData.Item2;
                settingType = "playerName";
                break;
            case SettingType.NameBackground:
                nameBackground = settingData.Item2;
                settingType = "nameBackground";
                break;
            case SettingType.Color:
                color = settingData.Item2;
                settingType = "color";
                break;
            case SettingType.ColorIndex:
                colorIndex = settingData.Item2;
                settingType = "colorIndex";
                break;
        }

        ModifyUserInfoRequestMessage msg = new ModifyUserInfoRequestMessage
        {
            account = account,
            settingType = settingType,
            settingValue = settingValue
        };
        
        NetworkClient.Send(msg);
    }

    private void OnUserInfoResponse(UserInfoResponseMessage msg)
    {
        this.hair = msg.hair;
        this.bottom = msg.bottom;
        this.eyebrows = msg.eyebrow;
        this.cloth = msg.top;
        this.shoes = msg.shoes;
        this.head = msg.head;
        this.weapon = msg.mainWeapon;
        this.action = msg.victoryAction;
        this.color = msg.color;
        this.colorIndex = msg.colorIndex;
        this.playerName = msg.playerName;
        this.nameBackground = msg.nameBackground;
    }
}
