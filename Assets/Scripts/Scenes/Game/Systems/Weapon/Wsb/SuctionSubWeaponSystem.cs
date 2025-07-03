using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SuctionSubWeaponSystem : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] private float throwSpeed = 10f;
    [Header("事件")]
    [SerializeField] private BoolEventChannel syncHoldEvent;
    [Header("基本设置")]
    [SerializeField] public Transform suctionPoint;  // 吸盘底部位置
    // [SerializeField] public LayerMask suctionLayers; // 可吸附的层

    private bool isAttached = false;
    private Rigidbody rb;
    private bool isThrow;
    private bool hasExplosion;
    private ParticleSystem[] explosion;
    private Camera cam;
    private Animator animator;

    private void Awake()
    {
        cam = Camera.main;
        explosion = GetComponentsInChildren<ParticleSystem>();
        hasExplosion = false;
        animator = GetComponent<Animator>();
        animator.SetBool("hold", true);
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
        animator.SetBool("hold", throwInput);
        Debug.Log("throw: " + isThrow);
        if (throwInput == false)
        {
            Throw();
        }
    }
    #endregion

    async Task OnCollisionEnter(Collision collision)
    {
        if (isAttached) return;
        ContactPoint contact = collision.contacts[0];
        Attach(contact.point, contact.normal); // 固定炸弹

        // // 检查是否是可吸附表面
        // if (((1 << collision.gameObject.layer) & suctionLayers) != 0)
        // {
        //     // 获取最近的接触点
        //     ContactPoint contact = collision.contacts[0];

        //     // 固定炸弹
        //     Attach(contact.point, contact.normal);
        // }
        if (isThrow == false && hasExplosion == false)
        {
            hasExplosion = true;
            await UniTask.Delay(2000);
            Debug.Log("开始爆炸");
            Bomb();
            Debug.Log("爆炸后的isThrow: " + isThrow);
        }
    }

    void Attach(Vector3 point, Vector3 normal)
    {
        animator.SetBool("stick", true);
        isAttached = true;
        rb.isKinematic = true;
        // 使炸弹的向上方向与表面法线对齐
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, normal), normal);
        // 调整位置使吸盘正好接触表面
        Vector3 positionOffset = point - suctionPoint.position;
        transform.position += positionOffset;
        transform.rotation = targetRotation;
    }
    private void Throw()
    {
        transform.gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        rb.mass = 0.5f;
        Vector3 throwDirection = cam.transform.forward; // 获取投掷方向
        Debug.Log("投掷方向: " + throwDirection);
        transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(throwDirection * throwSpeed, ForceMode.Impulse);
    }
    private async void Bomb()
    {
        foreach (var par in explosion)
        {
            par.Play();
            var em = par.emission;
            em.enabled = true;
        }
        await UniTask.Delay(500);
        Destroy(transform.gameObject);
    }
}
