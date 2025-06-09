using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintObj : MonoBehaviour
{
    Renderer rend;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    
    void Awake()
    {
        rend = gameObject.GetComponent<Renderer>();
        if(rend != null){
            PaintDataPool.Instance.AddRenderer(rend);
        }
    }

    void Start(){
        rend.material.SetTexture("_MaskTex", InkManager.Instance.inkRT);
        rend.material.SetVector("_LightmapST",rend.lightmapScaleOffset);
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("particle collision");
        ParticleSystem ps = other.GetComponent<ParticleSystem>();
        if (ps == null) return;

        int numCollisonEvents = ps.GetCollisionEvents(gameObject, collisionEvents);

        for(int i = 0; i < numCollisonEvents; i++){
            RaycastHit hitInfo;
            Vector3 rawPos = collisionEvents[i].intersection;
            Vector3 normal = collisionEvents[i].normal;
            Vector3 hitNormal = collisionEvents[i].normal; // 碰撞表面法线
            Vector3 particleVelocity = collisionEvents[i].velocity; // 粒子速度方向
            float impactAngle = 0;
            float impactDirection = CalculateangleBias(normal,particleVelocity);
            if (particleVelocity.magnitude > 0.001f) { // 确保速度有效
                Vector3 particleDir = particleVelocity.normalized;
            
                // 计算角度（0°~90°，0°=垂直撞击，90°=擦过）
                impactAngle = Vector3.Angle(-particleDir, hitNormal);
            }
            
            // 用一个很短的Ray从上往下检测
            if (Physics.Raycast(rawPos + normal * 0.1f, -normal, out hitInfo, 1f)) {
                Vector3 correctedHit = hitInfo.point;
                Vector4 pos = new Vector4(correctedHit.x, correctedHit.y, correctedHit.z, 1);
                Vector4 color = new Vector4(1, 0, 0, 1);
                PaintDataPool.Instance.AddCommands(impactAngle, pos, color, 2.5f, hitInfo.normal, impactDirection);
            }
        }
    }

    float CalculateangleBias(Vector3 normal, Vector3 velocity)
    {
        float angleToRight, angleToUp;
        normal = normal.normalized;
        // 选择一个全局参考向量，避免与 normal 共线
        Vector3 worldUp = Mathf.Abs(normal.y) < 0.99f ? Vector3.up : Vector3.right;
        // 计算平面 X 轴（right）
        Vector3 right = Vector3.Cross(worldUp, normal).normalized;
        // 计算平面 Y 轴（up）
        Vector3 up = Vector3.Cross(normal, right);
        Vector3 v_proj = velocity - Vector3.Dot(velocity, normal) * normal;
        if (v_proj.sqrMagnitude < 1e-6f)
        {
            angleToRight = angleToUp = 0f;
            return 0;
        }
        
        Vector3 vp_n = v_proj.normalized;
        float signedAngle = Vector3.SignedAngle(vp_n, right, normal);
        Debug.Log("signedAngle:" + signedAngle);

        // 2. 用点积和 arccos 求夹角，转为角度
        angleToRight = Mathf.Acos(Mathf.Clamp(Vector3.Dot(vp_n, right), -1f, 1f))
                       * Mathf.Rad2Deg;
        angleToUp    = Mathf.Acos(Mathf.Clamp(Vector3.Dot(vp_n, up), -1f, 1f))
                       * Mathf.Rad2Deg;
        //Debug.Log("angleToUp: " + angleToUp + " angleToRight" + angleToRight);
        return -signedAngle;
    }
}
