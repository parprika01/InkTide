using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//控制左侧内容面板的显示和切换
public class CanvasController : MonoBehaviour
{
    [Header("内容面板列表")] public ContentCanvas[] CCL;
    private ContentCanvas lastedCanvas;//上一次选中的面板
    private Vector2 OnScreenPos = new Vector2(-1.1f, 1.65f);
    private Vector2 OffScreenPos = new Vector2(-5.6f, 1.8f);

    void Awake()
    {
        foreach (var canvas in CCL)
        {
            // canvas.gameObject.SetActive(false);//默认隐藏所有面板
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.anchoredPosition = OnScreenPos; // 设置 UI 坐标
            canvas.gameObject.SetActive(false);

        }
    }
    //页面之间的切换
    public void SwitchToHandlerEvent(ContentCanvas from, ContentCanvas to)
    {
        if (from != null)
        {
            from.gameObject.SetActive(false);
        }
        to.gameObject.SetActive(true);

    }
    //获取对应的Canvas对象
    public ContentCanvas GetCanvasByName(string name)
    {
        foreach (var canvas in CCL)
        {
            if (canvas.canvasname == name)
            {
                return canvas;
            }
        }
        return null;
    }
    

}
