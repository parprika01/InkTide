using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BottomCellTab : MonoBehaviour
{


    // Start is called before the first frame update
    private vector2 OriginPos;//原始坐标信息
    private RectTransform rectTransform;
    [Header("弹出高度")] public float moveDistance = 30f;
    [Header("动画执行时长")] public float duration = 0.3f;
    public Ease easeType = Ease.OutBack;

    private void Awake()
    {
        //获取整个的移动组件
        rectTransform = GetComponent<RectTransform>();
    }


    public void UpMoveEvent()
    {
        
    }
    public void DownMoveEvent()
    {

    }


}
