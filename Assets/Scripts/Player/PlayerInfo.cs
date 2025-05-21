using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance {get; private set;}
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
}
