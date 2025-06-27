using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCellGroup : MonoBehaviour
{
    [Header("底部点击单元集合")] BottomCellTab[] BtnCellList;
    private BottomCellTab lastSelectedBtn = null;

    //当某个Btn被选中时执行响应的动作
    private void HandleCellSelectedEvent(BottomCellTab btn)
    {
        //判断当前是否有其他Cell已经被选中
        if (!is_Selected(btn))
        {
            //上一次选中的和当前再次点击的Btn是同一个
            return;
        }
        //取消lastSelectedBtn的选中事件
        if (lastSelectedBtn != null)
        {
           lastSelectedBtn.DownMoveEvent(); 
        }
        //更新
        lastSelectedBtn = btn;
        //btn的move事件
    }

    private bool is_Selected(BottomCellTab btn)
    {
        if (btn == lastSelectedBtn)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}
