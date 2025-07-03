using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MatchSceneController : MonoBehaviour
{
    //显示内容的标签
    public TextMeshProUGUI l_nameText;
    public TextMeshProUGUI r_nameText;
    void Start()
    {
        l_nameText.text = "Self";//传入自己的name信息
        r_nameText.text = "Enemy";//传入匹配对象的姓名信息
        // 假设此处模拟匹配成功（你可以换成收到服务器返回匹配成功的时机）
        StartCoroutine(StartGameAfterDelay(3f));
    }
    /// <summary>
    /// 匹配成功后，等待 delay 秒后进入游戏场景
    /// </summary>
    IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneDirector.Instance.LoadScene("Game"); // 替换为你的目标场景名
    }
}
