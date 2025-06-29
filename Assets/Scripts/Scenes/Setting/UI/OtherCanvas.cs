using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCanvas : MonoBehaviour
{
    public string canvasname;
    void Awake()
    {
        string name = gameObject.name;
        string[] parts = name.Split('_');
        canvasname = parts[1];
    }
}
