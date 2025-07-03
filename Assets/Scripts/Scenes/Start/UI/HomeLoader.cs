using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HomeLoader : MonoBehaviour
{
    public Button Loginbtn;
    void Start()
    {
        //控制当前场景切换时对象不会被销毁
        GameObject.DontDestroyOnLoad(this.gameObject);
        Loginbtn.onClick.AddListener(() => LoadHomeScene());
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LoadHomeScene()
    {
        
    }
}
