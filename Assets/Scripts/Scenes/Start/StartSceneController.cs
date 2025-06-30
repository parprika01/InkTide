using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StartSceneController : MonoBehaviour
{
    [Header("satrt Btn")]
    public Button SLbtn;//pageindex = 0;
    public Button SRbtn;//pageindex = 1;

    [Header("切换动画")]
    public ScreenTransitionController screencontroller;
    public PageScroller ps;

    private int pageindex = 0;
    void Start()
    {
        screencontroller = gameObject.GetComponent<ScreenTransitionController>();
        if (screencontroller == null)
        {
            Debug.Log("获取ScreenTransitionController组件失败");
        }
        SLbtn.onClick.AddListener(() => SwitchPageTo(SLbtn.transform,0));
        SRbtn.onClick.AddListener(() => SwitchPageTo(SRbtn.transform,1));
        AnimateButtonBounce(SLbtn.transform);
        AnimateButtonBounce(SRbtn.transform);
    }

    private void SwitchPageTo(Transform btnTransform,int index)
    {
        btnTransform.DOKill(true); // 停止并恢复原始状态
        screencontroller.SwitchPage(index);
        if (index == 1)
        {
            ps.currentPage = index;
        }
    }
    void AnimateButtonBounce(Transform btnTransform)
    {
        btnTransform.DOScale(1.1f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
