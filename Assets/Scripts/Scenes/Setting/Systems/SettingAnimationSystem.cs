using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingAnimState
{
    Wait,
    WeaponHold,
    Victory
}
public class SettingAnimationSystem : MonoBehaviour
{
    [Header("event")]
    [SerializeField] private SettingChangeEventChannel settingChangeEventChannel;
    [SerializeField] private SettingAnimStateChangeChannel settingAnimationStateEventChannel;

    #region animation code
    int wait_code;
    int weaponHold_code;
    int victory_code;
    #endregion

    private Animator animator;
    private SettingAnimState currentState = SettingAnimState.WeaponHold;

    void Awake()
    {
        animator = GetComponent<Animator>();

        wait_code = Animator.StringToHash("wait");
        weaponHold_code = Animator.StringToHash("weaponHold");
        victory_code = Animator.StringToHash("victory");
    }

    void OnEnable()
    {
        settingChangeEventChannel.OnEventRaised += HandlesettingChangeEvent;
        settingAnimationStateEventChannel.OnEventRaised += HandlesettingAnimationStateEvent;
    }

    void OnDisable()
    {
        settingChangeEventChannel.OnEventRaised -= HandlesettingChangeEvent;
        settingAnimationStateEventChannel.OnEventRaised -= HandlesettingAnimationStateEvent;        
    }

    void HandlesettingChangeEvent((SettingType, string) settingData)
    {
        
    }

    void HandlesettingAnimationStateEvent(SettingAnimState state)
    {
        if (state == currentState) return;
        switch (state)
        {
            case SettingAnimState.WeaponHold:
                animator.SetTrigger(weaponHold_code);
                break;
            case SettingAnimState.Victory:
                animator.SetTrigger(victory_code);
                break;
            case SettingAnimState.Wait:
                animator.SetTrigger(wait_code);
                break;
        }
        currentState = state;
    }
}
