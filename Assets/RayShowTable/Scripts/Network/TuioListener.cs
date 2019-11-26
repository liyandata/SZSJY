using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TUIOsharp;
using TUIOsharp.DataProcessors;

//功能：从指定端口接收和解析TUIO数据
public class TuioListener : SceneMonoSingleton<TuioListener>
{
    //监听端口
    public int tuioPort = 3333;
    public Text infoText;
    //事件通知
    //marker新增
    public System.Action<List<Marker>> MarkerAddCallback = delegate { };
    //marker移动
    public System.Action<List<Marker>> MarkerUpdateCallback = delegate { };
    //marker结束
    public System.Action<List<Marker>> MarkerRemoveCallback = delegate { };


    private TuioServer mServer;
    private ObjectProcessor mObjectProcessor;
    //保存当前所有marker的字典： guid - marker
    private Dictionary<int, Marker> mMarkerDictionary = new Dictionary<int, Marker>();

    float screenWidth = 1920f;
    float screenHeight = 1080f;

    private static readonly object locker = new object();

    List<Marker> addMarkers = new List<Marker>();
    List<Marker> updateMarkers = new List<Marker>();
    List<Marker> removeMarkers = new List<Marker>();
    List<Marker> expireMarkers = new List<Marker>();

    //待处理的tuio消息
    List<TUIOMsg> tuioMsgList = new List<TUIOMsg>();


    protected void OnEnable()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Connect();
    }

    protected  void OnDisable()
    {
        Disconnect();
    }

    //监听端口
    private void Connect()
    {
        if (!Application.isPlaying) return;
        if (mServer != null) Disconnect();

        mServer = new TuioServer(tuioPort);
        mServer.Connect();
        InitProcessor();
    }

    //停止监听
    private void Disconnect()
    {
        if (mServer != null)
        {
            mServer.RemoveAllDataProcessors();
            mServer.Disconnect();
            mServer = null;
        }
        mMarkerDictionary.Clear();
    }

    //添加tuio解析器
    private void InitProcessor()
    {
        if (mServer == null) return;
        mObjectProcessor = new ObjectProcessor();
        mObjectProcessor.ObjectAdded += OnObjectAdded;
        mObjectProcessor.ObjectUpdated += OnObjectUpdated;
        mObjectProcessor.ObjectRemoved += OnObjectRemoved;
        mServer.AddDataProcessor(mObjectProcessor);
    }



    private void RemoveExpiredMarker()
    {
        expireMarkers.Clear();
        foreach (var item in mMarkerDictionary)
        {
            if(item.Value.IsExpired())
            {
                expireMarkers.Add(item.Value);
            }
        }
        foreach (var item in expireMarkers)
        {
            mMarkerDictionary.Remove(item.guid);
        }
    }

    private void OnObjectAdded(object sender, TuioObjectEventArgs e)
    {
        var entity = e.Object;
        Debug.LogFormat("Marker Add ,Class = {0} ,ID = {1}", entity.ClassId,entity.Id);
        lock (locker)
        {
            tuioMsgList.Add(new TUIOMsg(entity.X, entity.Y, entity.Angle, entity.Id, entity.ClassId, 0));
        }
    }

    private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
    {
        var entity = e.Object;
        //Debug.LogFormat("Marker Update ,Class = {0} ,ID = {1}", entity.ClassId, entity.Id);
        lock (locker)
        {
            tuioMsgList.Add(new TUIOMsg(entity.X, entity.Y, entity.Angle, entity.Id, entity.ClassId, 1));
        }
    }


    private void OnObjectRemoved(object sender, TuioObjectEventArgs e)
    {
        var entity = e.Object;
        Debug.LogFormat("Marker Remove ,Class = {0} ,ID = {1}", entity.ClassId, entity.Id);
        lock (locker)
        {
            tuioMsgList.Add(new TUIOMsg(entity.X, entity.Y, entity.Angle, entity.Id, entity.ClassId, 2));
        }
    }

    //tuio的标准化坐标换算到屏幕坐标
    private Vector2 Tuio2Screen(float x, float y)
    {
        Vector2 screenPos = new Vector2(x * screenWidth, (1.0f - y) * screenHeight);
        return screenPos;
    }

    //更新ui信息
    void UpdateInfo()
    {
        if (infoText)
        {
            string info = string.Format("令牌数量 = {0}\n", mMarkerDictionary.Count);
            info += "-编码--位置--角度-\n";
            foreach (var item in mMarkerDictionary)
            {
                info += string.Format("    {0}     ({1},{2})  {3}\n", item.Value.code, item.Value.position.x.ToString("F0"), item.Value.position.y.ToString("F0"), item.Value.angle.ToString("F0"));
            }
            infoText.text = info;
        }
    }

    //在gui上显示marker信息
    private void Update()
    {
        HandleTUIOMsg();
        RemoveExpiredMarker();
        if (addMarkers.Count > 0)
        {
            MarkerAddCallback(addMarkers);
        }
        if (updateMarkers.Count > 0)
        {
            MarkerUpdateCallback(updateMarkers);
        }
        if (removeMarkers.Count > 0)
        {
            MarkerRemoveCallback(removeMarkers);
        }
        if (expireMarkers.Count > 0)
        {
            MarkerRemoveCallback(expireMarkers);
        }
        UpdateInfo();
    }


    void HandleTUIOMsg()
    {
        List<TUIOMsg> msgs;
        lock (locker)
        {
            msgs = new List<TUIOMsg>(tuioMsgList);
            tuioMsgList.Clear();
        }

        addMarkers.Clear();
        updateMarkers.Clear();
        removeMarkers.Clear();
        foreach (var msg in msgs)
        {
            switch (msg.eventType)
            {
                case 0:
                case 1:
                    {
                        if(mMarkerDictionary.ContainsKey(msg.id) == false)
                        {
                            Marker m = new Marker(Tuio2Screen(msg.x, msg.y), MathUtil.StandardizeAngle(Mathf.Rad2Deg * msg.angle) , msg.classID, msg.id);
                            mMarkerDictionary.Add(m.guid, m);
                            addMarkers.Add(m);
                        }
                        else
                        {
                            Marker m = mMarkerDictionary[msg.id];
                            m.Update(Tuio2Screen(msg.x, msg.y), MathUtil.StandardizeAngle(Mathf.Rad2Deg * msg.angle));
                            updateMarkers.Add(m);
                        }

                    }
                    break;
                case 2:
                    {
                        if (mMarkerDictionary.ContainsKey(msg.id))
                        {
                            Marker m = mMarkerDictionary[msg.id];
                            mMarkerDictionary.Remove(msg.id);
                            removeMarkers.Add(m);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}


public class TUIOMsg
{
    public float x;
    public float y;
    public float angle;
    public int id;
    public int classID;
    //事件类型 0 = 新增， 1 = 更新， 2 = 结束
    public int eventType;

    public TUIOMsg(float _x, float _y, float _angle, int _id, int _classID, int _eventType)
    {
        x = _x;
        y = _y;
        angle = _angle;
        id = _id;
        classID = _classID;
        eventType = _eventType;
    }
}