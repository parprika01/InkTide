using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ThrowSubWeaponSystem : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private float throwSpeed = 15f;
    [SerializeField] private float spinSpeed = 2f;
    [SerializeField] private float explosionDelay = 1.5f;
    [Header("事件")]
    [SerializeField] private BoolEventChannel syncHoldEvent;

    private GameObject gameObject;
    private Rigidbody rb;
    private bool isThrow;
    private bool hasExplosion;
    private ParticleSystem[] explosion;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        gameObject = transform.gameObject;
        explosion = GetComponentsInChildren<ParticleSystem>();
        hasExplosion = false;
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
        Debug.Log("throw: " + isThrow);
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
            await UniTask.Delay(1000);
            Debug.Log("开始爆炸");
            Bomb();
            Debug.Log("爆炸后的isThrow: " + isThrow);
        }
    }

    private void Throw()
    {
        gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        rb.mass = 0.1f;
        Vector3 throwDirection = cam.transform.forward; // 获取投掷方向
        Debug.Log("投掷方向: " + throwDirection);
        transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(throwDirection * throwSpeed, ForceMode.VelocityChange);
        rb.AddTorque(throwDirection * spinSpeed, ForceMode.VelocityChange); // 旋转效果
    }
    // public bool IsStopped(Rigidbody rby, float velocityThreshold = 0.01f)
    // {
    //     // 检查线速度和角速度
    //     return rby.velocity.magnitude < velocityThreshold &&
    //            rby.angularVelocity.magnitude < velocityThreshold;
    // }
    private async void Bomb()
    {
        // Debug.Log("三角雷粒子系统: " + explosion.Length);
        foreach (var par in explosion)
        {
            par.Play();
            var em = par.emission;
            em.enabled = true;
            // par.Play();
        }
        await UniTask.Delay(200);
        Destroy(gameObject);
    }
}
