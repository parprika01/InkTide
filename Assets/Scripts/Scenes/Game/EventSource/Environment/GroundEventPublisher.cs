using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GroundEventPublisher : MonoBehaviour
{
    [Header("事件通道")]
    [SerializeField] private EventChannel<bool> groundEvent;
    [SerializeField] private EventChannel<bool> slopeEvent;
    public CapsuleCollider capcol;
    public Rigidbody rb;
    private Vector3 pointTop, pointBottom;
    private Collider[] colliders;    
    private float radius;
    private bool isOnGround;
    private bool previousIsOnGround;
    private bool isOnSlope;
    private bool previousIsOnSlope;


    void Awake()
    {
        previousIsOnSlope = isOnGround = true;
        previousIsOnGround = isOnSlope = false;
        capcol = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        radius = capcol.radius;
    }

    void Update()
    {
        isOnGround = onGround();
        if(isOnGround != previousIsOnGround)
        {
            Debug.Log("groudEvent: " + isOnGround);
            groundEvent.Raise(isOnGround);
            previousIsOnGround = isOnGround;
        }

        isOnSlope = onSlope();
        if(isOnSlope != previousIsOnSlope)
        {
            slopeEvent.Raise(isOnSlope);
            previousIsOnSlope = isOnSlope;
        }
    }

    // 待修改
    private bool onGround()
    {
        pointBottom = transform.position + transform.up * radius;
        pointTop = transform.position + transform.up * capcol.height - transform.up * radius;
        LayerMask ignoreMask = ~(1 << 8);
 
        colliders = Physics.OverlapCapsule(pointBottom, pointTop, radius, ignoreMask);
        Debug.DrawLine(pointBottom, pointTop,Color.blue);
        if (colliders.Length!=0)
        {
            return true;
        }
        else
        {
            return false;
        }        
    }

    private bool onSlope()
    {
        float slopeHeightMaxDistance = 2f;
        float heightOffset = 2f;

        RaycastHit hit;
        if (Physics.Raycast(rb.position + Vector3.up * heightOffset, Vector3.down, out hit, slopeHeightMaxDistance))
        {
            return hit.normal != Vector3.up;
        }

        return false;
    }
}
