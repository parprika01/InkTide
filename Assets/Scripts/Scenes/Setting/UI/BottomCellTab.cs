using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BottomCellTab : MonoBehaviour,IPointerClickHandler
{


    // Start is called before the first frame update
    private Vector2 OriginPos;//原始坐标信息
    private RectTransform rectTransform;
    [Header("弹出高度")][SerializeField] private float moveDistance = 0.2f;
    [Header("动画执行时长")][SerializeField] private float duration = 0.02f;

    public Ease easeType = Ease.OutBack;
    [Header("Cell的控制对象")]public BottomCellGroup BtnController;

    private void Awake()
    {
        //获取整个的移动组件
        rectTransform = GetComponent<RectTransform>();
        OriginPos = rectTransform.anchoredPosition;
        Debug.Log(OriginPos);

    }
    public void OnPointerClick(PointerEventData evenData)
    {
        BtnController.HandleCellSelectedEvent(this);
    }
    public void UpMoveEvent()
    {
        rectTransform.DOAnchorPos(OriginPos + new Vector2(0, moveDistance), duration).SetEase(easeType);
    }
    public void DownMoveEvent()
    {
        rectTransform.DOAnchorPos(OriginPos, duration).SetEase(Ease.InOutQuad);
    }


}
