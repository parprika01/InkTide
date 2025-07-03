using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LastCell : MonoBehaviour
{

    public void ShowCell()
    {
        Color new_color = new Color(223f / 255f, 90f / 255f, 48f / 255f);
        gameObject.GetComponent<Image>().color = new_color;
    }
    public void UnableCell()
    {
        Color new_color = new Color(1f, 1f, 1f);
        gameObject.GetComponent<Image>().color = new_color;
    }
    // Start is called before the first frame update
    void Start()
    {
        ShowCell();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
