using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHumanAnimSystem : MonoBehaviour
{
    [SerializeField] private AsyncEventChannel<bool> squidEvent;
    private int squid_code;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        squid_code = Animator.StringToHash("squid");
    }

    void OnEnable()
    {
        squidEvent.OnEventRaised += HandleSquidEvent;
    }

    void OnDisable()
    {
        squidEvent.OnEventRaised -= HandleSquidEvent;
    }

    void HandleSquidEvent(InputType name, bool value)
    {
        animator.SetBool(squid_code, value);
    }

}
