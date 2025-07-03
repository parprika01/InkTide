using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 如果使用TextMeshPro
public class Timer : MonoBehaviour
{
    // 使用TextMeshProUGUI组件
    public TextMeshProUGUI timerText;
    private float elapsedTime;
    private bool isRunning = true; // 控制计时器是否运行
    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    private void UpdateTimerDisplay()
    {
        // 将时间转换为分钟、秒和毫秒
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);
        // 格式化字符串，例如：00:00:000
        // timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        
        // 如果只需要显示到秒，可以这样：
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // 可以添加方法来控制计时器的开始、暂停和重置
    public void PauseTimer()
    {
        isRunning = false;
    }
    public void StartTimer()
    {
        isRunning = true;
    }
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay();
    }
}