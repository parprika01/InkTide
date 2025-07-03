using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
//控制墨水余量和大招的使用
public class RemainController : MonoBehaviour
{
    public TextMeshProUGUI cur_number;//显示数字的Text对象
    private int last_number;
    public LastCell[] lastcells;
    private int index = 3;//开始默认全满

    // Start is called before the first frame update
    void Start()
    {
        cur_number.text = "0000p";
        last_number = 100;

    }
    //更新地图上的墨水面积
    public void SetCurAllNumber(string all)
    {
        cur_number.text = all + "p";
    }
    //墨水余量值设置接口
    public void SetRemainText(string number)
    {
        int num = Convert.ToInt32(number);
        last_number = num;
        int r_index = num / 100;
        if (r_index != index)
        {
            int i = 0;
            foreach (var cell in lastcells)
            {
                if (i > r_index)
                {
                    cell.UnableCell();
                }
                cell.ShowCell();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO
        //更新last_number.text
    }
}
