using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUIControllrt : MonoBehaviour
{

    public Button return_btn;
    private bool is_end = false;
    void Start()
    {
        return_btn.onClick.AddListener(() => ReturnHomeEvent());
    }
    void ReturnHomeEvent()
    {
        SceneDirector.Instance.LoadScene("Home");
    }
    public void GameEndChannel()
    {
        is_end = false;
    }
    public bool is_End()
    {
        return is_end;
    }

}
