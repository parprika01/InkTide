using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputPublisher : MonoBehaviour
{
    [SerializeField] private SettingChangeEventChannel settingChangeEvent;
    private string cloth = "00";
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (cloth == "00")
            {
                cloth = "01";
            }
            else
            {
                cloth = "00";
            }
            settingChangeEvent.Raise((SettingType.Cloth, cloth));
        }
    }
}
