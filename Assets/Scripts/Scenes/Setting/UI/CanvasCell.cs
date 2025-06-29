using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CanvasCell : MonoBehaviour, IPointerClickHandler
{
    [Header("Cell名称")] public string cellname;
    private Vector2 OriginPos;//原始坐标信息
    private RectTransform rectTransform;
    [Header("弹出高度")] private float moveDistance = 0.10f;
    [Header("动画执行时长")]private float duration = 0.02f;

    public Ease easeType = Ease.OutBack;
    [Header("Cell的控制对象")] public CanvasCellGroup CanvasCellController;
    [Header("选中时高亮背景的Tag")] public string Htagname = "Highlight_bg";
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        OriginPos = rectTransform.anchoredPosition;
        CanvasCellController = GetComponentInParent<CanvasCellGroup>();
        Image targetImage = FindChildImageByTag(Htagname);
        targetImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData evenData)
    {
        CanvasCellController.HandlerCellClickEvent(this);
    }

    public void UpMoveEvent()
    {
        rectTransform.DOAnchorPos(OriginPos + new Vector2(0, moveDistance), duration).SetEase(easeType);
        Image targetImage = FindChildImageByTag(Htagname);
        targetImage.gameObject.SetActive(true);
    }
    public void DownMoveEvent()
    {
        rectTransform.DOAnchorPos(OriginPos, duration).SetEase(Ease.InOutQuad);
        //获取背景的高亮对象
        Image targetImage = FindChildImageByTag(Htagname);
        targetImage.gameObject.SetActive(false);
    }
    private Image FindChildImageByTag( string tag)
    {
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true)) // true 也查找未激活
        {
            if (child.CompareTag(tag))
            {
                return child.GetComponent<Image>();
            }
        }
        return null;
    }
}
