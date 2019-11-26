using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//控制角色或相机的UI
public class MarkerUI_Camera : BaseMarkerUI
{
	public Slider heightSlider = null;
	public Slider bgScaleSlider = null;
	public CamViewRotate camViewDrag = null;
	public GameObject markerBodyObj = null;
	public Image markerInnerImg = null;
	public Image markerOutImg = null;
	public Image markerPointerImg = null;

	protected void Start ()
	{
		transform.localPosition = Vector3.zero;
       
		if (bgScaleSlider) {
            bgScaleSlider.onValueChanged.AddListener(OnScaleSliderValueChange);
		}
        if (heightSlider)
        {
            heightSlider.onValueChanged.AddListener(OnHeightValueChanged);
        }
        if (camViewDrag)
        {
            camViewDrag.onValueChanged.AddListener(OnViewValueChanged);
        }

        RectTransform rect = gameObject.GetComponent<RectTransform>();
        if(rect)
        {
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        UpdateUI();
    }

    public override void UpdateUI ()
	{
		if (marker == null || markerBodyObj == null || heightSlider == null ||
		    camViewDrag == null) {
			Debug.Log ("MarkerUI_Camera.UpdateUI Param == null");
			return;
		}
        markerBodyObj.transform.position = mMarker.position;
        markerBodyObj.transform.rotation = Quaternion.Euler(0, 0, -mMarker.angle);
        Network_SendCameraData();
    }

    protected void Network_SendCameraData ()
	{
        if (UdpClient.IsExisted == false ||
            UdpClient.Instance.Connected == false ||
            marker == null) return;

        Vector2 pos = MathUtil.Screen2Normalize(marker.position);

        string dataJson = JsonUtility.ToJson(new CameraData() {
            cameraPos = new Vector3(pos.x, heightSlider.value,pos.y),
            cameraEuler = new Vector3(-camViewDrag.rotateAngle,marker.angle,0)
        });
        CmdPackage cmd = new CmdPackage()
        {
            name = "Camera",
            data = dataJson,
        };
        //Debug.Log(JsonUtility.ToJson(cmd));
        UdpClient.Instance.SocketSend(JsonUtility.ToJson(cmd));
    }
		
	protected void OnScaleSliderValueChange (float value)
	{

	}

    protected void OnViewValueChanged(float value)
    {
        Network_SendCameraData();
    }

    protected void OnHeightValueChanged(float value)
    {
        Network_SendCameraData();
    }
}


public class CameraData
{
    //归一化相机位置,场景那边接收后再自行映射到 min - max 的范围内
    public Vector3 cameraPos;
    //旋转欧拉角
    public Vector3 cameraEuler;
}