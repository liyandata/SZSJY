using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//marker ui基类
public abstract class BaseMarkerUI : MonoBehaviour
{
    //ui名称，可用于区分相同样式的不同实例，例如 菜单1，菜单2
    public string uiName = "";
    protected Marker mMarker = null;

    public Marker marker { get { return mMarker; } }

    public virtual void InitMarker(Marker m)
    {
        mMarker = m;
        UpdateUI();
    }

    //根据marker参数更新ui
    public virtual void UpdateUI()
    {
    }
}