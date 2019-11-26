using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UdpClient : SceneMonoSingleton<UdpClient>
{
    public string ip = "127.0.0.1";
    public int port = 8001;
    //用于测试的ui
    public InputField sendInputField;
    public Text receiveStrText;
    
    Socket socket; //目标socket  
    EndPoint serverEnd; //服务端  
    IPEndPoint ipEnd; //服务端端口  

    byte[] recvData = new byte[1024]; //接收的数据 
    byte[] sendData = new byte[1024]; //发送的数据 
    int recvLen; //接收的数据长度  
    Thread connectThread; //连接线程  
    List<string> recvStrList = new List<string>(); //接收的字符串  
    readonly object locker = new object();

    public System.Action<string> CmdReceiveCallback = delegate { };
    public bool Connected
    {
        get { return socket != null && socket.Connected; }
    }


    //初始化  
    void InitSocket()
    {
        //定义连接的服务器ip和端口 
        ipEnd = new IPEndPoint(IPAddress.Parse(ip), port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //定义服务端  
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("waiting for sending UDP dgram");
        //建立初始连接，非常重要，第一次连接初始化了serverEnd后面才能收到消息  
        SocketSend("");
        //开启一个接收线程  
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    public void SocketSend(string sendStr)
    {
        //清空发送缓存  
        sendData = new byte[1024];
        //数据类型转换  
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //发送给指定服务端  
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    public void SendInputText()
    {
        if(sendInputField)
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
            //获取客户端，获取服务端端数据，用引用给服务端赋值，实际上服务端已经定义好并不需要赋值  
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString()); //打印服务端信息  
            //输出接收到的数据  
            string str = Encoding.ASCII.GetString(recvData, 0, recvLen);
            lock (locker)
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
        InitSocket(); //在这里初始化  
    }


    // Update is called once per frame  
    void Update()
    {
        lock (locker)
        {
            foreach (var item in recvStrList)
            {
                //处理接收到的消息
                Debug.Log(item);
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