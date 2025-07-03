using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenTransitionController : MonoBehaviour
{
    public RectTransform leftMask;
    public RectTransform rightMask;

    public GameObject page_Start;
    public GameObject page_Startin;

    private bool onPageStart = true;

    public float slideDuration = 0.5f;
    public Ease easeType = Ease.InOutQuad;

    private Vector2 leftOriginalPos;
    private Vector2 rightOriginalPos;

    void Start()
    {
        leftOriginalPos = leftMask.anchoredPosition;
        rightOriginalPos = rightMask.anchoredPosition;
    }


    public void SwitchPage(int index)
    {
        // 1. 两边滑入覆盖
        Sequence seq = DOTween.Sequence();

        float screenWidth = Screen.width;

        seq.Append(leftMask.DOAnchorPosX(-5.3f, slideDuration).SetEase(easeType));
        seq.Join(rightMask.DOAnchorPosX(5.25f, slideDuration).SetEase(easeType));

        // 2. 切换页面（在遮住之后）
        seq.AppendCallback(() =>
        {
            page_Start.SetActive(!onPageStart);
            page_Startin.SetActive(onPageStart);
            onPageStart = !onPageStart;
        });

        // 3. 两边滑出还原
        seq.Append(leftMask.DOAnchorPos(leftOriginalPos, slideDuration).SetEase(easeType));
        seq.Join(rightMask.DOAnchorPos(rightOriginalPos, slideDuration).SetEase(easeType));
    }
}
