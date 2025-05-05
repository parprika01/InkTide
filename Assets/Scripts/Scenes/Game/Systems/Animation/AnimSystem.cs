using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSystem : MonoBehaviour
{
    public Animator animator;
    public VariableManager variableManager;
    [SerializeField] private float acceleration = 8f;  // 加速系数
    [SerializeField] private float deceleration = 12f; // 减速系数   

    #region animations
    private int fire_bool_code;
    private int fire_trigger_code;
    private int jump_code;
    private int damage_code;
    private int hit_code;
    private int speed_code;
    private int x_speed_code;
    private int y_speed_code;
    private int z_speed_code;
    private int come_on_code;
    private int nice_code;
    private int hold_code;
    private int throw_code;
    private int squid_code;
    private int ground_code;
    private int special_code;
    #endregion
    #region variables
    public Variable fire_bool;
    public Variable fire_trigger;
    public Variable come_on;
    public Variable nice;
    public Variable hold;
    public Variable to_throw;
    #endregion        
    #region animation event
    [SerializeField] private EventChannel<Vector2> moveEvent;
    [SerializeField] private EventChannel<bool> fireEvent;
    [SerializeField] private EventChannel fireTriggerEvent;
    [SerializeField] private EventChannel niceEvent;
    [SerializeField] private EventChannel comeOnEvent;
    [SerializeField] private EventChannel<bool> holdEvent;
    [SerializeField] private EventChannel throwEvent;
    [SerializeField] private EventChannel jumpEvent;
    [SerializeField] private EventChannel<bool> groundEvent;
    [SerializeField] private EventChannel<bool> squidEvent;
    [SerializeField] private EventChannel<bool> specialEvent;
    #endregion
    private Rigidbody rb;
    private bool isOnGround;
    private float currentSpeed;
    private float currentXSpeed;
    private float currentYSpeed;
    private Vector2 lastMoveInput;

    void Awake()
    {
        animator = GetComponent<Animator>();
        variableManager = new VariableManager(animator);
        rb = GetComponent<Rigidbody>();

        #region animation var hash initialize
        fire_bool_code = Animator.StringToHash("fire_bool");
        fire_trigger_code = Animator.StringToHash("fire_trigger");
        jump_code = Animator.StringToHash("jump");
        damage_code = Animator.StringToHash("damage");
        hit_code = Animator.StringToHash("hit");
        speed_code = Animator.StringToHash("speed");
        x_speed_code = Animator.StringToHash("x_speed");
        y_speed_code = Animator.StringToHash("y_speed");
        z_speed_code = Animator.StringToHash("z_speed");
        come_on_code = Animator.StringToHash("come_on");
        nice_code = Animator.StringToHash("nice");
        hold_code = Animator.StringToHash("hold");
        throw_code = Animator.StringToHash("throw");
        squid_code = Animator.StringToHash("squid");
        ground_code = Animator.StringToHash("ground");
        special_code = Animator.StringToHash("special");
        #endregion

        #region variable initialize
        fire_bool = new Variable(2, false, fire_bool_code, false);
        variableManager.AddVariable(fire_bool);

        fire_trigger = new Variable(2, true, fire_trigger_code, false);
        variableManager.AddVariable(fire_trigger);

        come_on = new Variable(3, true, come_on_code);
        variableManager.AddVariable(come_on);

        nice = new Variable(3, true, nice_code);
        variableManager.AddVariable(nice);

        hold = new Variable(2, false, hold_code);
        variableManager.AddVariable(hold);

        to_throw = new Variable(2, true, throw_code);
        variableManager.AddVariable(to_throw);
        #endregion        
    }
    void OnEnable()
    {
        moveEvent.OnEventRaised += HandleMove;
        fireEvent.OnEventRaised += HandleFire;
        fireTriggerEvent.OnEventRaised += HandleFireTrigger;
        niceEvent.OnEventRaised += HandleNice;
        comeOnEvent.OnEventRaised += HandleComeOn;
        holdEvent.OnEventRaised += HandleHold;
        throwEvent.OnEventRaised += HandleThrow;
        jumpEvent.OnEventRaised += HandleJump;
        groundEvent.OnEventRaised += HandleGround;
        specialEvent.OnEventRaised += HandleSpecial;
    }
    void OnDisable()
    {
        moveEvent.OnEventRaised -= HandleMove;
        fireEvent.OnEventRaised -= HandleFire;
        fireTriggerEvent.OnEventRaised -= HandleFireTrigger;
        niceEvent.OnEventRaised -= HandleNice;
        comeOnEvent.OnEventRaised -= HandleComeOn;
        holdEvent.OnEventRaised -= HandleHold;
        throwEvent.OnEventRaised -= HandleThrow;
        jumpEvent.OnEventRaised -= HandleJump;
        groundEvent.OnEventRaised -= HandleGround;
        specialEvent.OnEventRaised -= HandleSpecial;
    }

    #region Event Handler
    private void HandleMove(Vector2 moveInput)
    {
        lastMoveInput = moveInput;       
    }
    private void HandleFire(bool fire)
    {
        if (fire)
            variableManager.ActivateVar(fire_bool);
        else    
            variableManager.DeactivateVar(fire_bool);    
    }
    private void HandleFireTrigger()
    {
        variableManager.ActivateVar(fire_trigger);
    }
    private void HandleNice()
    {
        variableManager.ActivateVar(nice);
    }
    private void HandleComeOn()
    {
        variableManager.ActivateVar(come_on);
    }
    private void HandleHold(bool isHold)
    {
        if (isHold)
            variableManager.ActivateVar(hold);
        else    
            variableManager.DeactivateVar(hold);
    }
    private void HandleThrow()
    {
        variableManager.ActivateVar(to_throw);
    }
    private void HandleGround(bool ground)
    {
        isOnGround = ground;
        animator.SetBool(ground_code, ground);
    }
    private void HandleJump()
    {
        animator.SetTrigger(jump_code);
    }

    private void HandleSpecial(bool special)
    {
        animator.SetBool(special_code, special);
    }

    #endregion
    private void ProcessMovementSmoothing()
{
    // 计算目标值
    float targetSpeed = lastMoveInput.magnitude;
    float targetXSpeed = lastMoveInput.x;
    float targetYSpeed = lastMoveInput.y;

    // 根据是否有输入选择平滑系数
    float smoothFactor = (targetSpeed > 0.01f) ? 
        acceleration * Time.deltaTime : 
        deceleration * Time.deltaTime;

    // 平滑过渡
    currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, smoothFactor);
    currentXSpeed = Mathf.Lerp(currentXSpeed, targetXSpeed, smoothFactor);
    currentYSpeed = Mathf.Lerp(currentYSpeed, targetYSpeed, smoothFactor);

    // 当接近零时强制归零
    if (currentSpeed < 0.01f) currentSpeed = 0f;
    if (Mathf.Abs(currentXSpeed) < 0.01f) currentXSpeed = 0f;
    if (Mathf.Abs(currentYSpeed) < 0.01f) currentYSpeed = 0f;

    // 更新Animator
    animator.SetFloat(speed_code, currentSpeed);
    animator.SetFloat(x_speed_code, currentXSpeed);
    animator.SetFloat(y_speed_code, currentYSpeed);
}
    void Update()
    {
        ProcessMovementSmoothing();
        if(!isOnGround){
            animator.SetFloat(z_speed_code, rb.velocity.z);
        }
    }
}
