using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//上屏场景业务逻辑处理
public class Example3_Server : MonoBehaviour
{
    UdpServer udpServer;
    //测试相机
    public Camera testCamera;
    //菜单测试物体
    //public GameObject testMenuObj;
    //滑动条测试物体
    public GameObject testSliderObj;
    public Text Tips;
    ////用来定义相机移动范围，从client发过来的是归一化的参数，到这里后映射到min - max
    //public Transform areaMin;
    //public Transform areaMax;

    [System.Serializable]
    public class AreaConfig
    {
        public Transform areaMin;
        public Transform areaMax;
        public string tips;
    }
    public List<AreaConfig> areaConfigs;

    // Use this for initialization
    void Awake()
    {
        udpServer = FindObjectOfType<UdpServer>();
        if (udpServer == null)
        {
            Debug.LogError("找不到UdpServer!");
        }
        else
        {
            udpServer.CmdReceiveCallback += OnCmdReceive;
        }
    }


    //处理接收到的消息
    void OnCmdReceive(string cmdJson)
    {
        //Debug.Log("Cmd Json = " + cmdJson);
        if (string.IsNullOrEmpty(cmdJson)) return;
        try
        {
            CmdPackage cmd = JsonUtility.FromJson<CmdPackage>(cmdJson);
            if (cmd == null) return;
            switch (cmd.name)
            {
                case "Camera":
                    HandleCamera(JsonUtility.FromJson<CameraData>(cmd.data));
                    break;
                case "Menu":
                    HandleMenu(JsonUtility.FromJson<MenuData>(cmd.data));
                    break;
                case "Slider":
                    HandleSlider(JsonUtility.FromJson<SliderData>(cmd.data));
                    break;
                default:
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

    }
    int n = 0;
    void HandleCamera(CameraData camData)
    {
        if (testCamera == null) return;
        Vector3 minPos = areaConfigs[n].areaMin.position;
        Vector3 maxPos = areaConfigs[n].areaMax.position;
        Tips.text = areaConfigs[n].tips;
        Vector3 pos = new Vector3(
            Mathf.Lerp(minPos.x, maxPos.x, camData.cameraPos.x),
            Mathf.Lerp(minPos.y, maxPos.y, camData.cameraPos.y),
            Mathf.Lerp(minPos.z, maxPos.z, camData.cameraPos.z));

        testCamera.transform.position = pos;
        testCamera.transform.rotation = Quaternion.Euler(camData.cameraEuler);

    }

    void HandleMenu(MenuData data)
    {
        //if (testMenuObj == null) return;

        //Color c = Color.white;
        switch (data.selectedItemIndex)
        {
            case 0:
                n = 0;
                break;
            case 1:
                n = 1;
                break;
            case 2:
                n = 2;
                break;
            case 3:
                n = 3;
                break;
            case 4:
                n = 4;
                break;
        }

    }


    void HandleSlider(SliderData data)
    {


        if (testSliderObj == null) return;
        if (data.sliderValue < 0.4f)
        {
            testSliderObj.SetActive(true);

        }
        else
            testSliderObj.SetActive(false);


        //print(data.sliderValue);o
        //MeshRenderer mr = testSliderObj.GetComponent<MeshRenderer>();
        //if (mr)
        //{
        //    mr.material.color = Color.Lerp(Color.green, Color.red, data.sliderValue);
        //}
    }
}
