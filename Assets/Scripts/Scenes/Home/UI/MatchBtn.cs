using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
[RequireComponent(typeof(Button))]
public class MatchBtn : MonoBehaviour
{
    [Header("user info")]
    public string username;
    public string level;
    [Header("呼吸动画参数")]
    public float scaleMin = 0.95f;
    public float scaleMax = 1.05f;
    public float duration = 1.2f;

    private Button button;
    private Tween breathingTween;
    private bool isBreathing = false;
    void Start()
    {
        GameObject obj = GameObject.FindWithTag("username");
        TextMeshProUGUI te = obj.GetComponent<TextMeshProUGUI>();
        username = te.text;
        button = GetComponent<Button>();

        // 如果按钮没有注册任何点击事件，就启动呼吸动画
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            StartBreathing();
        }
    }
    public void StartBreathing()
    {
        if (isBreathing) return;

        isBreathing = true;
        breathingTween = transform.DOScale(scaleMax, duration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    public void StopBreathing()
    {
        if (!isBreathing) return;

        breathingTween.Kill();
        transform.DOScale(1f, 0.2f); // 恢复原始大小
        isBreathing = false;
    }

}
