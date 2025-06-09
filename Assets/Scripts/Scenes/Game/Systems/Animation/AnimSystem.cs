using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;
using UnityEditor;

public class AnimSystem : MonoBehaviour
{
    public Animator animator;
    public VariableManager variableManager;
    public string systemName = "animation";
    [SerializeField] private float acceleration = 8f;  // 加速系数
    [SerializeField] private float deceleration = 12f; // 减速系数   
    [SerializeField] private float maxSpeed = 5f;

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
    // 同步事件
    [SerializeField] private Vector2EventChannel moveEvent;
    [SerializeField] private BoolEventChannel groundEvent;
    [SerializeField] private EventChannel jumpEvent;
    [SerializeField] private EventChannel ToSquidAnimDoneEvent;   
    // 异步事件
    [SerializeField] private AsyncEventChannel<bool> fireEvent;
    [SerializeField] private AsyncEventChannel fireTriggerEvent;
    [SerializeField] private AsyncEventChannel niceEvent;
    [SerializeField] private AsyncEventChannel comeOnEvent;
    [SerializeField] private AsyncEventChannel<bool> holdEvent;

    [SerializeField] private AsyncEventChannel<bool> squidEvent;
    [SerializeField] private AsyncEventChannel<bool> specialEvent;

    #endregion

    private Rigidbody rb;
    private bool isOnGround;
    private float currentSpeed;
    private float currentXSpeed;
    private float currentYSpeed;
    private Vector2 lastMoveInput;
    private Dictionary<InputType, string> stateDict = new Dictionary<InputType, string>();
    private Dictionary<InputType, Variable> varDict = new Dictionary<InputType, Variable>();
    private CancellationTokenSource cts = new CancellationTokenSource();
    private Dictionary<InputType, UniTaskCompletionSource> tcsDict = new Dictionary<InputType, UniTaskCompletionSource>();    
    private Dictionary<InputType, AsyncEventChannel> triggerEventDict = new Dictionary<InputType, AsyncEventChannel>();
    private Dictionary<InputType, AsyncEventChannel<bool>> boolEventDict = new Dictionary<InputType, AsyncEventChannel<bool>>();

    void Awake()
    {
        animator = GetComponent<Animator>();
        variableManager = new VariableManager(animator);
        rb = GetComponentInParent<Rigidbody>();
        //rb = GetComponent<Rigidbody>();

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
        varDict[InputType.Fire] = new Variable(2, false, fire_bool_code);
        variableManager.AddVariable(varDict[InputType.Fire]);

        varDict[InputType.FireTrigger] = new Variable(2, true, fire_trigger_code, false);
        variableManager.AddVariable(varDict[InputType.FireTrigger]);

        varDict[InputType.Nice] = new Variable(3, true, nice_code);
        variableManager.AddVariable(varDict[InputType.Nice]);

        varDict[InputType.ComeOn] = new Variable(3, true, come_on_code);
        variableManager.AddVariable(varDict[InputType.ComeOn]);

        varDict[InputType.Hold] = new Variable(2, false, hold_code);
        variableManager.AddVariable(varDict[InputType.Hold]);

        varDict[InputType.Squid] = new Variable(2, false, squid_code);
        variableManager.AddVariable(varDict[InputType.Squid]);
        #endregion
        #region async event initialize
        boolEventDict[InputType.Fire] = fireEvent;
        boolEventDict[InputType.Hold] = holdEvent;
        boolEventDict[InputType.Squid] = squidEvent;
        boolEventDict[InputType.Special] = specialEvent;

        triggerEventDict[InputType.FireTrigger] = fireTriggerEvent;
        triggerEventDict[InputType.Nice] = niceEvent;
        triggerEventDict[InputType.ComeOn] = comeOnEvent;
        #endregion
        #region animation state initialize
        stateDict[InputType.FireTrigger] = "Attack";
        stateDict[InputType.Nice] = "Nice";
        stateDict[InputType.ComeOn] = "ComeOn";
        stateDict[InputType.Squid] = "ToSquid";
        #endregion

    }
    void OnEnable()
    {
        // 同步事件
        moveEvent.OnEventRaised += HandleMove;
        groundEvent.OnEventRaised += HandleGround;
        jumpEvent.OnEventRaised += HandleJump;

        // 异步事件
        foreach (var pair in boolEventDict)
        {
            pair.Value.OnEventRaised += ctsClear;
            pair.Value.OnEventRaised += HandleBoolEvent;
        }

        foreach (var pair in triggerEventDict)
        {
            pair.Value.OnEventRaised += ctsClear;
            pair.Value.OnEventRaised += HandleTriggerEvent;
        }

        // 特殊事件
        squidEvent.OnEventRaised += HandleSquidEvent;
    }
    void OnDisable()
    {
        // 同步事件
        moveEvent.OnEventRaised -= HandleMove;
        groundEvent.OnEventRaised -= HandleGround;
        // 异步事件
        foreach (var pair in boolEventDict)
        {
            pair.Value.OnEventRaised -= ctsClear;
            pair.Value.OnEventRaised -= HandleBoolEvent;
        }

        foreach (var pair in triggerEventDict)
        {
            pair.Value.OnEventRaised -= ctsClear;
            pair.Value.OnEventRaised -= HandleTriggerEvent;
        }

        // 特殊事件
        squidEvent.OnEventRaised -= HandleSquidEvent;        
    }

    #region Event Handler
    private void HandleMove(Vector2 moveInput)
    {
        lastMoveInput = moveInput;       
    }
    private void HandleGround(bool ground)
    {
        isOnGround = ground;
        animator.SetBool(ground_code, ground);
    }  
    private void HandleJump(){
        animator.SetTrigger(jump_code);
    }
    private async void HandleBoolEvent(InputType name, bool value){
        
        if (value) {
            // 变量激活
            variableManager.ActivateVar(varDict[name]);
            // 异步等待bool变为false
            tcsDict[name] = new UniTaskCompletionSource();
            try
            {
                await tcsDict[name].Task.AttachExternalCancellation(cts.Token);
                //boolEventDict[name].Ready(systemName);  
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                // 通知事件通道动画系统已处理完成
            }
        } else {
            variableManager.DeactivateVar(varDict[name]);
            tcsDict[name].TrySetResult();
            await UniTask.Delay(100);
            boolEventDict[name].Ready(systemName);
        }
    }
    private async void HandleTriggerEvent(InputType name)
    {
        try {
            // 变量激活
            variableManager.ActivateVar(varDict[name]);
            // 异步等待动画播放完毕
            await UniTask.WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(1).IsName(stateDict[name]) &&
                animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.99f &&
                !animator.IsInTransition(1),
                cancellationToken: cts.Token
            );   

            //Debug.Log("动画播放完成：" + name);
        } catch(OperationCanceledException) {
        } finally {
            // 通知事件通道动画系统已处理完成
            triggerEventDict[name].Ready(systemName);
        }
        
    }   
    private void ctsClear(InputType name, bool value)
    {
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();
    }    
    private void ctsClear(InputType name)
    {
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();
    }

    private async void HandleSquidEvent(InputType name, bool value)
    {   
        if(!value) return;
        await UniTask.WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(2).IsName(stateDict[InputType.Squid]) &&
            animator.GetCurrentAnimatorStateInfo(2).normalizedTime >= 0.99f &&
            !animator.IsInTransition(2)
        ); 
        ToSquidAnimDoneEvent.Raise();       
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

    void MovementSetting()
    {
        currentSpeed = rb.velocity.magnitude;
        currentXSpeed = rb.velocity.x;
        currentYSpeed = rb.velocity.y;
        animator.SetFloat(speed_code, currentSpeed / maxSpeed);
        animator.SetFloat(x_speed_code, currentXSpeed / maxSpeed);
        animator.SetFloat(y_speed_code, currentYSpeed / maxSpeed);
    }
    
    void Update()
    {
        MovementSetting();
        if(!isOnGround){
            animator.SetFloat(z_speed_code, rb.velocity.z);
        }
    }
}
