using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameUIController : MonoBehaviour
{
    public CountdownUI countdownUI;
    public Timer timer;
    [Header("self info")]
    public string path = "Weapon/Main";
    public Image self_bg;
    private string self_weapon_name = "weapon_yellow";
    public Image self_weapon_img;//武器的图片
    private Image self_died_icon;
    private Color self_color = new Color(28f / 255f, 0f / 255f, 237f / 255f);//墨水颜色
    [Header("enemy info")]
    public Image enemy_bg;
    public Image enemy_weapon_img;
    private string enemy_weapon_name = "weapon_yellow_big";
    private Color enemy_color = new Color(1f,1f,75f/255f);
    private Image enemy_died_icon;

    private bool is_gamed = false;//true代表游戏结束
    [Header("other")]
    public RemainController rc;//墨水余量相关UI控制对象

    private

    void Start()
    {
        SetInfo();
        LoadGamingInfo();
        timer.PauseTimer();
        StartCoroutine(countdownUI.StartCountdown());
        // timer.StartTimer();
        //对局中信息加载

    }
    public void SetInfo()
    {
        //根据Director的信息加载
        //self;
        // self_color =
        // enemy_color =
        // self_weapon_name =
        // enemy_weapon_name = 

    }
    void Update()
    {
        if (is_gamed)
        {

        }
    }
    //计算当前地图上的墨水占比
    public void UpdateCurArea()
    {
        // TODO
        // rc.SetCurAllNumber() //传入当前计算的Area值 int
        // rc.SetRemainText(string number);//更新自己剩下的墨水；
    }
    //开启结算
    public void GameEnd()
    {

    }
    //加载游戏对局信息资源
    void LoadGamingInfo()
    {
        //己方信息
        self_died_icon = self_bg.transform.Find("die_icon").GetComponent<Image>();
        enemy_died_icon = enemy_bg.transform.Find("die_icon").GetComponent<Image>();
        self_died_icon.gameObject.SetActive(false);
        enemy_died_icon.gameObject.SetActive(false);
        LoadImage(path + "/" + self_weapon_name, self_weapon_img);
        //敌方信息
        Debug.Log("enemy image path:" + path + "/" + enemy_weapon_name);
        LoadImage(path + "/" + enemy_weapon_name, enemy_weapon_img);
        LoadColor();

    }
    //加载游戏Image相关资源
    public void LoadImage(string pathInResources, Image img)
    {
        // 从Resources文件夹加载Sprite
        Sprite sprite = Resources.Load<Sprite>(pathInResources);

        if (sprite != null)
        {
            // 将加载的Sprite赋值给Image组件
            img.sprite = sprite;
        }
        else
        {
            Debug.LogError("Image not found at path: " + pathInResources);
        }
    }
    //加载游戏Color
    public void LoadColor()
    {
        //pic color
        self_bg.color = self_color;
        enemy_bg.color = enemy_color;
        //mask color;
        Transform maskTransform = self_bg.gameObject.transform.Find("mask");
        Image im = maskTransform.GetComponent<Image>();
        im.color = self_color;
        Transform e_maskTransform = enemy_bg.gameObject.transform.Find("mask");
        Image eim = e_maskTransform.GetComponent<Image>();
        eim.color = enemy_color;

    }

}
