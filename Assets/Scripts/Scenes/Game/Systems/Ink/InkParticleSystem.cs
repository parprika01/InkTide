using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkParticleSystem : MonoBehaviour
{
    [Header("event")]
    [SerializeField] private BoolEventChannel fireEvent;
    private ParticleSystem particleSystem;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        var em = particleSystem.emission;
        em.enabled = false;        
    }

    void OnEnable()
    {
        fireEvent.OnEventRaised += HandleFireEvent;
    }

    void OnDisable()
    {
        fireEvent.OnEventRaised -= HandleFireEvent;
    }
    
    void HandleFireEvent(bool value)
    {
        var em = particleSystem.emission;
        em.enabled = value;
    }
}
