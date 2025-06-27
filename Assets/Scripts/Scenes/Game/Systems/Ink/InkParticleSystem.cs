using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkParticleSystem : MonoBehaviour
{
    [Header("event")]
    [SerializeField] private BoolEventChannel fireEvent;
    private ParticleSystem[] particleSystem;

    void Awake()
    {
        particleSystem = GetComponentsInChildren<ParticleSystem>();
        foreach (var par in particleSystem)
        {
            Debug.Log("Particle: " + par);
            par.Play();
            var em = par.emission;
            em.enabled = false;
        }
             
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
        Debug.Log("当前fire: " + value);
        foreach (var par in particleSystem)
        {
            var em = par.emission;
            em.enabled = value;
        }
    }
}
