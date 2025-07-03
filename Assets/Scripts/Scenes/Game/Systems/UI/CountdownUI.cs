using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
public class CountdownUI : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public Image bg_text;
    public Timer timer;

    public IEnumerator StartCountdown(System.Action onComplete = null)
    {
        yield return CountdownCoroutine(onComplete);
        // CountdownCoroutine(onComplete);
    }

    private IEnumerator CountdownCoroutine(System.Action onComplete)
    {
        string[] countdownValues = { "3", "2", "1", "Go!" };

        foreach (var value in countdownValues)
        {
            countdownText.text = value;
            countdownText.color = new Color(1, 1, 1, 0); // 透明
            // countdownText.transform.localScale = Vector3.zero;

            // 缩放 + 淡入
            countdownText.DOFade(1f, 0.3f);
            countdownText.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.8f);

            // 淡出
            countdownText.DOFade(0f, 0.2f);
            countdownText.transform.DOScale(Vector3.zero, 0.2f);

            yield return new WaitForSeconds(0.2f);
        }

        // 隐藏文字
        countdownText.text = "";
        bg_text.gameObject.SetActive(false);
        onComplete?.Invoke(); // 倒计时完成后回调（可开始游戏）
        timer.StartTimer();
        
    }
}

