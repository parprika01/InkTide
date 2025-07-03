using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PageScroller : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Button leftButton;
    public Button rightButton;

    public int totalPages = 2;
    [Header("初始页面")]
    public int currentPage = 0;

    private float[] pagePositions;

    public float duration = 0.5f;
    public Ease easeType = Ease.OutBack; // 弹动效果

    public Image lh_icon;
    public Image rh_icon;
    public TMP_InputField l_id_inputField;
    public TMP_InputField r_id_inputField;
    public TMP_InputField l_pwd_inputField;
    public TMP_InputField r_pwd_inputField;
    void Start()
    {
        // 初始化页码对应的锚点位置（0 到 1）
        pagePositions = new float[totalPages];
        for (int i = 0; i < totalPages; i++)
        {
            pagePositions[i] = (float)i / (totalPages - 1);
        }

        // 设置初始页面（不带动画）
        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);
        scrollRect.horizontalNormalizedPosition = pagePositions[currentPage];

        leftButton.onClick.AddListener(() => MoveToPage(currentPage - 1));
        rightButton.onClick.AddListener(() => MoveToPage(currentPage + 1));

        UpdateButtons();
    }


    public void MoveToPage(int pageIndex)
    {
        lh_icon.gameObject.SetActive(false);
        rh_icon.gameObject.SetActive(false);
        //输入框清空
        CleanInput();
        pageIndex = Mathf.Clamp(pageIndex, 0, totalPages - 1);
        currentPage = pageIndex;

        // 停止之前动画，防止叠加
        scrollRect.DOKill();

        // 平滑滚动带弹性动画
        scrollRect.DOHorizontalNormalizedPos(pagePositions[pageIndex], duration).SetEase(easeType);

        UpdateButtons();
    }
    void CleanInput()
    {
        l_id_inputField.text = "";
        l_pwd_inputField.text = "";
        r_pwd_inputField.text = "";
        r_id_inputField.text = "";
    }

    void UpdateButtons()
    {
        leftButton.interactable = currentPage > 0;
        rightButton.interactable = currentPage < totalPages - 1;
    }

}
