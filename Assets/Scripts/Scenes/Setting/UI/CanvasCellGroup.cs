using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCellGroup : MonoBehaviour
{
    public CanvasCell lastCell;

    public void HandlerCellClickEvent(CanvasCell cc)
    {
        if (lastCell == cc)
        {
            return;
        }
        if (lastCell != null && lastCell != cc)
        {
            //取消上次选中的
            lastCell.DownMoveEvent();
        }
        cc.UpMoveEvent();
        lastCell = cc;

    }


}
