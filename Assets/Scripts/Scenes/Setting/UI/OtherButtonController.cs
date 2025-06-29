using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class OtherButtonController : MonoBehaviour
{
    [Header("Button")]
    public Button btn_TV;
    public Button btn_handheld;
    public Button btn_style;
    public Button btn_amiibo;
    [Header("Canvas 信息")]
    public OtherCanvas lastCanvas;//设置一个默认值
    public Button lastbtn;
    public OtherCanvas[] canvaslist;
    // public 
    // Start is called before the first frame update
    void Start()
    {
        btn_TV.onClick.AddListener(() => BtnClickEventHandler(btn_TV,"TV"));
        btn_style.onClick.AddListener(() => BtnClickEventHandler(btn_style,"Style"));
        btn_amiibo.onClick.AddListener(() => BtnClickEventHandler(btn_amiibo,"Amiibo"));
        btn_handheld.onClick.AddListener(() => BtnClickEventHandler(btn_handheld,"Handheld"));
        foreach (var canvas in canvaslist)
        {
            canvas.gameObject.SetActive(false);
        }
        lastCanvas.gameObject.SetActive(true);
        
    }
    private void SwitchTo(Button btn, OtherCanvas cur)
    {
        Transform l_icon = lastbtn.transform.Find("icon");
        Transform l_textTransform = lastbtn.transform.Find("Text");
        Transform c_textTransform = btn.transform.Find("Text");
        Transform c_icon = btn.transform.Find("icon");
        //icon的修改
        l_icon.gameObject.SetActive(false);
        c_icon.gameObject.SetActive(true);
        //canvas的切换
        lastCanvas.gameObject.SetActive(false);
        cur.gameObject.SetActive(true);
        //text颜色切换
        TextMeshProUGUI l_tmp = l_textTransform.GetComponent<TextMeshProUGUI>();
        l_tmp.color = new Color(1f, 1f, 1f); // 白色
        TextMeshProUGUI c_tmp = c_textTransform.GetComponent<TextMeshProUGUI>();
        c_tmp.color = new Color(1f,1f,73f/255f);
        lastCanvas = cur;
        lastbtn = btn;
    }

    OtherCanvas SearchCanvasByname(string name)
    {
        foreach (var canvas in canvaslist)
        {
            if (canvas.canvasname == name)
            {
                return canvas;
            }
        }
        return null;
    }
    // Update is called once per frame
    private void BtnClickEventHandler(Button btn,string name)
    {
        OtherCanvas cur = SearchCanvasByname(name);
        if (cur == null)
        {
            Debug.Log("找不到对应的Canvas！");
        }
        if (lastCanvas != cur)
        {
            SwitchTo(btn,cur);
        }
        if (lastCanvas == cur)
        {
            return;
        }
    }


}
