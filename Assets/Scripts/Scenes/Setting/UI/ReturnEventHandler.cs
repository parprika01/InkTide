using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReturnEventHandler : MonoBehaviour
{
    public Button ret_btn;
    void Start()
    {
        ret_btn.onClick.AddListener(() => ReturnHomeEvent());
    }
    void ReturnHomeEvent()
    {
        // SceneDirector.Instance.LoadScene("Home");
    }

}
