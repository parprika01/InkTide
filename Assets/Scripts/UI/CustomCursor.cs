using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理显示的鼠标样式文件
public class CustomCursor : MonoBehaviour
{
    [Header("自定义鼠标设置")]public Texture2D cursorTexture; // 拖入自定义图片
    public Vector2 hotspot = new Vector2(90,5); // 热点：鼠标点击位置在图片中的坐标
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
