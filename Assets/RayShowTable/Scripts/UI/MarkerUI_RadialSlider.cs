using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//环形进度条UI
public class MarkerUI_RadialSlider : BaseMarkerUI
{

    public GameObject sliderObj = null;
    private RadialSlider radialSlider = null;

    protected float deltaAngle = 0;
    protected float preMarkerAngle = 0;

    void Awake()
    {
        if (sliderObj)
            radialSlider = sliderObj.GetComponent<RadialSlider>();
        if (radialSlider)
        {
            radialSlider.ValueChangedCallback += OnValueChanged;
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
        if (marker == null)
            return;
        transform.position = marker.position;
        deltaAngle = marker.angle - preMarkerAngle;
        preMarkerAngle = marker.angle;
        if (Mathf.Abs(deltaAngle) > 300f)
        {
            //处理0和360临界的情况
            deltaAngle += (deltaAngle > 0f ? -360f : 360f);
        }
        //Debug.LogFormat("Angle = {0}, delta = {1}",marker.angle,deltaAngle);
        value01 += (deltaAngle / 360.0f);
    }

    protected  float value01
    {
        get { return radialSlider != null ? radialSlider.rawValue01 : 0; }
        set
        {
            if (radialSlider)
            {
                radialSlider.rawValue01 = value;
            }
        }
    }

    protected void OnValueChanged(float value)
    {
        NetWork_SendSliderData();
    }

    protected void NetWork_SendSliderData()
    {
        if (UdpClient.IsExisted == false ||
            UdpClient.Instance.Connected == false ||
            marker == null) return;

        string dataJson = JsonUtility.ToJson(new SliderData()
        {
            sliderName = uiName,
            sliderValue = radialSlider.rawValue01
        });

        CmdPackage cmd = new CmdPackage()
        {
            name = "Slider",
            data = dataJson,
        };
        //Debug.Log(JsonUtility.ToJson(cmd));
        UdpClient.Instance.SocketSend(JsonUtility.ToJson(cmd));
    }
}


public class SliderData
{
    public string sliderName;
    public float sliderValue;
}