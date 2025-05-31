using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
UIManager 支持跨场景工作
一个 全局常驻的 UIManager（挂在 DontDestroyOnLoad 的物体上），用来：
负责公共 UI（如 Loading、网络提示、Toast）
处理场景切换时的 UI 清理与初始化
*/
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Dictionary<string, UIBase> activeUIs = new();
    private GameObject commonUICanvas; // 常驻 UI Canvas

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async void ShowUI(string uiName, bool isCommon = false)
    {
        // 异步加载、挂到合适的 Canvas
    }

    public void CloseAllSceneUIs()
    {
        // 清理场景专属 UI，只保留 common UI
    }
}
