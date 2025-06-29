using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCellGroup : MonoBehaviour
{
    // [Header("底部点击单元集合")] BottomCellTab[] BtnCellList;
    private BottomCellTab lastSelectedBtn = null;
    public CanvasController canvasController;

    //当某个Btn被选中时执行响应的动作
    public void HandleCellSelectedEvent(BottomCellTab btn)
    {
        //判断当前是否有其他Cell已经被选中
        if (!is_Selected(btn))
        {
            //上一次选中的和当前再次点击的Btn是同一个
            return;
        }
        //取消lastSelectedBtn的选中事件
        ContentCanvas last;
        if (lastSelectedBtn != null)
        {
            lastSelectedBtn.DownMoveEvent();
            last = canvasController.GetCanvasByName(lastSelectedBtn.canvasname);
        }
        else
        {
            last = null;
        }
        
        //btn的move事件
        btn.UpMoveEvent();
        //Canvas事件
        
        ContentCanvas cur = canvasController.GetCanvasByName(btn.canvasname);
        canvasController.SwitchToHandlerEvent(last, cur);
        //更新
        lastSelectedBtn = btn;

    }
    private void is_Selected(BottomCellGroup btn){
        if(lastSelectedBtn == btn){
            return false;
        }else{
            return true;
        }
    }
}
