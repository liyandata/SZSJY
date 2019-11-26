using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//环形菜单UI
public class MarkerUI_RadialMenu : BaseMarkerUI
{
    public BaseMenu menu;

    protected float deltaAngle = 0;
    protected float preMarkerAngle = 0;


    void Awake()
    {
        if (menu == null)
        {
            menu = GetComponentInChildren<BaseMenu>();
        }
        if (menu != null)
        {

            menu.CreateMenuItems();
            menu.MenuSelectCallback += OnMenuSelect;
        }
    }

    public override void InitMarker(Marker m)
    {
        mMarker = m;
        preMarkerAngle = m.angle;
        UpdateUI();
    }


    public override void UpdateUI()
    {
        transform.position = marker.position;
        deltaAngle = marker.angle - preMarkerAngle;
        preMarkerAngle = marker.angle;
        if (Mathf.Abs(deltaAngle) > 300f)
        {
            //处理0和360临界的情况
            deltaAngle += (deltaAngle > 0f ? -360f : 360f);
        }
        if (menu)
        {
            menu.RotateSelectMenu(deltaAngle);
        }
    }


    //菜单项选中的触发
    protected virtual void OnMenuSelect(int menuIndex)
    {
        Debug.Log("Radial Menu : OnMenuSelect = " + menuIndex);
        Network_SendMenuData();
    }

    protected void Network_SendMenuData()
    {
        if (UdpClient.IsExisted == false ||
            UdpClient.Instance.Connected == false ||
            marker == null) return;

        string dataJson = JsonUtility.ToJson(new MenuData()
        {
            menuName = uiName,
            selectedItemIndex = menu.currentItemIndex
        });

        CmdPackage cmd = new CmdPackage()
        {
            name = "Menu",
            data = dataJson,
        };
        //Debug.Log(JsonUtility.ToJson(cmd));
        UdpClient.Instance.SocketSend(JsonUtility.ToJson(cmd));
    }
}


public class MenuData
{
    public string menuName;
    public int selectedItemIndex;
}