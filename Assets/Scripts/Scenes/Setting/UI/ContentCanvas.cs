using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//各自的Content
public class ContentCanvas : MonoBehaviour
{
    [Header("内容名称")][SerializeField] public string canvasname;
    void Awake()
    {
        string name = gameObject.name;
        string[] parts = name.Split('_');
        canvasname = parts[1];
    }


}
