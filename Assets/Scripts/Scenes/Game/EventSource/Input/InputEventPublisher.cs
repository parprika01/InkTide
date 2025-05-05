using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputEventPublisher : MonoBehaviour
{
    #region 事件通道
    [Header("事件通道")]
    [SerializeField] private EventChannel<Vector2> moveEvent;
    [SerializeField] private EventChannel<bool> fireEvent;
    [SerializeField] private EventChannel fireTriggerEvent;
    [SerializeField] private EventChannel niceEvent;
    [SerializeField] private EventChannel comeOnEvent;
    [SerializeField] private EventChannel<bool> holdEvent;
    [SerializeField] private EventChannel throwEvent;
    [SerializeField] private EventChannel jumpEvent;
    [SerializeField] private EventChannel<bool> squidEvent;
    #endregion
    private PlayerInput playerInput;
    private Vector2 _currentMoveInput;
    

    void Awake()
    {
        playerInput = new PlayerInput();        
        playerInput.player.move.performed += OnMovePerformed;
        playerInput.player.move.canceled += OnMoveCanceled;
        
        playerInput.player.fire.started += _ => fireEvent.Raise(true);
        playerInput.player.fire.canceled += _ => fireEvent.Raise(false);
        playerInput.player.fire.started += _ => fireTriggerEvent.Raise();

        playerInput.player.nice.started += _ => niceEvent.Raise();
        playerInput.player.come_on.started += _ => comeOnEvent.Raise();

        playerInput.player.sub_weapon.started += _ => holdEvent.Raise(true);
        playerInput.player.sub_weapon.canceled += _ => holdEvent.Raise(false);        
        playerInput.player.sub_weapon.canceled += _ => throwEvent.Raise();

        playerInput.player.jump.started += _ => jumpEvent.Raise();

        playerInput.player.squid.started += _ => squidEvent.Raise(true);
        playerInput.player.squid.canceled += _ => squidEvent.Raise(false);
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

    private void Update()
    {
        if (_currentMoveInput != Vector2.zero)
        {
            moveEvent.Raise(_currentMoveInput);
        }
    }     
}

