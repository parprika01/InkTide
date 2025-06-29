using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputEventPublisher : MonoBehaviour
{
    #region 事件通道
    [Header("事件通道")]
    [SerializeField] private BoolEventChannel SyncFireEvent;
    [SerializeField] private EventChannel jumpEvent;
    [SerializeField] private Vector2EventChannel moveEvent;
    [SerializeField] private AsyncBoolEventChannel fireEvent;
    [SerializeField] private AsyncEventChannel fireTriggerEvent;
    [SerializeField] private AsyncEventChannel niceEvent;
    [SerializeField] private AsyncEventChannel comeOnEvent;
    [SerializeField] private AsyncBoolEventChannel holdEvent;
    [SerializeField] private AsyncBoolEventChannel squidEvent;
    [SerializeField] private AsyncBoolEventChannel specialEvent;
    [SerializeField] private BoolEventChannel SyncHoldEvent;
    #endregion
    private PlayerInput playerInput;
    private Vector2 _currentMoveInput;
    private SignalManager signalManager;
   

    void Awake()
    {
        playerInput = new PlayerInput(); 
        signalManager = new SignalManager();
        playerInput.player.move.performed += OnMovePerformed;
        playerInput.player.move.canceled += OnMoveCanceled;
        
        playerInput.player.fire.started += _ => BoolSignalRaise(InputType.Fire, fireEvent, true);
        playerInput.player.fire.canceled += _ => BoolSignalRaise(InputType.Fire, fireEvent, false);
        playerInput.player.fire.started += _ => SyncFireEvent.Raise(true);
        playerInput.player.fire.canceled += _ => SyncFireEvent.Raise(false);
        //playerInput.player.fire.started += _ => TriggerSignalRaise(InputType.FireTrigger, fireTriggerEvent);

        playerInput.player.nice.started += _ => TriggerSignalRaise(InputType.Nice, niceEvent);
        playerInput.player.come_on.started += _ => TriggerSignalRaise(InputType.ComeOn, comeOnEvent);

        playerInput.player.sub_weapon.started += _ => BoolSignalRaise(InputType.Hold, holdEvent, true);
        playerInput.player.sub_weapon.canceled += _ => BoolSignalRaise(InputType.Hold, holdEvent, false);
        playerInput.player.sub_weapon.started += _ => SyncHoldEvent.Raise(true);
        playerInput.player.sub_weapon.canceled += _ => SyncHoldEvent.Raise(false);     

        playerInput.player.jump.started += _ => jumpEvent.Raise();

        playerInput.player.squid.started += _ => BoolSignalRaise(InputType.Squid, squidEvent, true);
        playerInput.player.squid.canceled += _ => BoolSignalRaise(InputType.Squid, squidEvent, false);
        
        playerInput.player.special.started += _ => BoolSignalRaise(InputType.Special, specialEvent, true);
        playerInput.player.special.canceled += _ => BoolSignalRaise(InputType.Special, specialEvent, false);
    }

    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }  

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _currentMoveInput = ctx.ReadValue<Vector2>();
        moveEvent.Raise(_currentMoveInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _currentMoveInput = Vector2.zero;
        moveEvent.Raise(_currentMoveInput); // 通知停止移动
    }   

    private void FixedUpdate()
    {
        if (_currentMoveInput != Vector2.zero)
        {
            moveEvent.Raise(_currentMoveInput);
        }
    }


    private void TriggerSignalRaise(InputType inputType, AsyncEventChannel eventChannel){
        // 记得信号的释放
        TriggerSignal signal = new TriggerSignal(eventChannel,inputType);
        signalManager.Add(signal);
    }

    private void BoolSignalRaise(InputType inputType, AsyncBoolEventChannel eventChannel, bool value){
        BoolSignal signal = new BoolSignal(eventChannel,inputType, value);
        signalManager.Add(signal);
    }
}

