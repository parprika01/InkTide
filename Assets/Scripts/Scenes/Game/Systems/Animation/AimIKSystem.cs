using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class AimIKSystem : MonoBehaviour
{
    [Header("event")]
    [SerializeField] private BoolEventChannel fireEvent;
    [Header("aim")]
    [SerializeField] private GameObject aimObject;
    [SerializeField] private Transform aimPoint;
    private AimIK aimIK;
    private Camera cam;
    private bool isFire;
    private void Awake()
    {
        cam = Camera.main;
        aimIK = GetComponent<AimIK>();
    }
    
    private void OnEnable()
    {
        fireEvent.OnEventRaised += HandleFireEvent;
    }

    private void OnDisable()
    {
        fireEvent.OnEventRaised -= HandleFireEvent;
    }

    private void Update()
    {
        if(isFire) UpdateAim();
    }

    private void HandleFireEvent(bool value)
    {
        isFire = value;
        aimIK.enabled = value;
    }

    private void UpdateAim()
    {
        var forward = cam.transform.forward;
        aimObject.transform.position = aimPoint.transform.position + forward * 5f;
    }
}
