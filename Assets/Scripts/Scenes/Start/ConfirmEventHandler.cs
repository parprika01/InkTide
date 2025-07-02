using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmEventHandler : MonoBehaviour
{
    public  Button loginbtn;
    private Image lh_icon;
    public Button registerbtn;
    private Image rh_icon;

    public TMP_InputField l_id_inputField;
    public TMP_InputField r_id_inputField;
    public TMP_InputField l_pwd_inputField;
    public TMP_InputField r_pwd_inputField;

    void Start()
    {

        rh_icon = registerbtn.transform.Find("high").GetComponent<Image>();
        rh_icon.gameObject.SetActive(false);
        lh_icon = loginbtn.transform.Find("high").GetComponent<Image>();
        lh_icon.gameObject.SetActive(false);

        loginbtn.onClick.AddListener(() => LoginEventHandler());
        registerbtn.onClick.AddListener(() => RegisterEventHandler());

    }
    private void LoginEventHandler()
    {
        lh_icon.gameObject.SetActive(true);
        //获取当前输入框的内容
        string id = l_id_inputField.text;
        string pwd = l_pwd_inputField.text;
        
        //数据库操作

    }

    private void RegisterEventHandler()
    {
        rh_icon.gameObject.SetActive(true);
        string id = r_id_inputField.text;
        string pwd = r_pwd_inputField.text;
        //数据库操作
    }
}
