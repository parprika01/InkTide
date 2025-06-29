using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCellTab : MonoBehaviour
{
    // Start is called before the first frame update
<<<<<<< Updated upstream
    void Start()
    {
        
=======
    private Vector2 OriginPos;//原始坐标信息
    private RectTransform rectTransform;
    [Header("弹出高度")][SerializeField] private float moveDistance = 0.2f;
    [Header("动画执行时长")][SerializeField] private float duration = 0.02f;

    public Ease easeType = Ease.OutBack;
    [Header("Cell的控制对象")]public BottomCellGroup BtnController;
    [Header("控制面板的名称")] public string canvasname;
    

    private void Awake()
    {
        //获取整个的移动组件
        rectTransform = GetComponent<RectTransform>();
        OriginPos = rectTransform.anchoredPosition;
        Debug.Log(OriginPos);
        //提取控制内容名称
        string name = gameObject.name;
        string[] parts = name.Split('_');
        canvasname = parts[1];

    }
    public void OnPointerClick(PointerEventData evenData)
    {
        BtnController.HandleCellSelectedEvent(this);
    }
    public void UpMoveEvent()
    {
        rectTransform.DOAnchorPos(OriginPos + new Vector2(0, moveDistance), duration).SetEase(easeType);
    }
    public void DownMoveEvent()
    {
        rectTransform.DOAnchorPos(OriginPos, duration).SetEase(Ease.InOutQuad);
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
