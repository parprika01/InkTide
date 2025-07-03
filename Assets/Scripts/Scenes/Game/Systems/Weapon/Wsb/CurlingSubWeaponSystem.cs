using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;

public class CurlingSubWeaponSystem : MonoBehaviour
{
    [Header("事件")]
    [SerializeField] private BoolEventChannel syncHoldEvent;
    [Header("参数")]
    [SerializeField] private float notChargeSpeed = 15f;
    [SerializeField] private float chargeSpeed = 8f; 
    [SerializeField] private int notChargeDelay = 4;
    [SerializeField] private int chargeDelay = 2;
    [SerializeField] private float maxHoldTime = 0.5f;
    private Camera cam;
    private float holdTime;
    private bool isCharge;
    private bool isThrow;
    private bool hasExplosion;
    private Animator animator;
    private Rigidbody rb;
    private ParticleSystem[] explosion;

    private void Awake()
    {
        isThrow = true;
        hasExplosion = false;
        cam = Camera.main;
        holdTime = 0;
        animator = transform.gameObject.GetComponent<Animator>();
        explosion = GetComponentsInChildren<ParticleSystem>();
        animator.SetBool("hold", true);
    }
    private void FixedUpdate()
    {
        if (isThrow)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= maxHoldTime)
            {
                isCharge = true;
            }
            else
            {
                isCharge = false;
            }
        }
    }
    public void OnEnable()
    {
        syncHoldEvent.OnEventRaised += HandleThrow;
    }
    public void OnDisable()
    {
        syncHoldEvent.OnEventRaised -= HandleThrow;
    }

    #region EventHandles
    private void HandleThrow(bool throwInput)
    {
        isThrow = throwInput;
        if (throwInput == false)
        {
            Throw();
        }
    }
    #endregion

    async Task OnCollisionEnter(Collision collision)
    {
        if (isThrow == false && hasExplosion == false)
        {
            hasExplosion = true;
            var em = explosion[4].emission;
            em.enabled = true;
            explosion[4].Play();
            int delayTime;
            if (isCharge)
            {
                delayTime = chargeDelay * 1000;
            }
            else
            {
                delayTime = notChargeDelay * 1000;
            }
            await UniTask.Delay(delayTime);
            Debug.Log("开始爆炸");
            Bomb();
            Debug.Log("爆炸后的isThrow: " + isThrow);
        }
    }
    private void Throw()
    {
        Debug.Log("在手上的时间: " + holdTime);
        transform.gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        rb.mass = 0.5f;
        Vector3 throwDirection = cam.transform.forward; // 获取投掷方向
        throwDirection.y = 0;
        transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
        Vector3 forceTest;
        float ttt;
        if (isCharge)
        {
            forceTest = chargeSpeed * throwDirection;
            ttt = chargeSpeed;
        }
        else
        {
            forceTest = notChargeSpeed * throwDirection;
            ttt = notChargeSpeed;
        }
        transform.eulerAngles = new Vector3(0, 0, 0);
        rb.freezeRotation = true;
        rb.AddForce(forceTest, ForceMode.Impulse);
        animator.SetBool("hold", false);
        Debug.Log("投掷方向: " + throwDirection);
        Debug.Log("作用在curling上的力: " + forceTest);
        Debug.Log("施加的速度: " + ttt);
    }
    private async void Bomb()
    {
        animator.SetTrigger("bomb");
        foreach (var par in explosion)
        {
            par.Play();
            var em = par.emission;
            em.enabled = true;
        }
        await UniTask.Delay(800);
        Destroy(transform.gameObject);
    }
}
