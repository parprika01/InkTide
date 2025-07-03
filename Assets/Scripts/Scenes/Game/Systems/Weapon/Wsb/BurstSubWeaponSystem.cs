using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class BurstSubWeaponSystem : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private float throwSpeed = 15f;
    [SerializeField] private float spinSpeed = 0.2f;
    [Header("事件")]
    [SerializeField] private BoolEventChannel syncHoldEvent;

    private Rigidbody rb;
    private bool isThrow;
    private bool hasExplosion;
    private ParticleSystem[] explosion;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
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
            Debug.Log("前");
            Throw();
            Debug.Log("后");
        }
    }
    #endregion

    void OnCollisionEnter(Collision collision)
    {
        if (isThrow == false && hasExplosion == false)
        {
            hasExplosion = true;
            Debug.Log("开始爆炸");
            Bomb();
            Debug.Log("爆炸后的isThrow: " + isThrow);
        }
    }

    private void Throw()
    {
        transform.gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        Vector3 throwDirection = cam.transform.forward; // 获取投掷方向
        Debug.Log("投掷方向: " + throwDirection);
        transform.SetParent(null);
        Debug.Log("脱离了上一级物体");
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(throwDirection * throwSpeed, ForceMode.Impulse);
        rb.AddTorque(throwDirection * spinSpeed, ForceMode.Impulse); // 旋转效果
    }

    private async void Bomb()
    {
        foreach (var par in explosion)
        {
            par.Play();
            var em = par.emission;
            em.enabled = true;
            // par.Play();
        }
        await UniTask.Delay(800);
        Destroy(transform.gameObject);
    }
}
