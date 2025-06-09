using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class MoveSystem : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("事件")]
    [SerializeField] private Vector2EventChannel moveEvent;
    [SerializeField] private BoolEventChannel fireEvent;
    [SerializeField] private EventChannel jumpEvent;
    [SerializeField] private BoolEventChannel groundEvent;
    [SerializeField] private BoolEventChannel slopeEvent;

    private Camera cam;
    private Rigidbody rb;
    private Vector3 targetDirection;
    private Quaternion freeRotation;
    private bool isFireLocked = true;
    private bool isFire;
    private bool isOnGround;
    private bool isOnSlope;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isFire && isFireLocked)
        {
            UpdateDirection();
        }
    }

    private void OnEnable()
    {
        moveEvent.OnEventRaised += HandleMove;
        fireEvent.OnEventRaised += HandleFire;
        jumpEvent.OnEventRaised += HandleJump;
        groundEvent.OnEventRaised += HandleGround;
        slopeEvent.OnEventRaised += HandleSlope;
    }

    private void OnDisable()
    {
        // 取消订阅防止内存泄漏
        moveEvent.OnEventRaised -= HandleMove;
        fireEvent.OnEventRaised -= HandleFire;
        jumpEvent.OnEventRaised -= HandleJump;
        groundEvent.OnEventRaised -= HandleGround;
        slopeEvent.OnEventRaised -= HandleSlope;
    }

    #region EventHandles
    private void HandleMove(Vector2 moveInput)
    {
        UpdateTargetDirection(moveInput);
        UpdateMotion(moveInput);
    }

    private void HandleFire(bool fire)
    {
        isFire = fire;
    }

    private void HandleJump()
    {
        Jump();
    }

    private void HandleGround(bool ground)
    {
        isOnGround = ground;
    }

    private void HandleSlope(bool slope)
    {
        isOnSlope = slope;
    }
    #endregion

    private void UpdateTargetDirection(Vector2 moveInput)
    {  
        var forward = cam.transform.forward;
        forward.y = 0;
        var right = cam.transform.right;
        
        targetDirection = (forward * moveInput.y + right * moveInput.x).normalized;        
    }

    // 暂未做斜坡位移处理
    private void UpdateMotion(Vector2 moveInput)
    {
        UpdateDirection();
        //更新速度
        var speed = moveInput.sqrMagnitude;//Mathf.Sqrt(Mathf.Pow(moveInput.x, 2) + Mathf.Pow(moveInput.y, 2));
        //transform.position += targetDirection * speed * moveSpeed * Time.deltaTime;
        rb.AddForce(targetDirection * speed * acceleration, ForceMode.Force);
        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }
    }
    
    private void UpdateDirection()
    {
        //更新方向
        if(isFire && isFireLocked)
            freeRotation = Quaternion.LookRotation(cam.transform.forward, transform.up);
        else
            freeRotation = Quaternion.LookRotation(targetDirection, transform.up);
        var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
        var eulerY = transform.eulerAngles.y;
        if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
        var euler = new Vector3(0, eulerY, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * Time.deltaTime);        
    }    
    
    private void Jump()
    {
        if(isOnGround){
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
        }
    }


}
