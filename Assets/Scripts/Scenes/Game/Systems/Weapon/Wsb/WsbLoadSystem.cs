using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WsbLoadSystem : MonoBehaviour
{
    [Header("事件")]
    [SerializeField] private BoolEventChannel syncHoldEvent;
    GameObject firstChildObject;
    GameObject wsbPrefab;
    public void OnEnable()
    {
        syncHoldEvent.OnEventRaised += HandleThrow;
    }
    public void OnDisable()
    {
        syncHoldEvent.OnEventRaised -= HandleThrow;
    }

    #region EventHandles
    private void HandleThrow(bool throwInput)
    {
        Debug.Log("throwInput: " + throwInput);
        if (throwInput)
        {
            WsbLoad();
        }
    }
    #endregion

    private void WsbLoad()
    {
        Transform weapon = transform.Find("Weapon_R");
        if (weapon.childCount > 0)
        {
            Transform firstChild = weapon.GetChild(0);
            firstChildObject = firstChild.gameObject;
            Debug.Log("Tag: " + firstChildObject.tag);
            if (firstChildObject.tag == "shtr" || firstChildObject.tag == "slsh" || firstChildObject.tag == "chrg")
            {
                wsbPrefab = Resources.Load<GameObject>("Weapon/Sub/throw/throw");
                // Debug.Log("object: ", wsbPrefab);
            }
            else if (firstChildObject.tag == "rllr")
            {
                wsbPrefab = Resources.Load<GameObject>("Weapon/Sub/curling/curling");
            }
            else
            {
                wsbPrefab = Resources.Load<GameObject>("Weapon/Sub/burst/burst");
            }
        }
        if (wsbPrefab == null)
        {
            Debug.LogError("预制体加载失败！");
        }
        else
        {
            GameObject child = Instantiate(wsbPrefab);
            ParticleSystem []explosion = GetComponentsInChildren<ParticleSystem>();
            foreach (var par in explosion)
            {
                par.Stop();
                var em = par.emission;
                em.enabled = false;
            }
            child.transform.SetParent(transform);
            child.transform.localPosition = new Vector3(0.06280009f, 0.007999959f, -0.01599985f);
            // child.transform.localScale = new Vector3(4f, 4f, 4f);
            // Debug.Log("child localposition: " + child.transform.localPosition);
            // Debug.Log("child position: " + child.transform.position);
            // Debug.Log("child scale: " + child.transform.localScale);
        }
    }
}
