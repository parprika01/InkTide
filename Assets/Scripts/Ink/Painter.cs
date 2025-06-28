using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)){
            Vector3 position = Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(position);
            RaycastHit hit;

            if(Physics.Raycast(ray,out hit, 100f)){
                Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
                Vector4 pos = new Vector4(hit.point.x, hit.point.y, hit.point.z, 1);
                Vector4 color = new Vector4(0.5f, 0, 0, 1);
                float angle = Vector3.Angle(-ray.direction, hit.normal);
                //Debug.Log($"击中角度: {angle}°");
                PaintDataPool.Instance.AddCommands(angle, pos, color, 2.5f, hit.normal, 0);
            }
        }
    }
}
