using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//下屏逻辑
public class Example3_Client : MonoBehaviour {
    UdpClient udpClient;
 
    // Use this for initialization
    void Awake()
    {
        udpClient = FindObjectOfType<UdpClient>();
        if (udpClient == null)
        {
            Debug.LogError("找不到UdpClient!");
        }
        else
        {
            udpClient.CmdReceiveCallback += OnCmdReceive;
        }
    }


    //处理接收到的消息
    void OnCmdReceive(string cmdJson)
    {
        //CmdPackage cmd = JsonUtility.FromJson<CmdPackage>(cmdJson);
        //如果有下屏ui要处理的网络命令，在这里处理

    }
}




