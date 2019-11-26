using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class UdpServer : MonoBehaviour
{
    public int port = 8001;
    //用于测试的ui
    public InputField sendInputField;
    public Text receiveStrText;

    Socket socket; //目标socket  
    EndPoint clientEnd; //客户端  
    IPEndPoint ipEnd; //侦听端口  
    
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节  
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节  
    int recvLen; //接收的数据长度  
    Thread connectThread; //连接线程  
    List<string> recvStrList = new List<string>(); //接收的字符串  
    readonly object locker = new object();

    public System.Action<string> CmdReceiveCallback = delegate { };

    //初始化  
    void InitSocket()
    {
        //定义侦听端口,侦听任何IP  
        ipEnd = new IPEndPoint(IPAddress.Any, port);
        //定义套接字类型,在主线程中定义  
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //服务端需要绑定ip  
        socket.Bind(ipEnd);
        //定义客户端  
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        clientEnd = (EndPoint)sender;
        print("Waiting for UDP dgram");
        //开启一个线程连接
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketSend(string sendStr)
    {
        //清空发送缓存  
        sendData = new byte[1024];
        //数据类型转换  
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //发送给指定客户端  
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }

    public void SendInputText()
    {
        if (sendInputField)
        {
            SocketSend(sendInputField.text);
        }
    }

    //服务器接收  
    void SocketReceive()
    {
        //进入接收循环  
        while (true)
        {
            //对data清零  
            recvData = new byte[1024];
            //获取客户端，获取客户端数据，用引用给客户端赋值  
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            //print("message from: " + clientEnd.ToString()); //打印客户端信息  
            //输出接收到的数据  
            string str = Encoding.ASCII.GetString(recvData, 0, recvLen);
            //print(str);
            lock(locker)
            {
                recvStrList.Add(str);
            }
        }
    }

    //连接关闭  
    void SocketQuit()
    {
        //关闭线程  
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭socket  
        if (socket != null)
            socket.Close();
    }

    // Use this for initialization  
    void Start()
    {
        //初始化server  
        InitSocket(); 
    }


    // Update is called once per frame  
    void Update()
    {
        lock (locker)
        {
            foreach (var item in recvStrList)
            {
                //处理接收到的消息
                //Debug.Log(item);
                if (receiveStrText) receiveStrText.text = item;
                CmdReceiveCallback(item);
            }
            recvStrList.Clear();
        }
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}