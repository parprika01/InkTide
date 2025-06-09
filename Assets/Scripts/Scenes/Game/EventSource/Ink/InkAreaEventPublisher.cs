using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkAreaEventPublisher : MonoBehaviour
{
    [Header("event")]
    [SerializeField] private BoolEventChannel inkAreaEvent;
    
    private bool _isInInkArea;
    private bool _previousIsInInkArea;

    void Awake()
    {
        _isInInkArea = _previousIsInInkArea = false;
    }

    void Update()
    {
        _isInInkArea = InkManager.Instance.IsInInkArea(transform.position);
        Debug.Log("当前是否位于墨水中：" + _isInInkArea);
        if (_isInInkArea != _previousIsInInkArea)
        {
            Debug.Log("当前是否在墨水中：" + _isInInkArea);
            _previousIsInInkArea = _isInInkArea;
            inkAreaEvent.Raise(_isInInkArea);
        }
    }
}
