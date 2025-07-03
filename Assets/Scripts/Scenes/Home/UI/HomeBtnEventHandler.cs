using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class HomeBtnEventHandler : MonoBehaviour
{
    public Button matchbtn;
    public Button settingbtn;
    public HomeSceneChange HSC;
    private Image m_icon;
    private Image s_icon;

    void Start()
    {
        m_icon = matchbtn.transform.Find("high").GetComponent<Image>();
        m_icon.gameObject.SetActive(false);
        s_icon = settingbtn.transform.Find("high").GetComponent<Image>();
        s_icon.gameObject.SetActive(false);
        matchbtn.onClick.AddListener(() => MacthEventHandler(matchbtn));
        settingbtn.onClick.AddListener(() => SettingEventHandler(settingbtn));
    }
    void MacthEventHandler(Button btn)
    {
        MatchBtn mbtn = btn.gameObject.GetComponent<MatchBtn>();
        mbtn.StopBreathing();
        //显示背景图片
        m_icon.gameObject.SetActive(true);
        SceneDirector.Instance.LoadScene("Match");
    }
    void SettingEventHandler(Button btn)
    {
        SettingBtn mbtn = btn.gameObject.GetComponent<SettingBtn>();
        mbtn.StopBreathing();
        //显示背景图片
        s_icon.gameObject.SetActive(true);
        SceneDirector.Instance.LoadScene("Setting");
    }
}