using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrackerSystem : MonoBehaviour
{
    [Header("手部切线追踪")]
    [SerializeField] private float tangentSmoothness = 0.2f; // 平滑处理
    
    private Vector3 lastPosition;
    private Vector3 currentTangent;
    private Vector3 smoothedTangent;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // 计算原始切线方向（手部当前帧移动方向）
        currentTangent = (transform.position - lastPosition).normalized;
        
        // 平滑处理
        smoothedTangent = Vector3.Lerp(smoothedTangent, currentTangent, tangentSmoothness * Time.deltaTime * 60f);
        
        lastPosition = transform.position;
    }

    // 获取当前平滑后的切线方向
    public Vector3 GetHandTangent()
    {
        return smoothedTangent;
    }

    // 在副武器脱手时调用，获取瞬时切线
    public Vector3 GetInstantHandTangent()
    {
        return currentTangent;
    }
}
