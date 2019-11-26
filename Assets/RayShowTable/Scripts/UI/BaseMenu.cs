using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenu : MonoBehaviour {
    
    public Sprite[] itemImages = null;
    public System.Action<int> MenuSelectCallback = delegate
    {

    };

    public virtual int currentItemIndex
    {
        get { return 0; }
        protected set { }
    }

    //创建MenuItem
    public virtual void CreateMenuItems(bool reCreate = false , int initItemIndex = 0)
    {
        
    }
    //销毁MenuItem
    public virtual void DestroyMenuItems() {

    }

    //传入的是marker的旋转角度变化
    public virtual void RotateSelectMenu(float deltaAngle)
    {

    }

    public virtual void SelectItem(int index,bool invokeCallback = true)
    {

    }
}
