using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HomeLoader : MonoBehaviour
{

    void Start()
    {
        //控制当前场景切换时对象不会被销毁
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    public void LoadHomeScene(string userID, string userPwd)
    {
        SceneDirector.Instance.LoadScene("Home");
        SceneDirector.Instance.userId = userID;
        SceneDirector.Instance.userpwd = userPwd;

    }
}
