using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSystem : MonoBehaviour
{
    public Animator animator;
    public VariableManager variableManager;    
    #region animations
    public int fire_bool_code;
    public int fire_trigger_code;
    public int jump_code;
    public int damage_code;
    public int hit_code;
    public int speed_code;
    public int x_speed_code;
    public int y_speed_code;
    public int z_speed_code;
    public int come_on_code;
    public int nice_code;
    public int hold_code;
    public int throw_code;
    public int squid_code;
    public int ground_code;
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
    #endregion

    private Rigidbody rb;
    private bool isOnGround;

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
    }

    #region Event Handler
    private void HandleMove(Vector2 moveInput)
    {
        float speed = Mathf.Sqrt(moveInput.x*moveInput.x + moveInput.y*moveInput.y);
        animator.SetFloat(speed_code, speed);
        animator.SetFloat(x_speed_code, moveInput.x);
        animator.SetFloat(y_speed_code, moveInput.y);        
    }
    private void HandleFire(bool fire)
    {
        animator.SetBool("fire_bool", fire);
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
    #endregion

    void Update()
    {
        if(!isOnGround){
            animator.SetFloat(z_speed_code, rb.velocity.z);
        }
    }
}
