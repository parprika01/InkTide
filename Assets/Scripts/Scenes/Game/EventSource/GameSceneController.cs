using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [Header("event")]
    [SerializeField] private EventChannel InitializeEvent;

    void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        InitializeEvent.Raise();
    }
}
