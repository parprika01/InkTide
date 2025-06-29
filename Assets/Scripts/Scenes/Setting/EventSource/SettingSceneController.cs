using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSceneController : MonoBehaviour
{
    [Header("event")]
    [SerializeField] private EventChannel InitializeEvent;
<<<<<<< Updated upstream
=======
    [Header("Button")]
    public Button returnbtn;
    public Button ootdbtn;
>>>>>>> Stashed changes

    void Start()
    {
        Initialize();
        returnbtn.onClick.AddListener(OnReturnBtnClick);
        ootdbtn.onClick.AddListener(OnOotdBtnClick);
    }
    void Initialize()
    {
        InitializeEvent.Raise();
    }
    //ReturnBtn is Click
    void OnReturnBtnClick()
    {

    }
    //Show style btn
    void OnOotdBtnClick()
    {

    }
}
