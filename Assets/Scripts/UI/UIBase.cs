using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public virtual void OnShow() => gameObject.SetActive(true);
    public virtual void OnHide() => gameObject.SetActive(false);
}
