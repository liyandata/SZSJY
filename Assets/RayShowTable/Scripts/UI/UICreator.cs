using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MarkerUIConfig
{
    public int code;
    public BaseMarkerUI uiPrefab;
}



public class UICreator : MonoBehaviour {
    [Header("[MarkerUI预设配置]")]
    [Tooltip("为每个Marker编码设置各自的UI")]
    public List<MarkerUIConfig> uiConfigs;
    [Header("[默认UI预设]")]
    [Tooltip("没有设置UI样式的Marker将会使用默认的UI样式")]
    public BaseMarkerUI defaultUIPrefab;
    [Header("[UI Canvas]")]
    public Canvas uiRoot;

    //UI预设字典，编码 - 预设 键值对
    Dictionary<int, BaseMarkerUI> mMarkerPrefabs = new Dictionary<int, BaseMarkerUI>();
    //UI实例字典，guid - ui实例
    Dictionary<int, BaseMarkerUI> mMarkerUIInstance = new Dictionary<int, BaseMarkerUI>();


    void Awake () {
        TuioListener listener = FindObjectOfType<TuioListener>();
        if(listener)
        {
            listener.MarkerAddCallback += OnMarkerAdd;
            listener.MarkerUpdateCallback += OnMarkerUpdate;
            listener.MarkerRemoveCallback += OnMarkerRemove;
        }
        else
        {
            Debug.LogError("找不到TuioListener!");
        }

        if(uiRoot == null)
        {
            uiRoot = FindObjectOfType<Canvas>();
        }

        mMarkerPrefabs.Clear();
        foreach (var item in uiConfigs)
        {
            mMarkerPrefabs[item.code] = item.uiPrefab;
        }
	}

    //根据marker编码得到ui预设
    BaseMarkerUI GetMarkerUIPrefab(int code)
    {
        BaseMarkerUI prefab = null;
        if (mMarkerPrefabs.ContainsKey(code))
        {
            prefab = mMarkerPrefabs[code];
        }
        if (prefab == null) prefab = defaultUIPrefab;
        return prefab;
    }


    void OnMarkerAdd(List<Marker> markers)
    {
        foreach (var m in markers)
        {
            BaseMarkerUI prefab = GetMarkerUIPrefab(m.code);
            if (prefab == null) continue;
            Transform parent = uiRoot != null ? uiRoot.transform : null;
            GameObject uiObj = Instantiate(prefab.gameObject, parent) as GameObject;
            if (uiObj == null) continue;
            BaseMarkerUI ui = uiObj.GetComponent<BaseMarkerUI>();
            if (ui)
            {
                ui.transform.SetParent(parent);
                ui.InitMarker(m);
            }
            mMarkerUIInstance.Add(m.guid, ui);
        }
    }

    void OnMarkerUpdate(List<Marker> markers)
    {
        foreach (var m in markers)
        {
            if (mMarkerUIInstance.ContainsKey(m.guid))
            {
                BaseMarkerUI ui = mMarkerUIInstance[m.guid];
                if (ui)
                {
                    ui.UpdateUI();
                }
            }
        }
    }

    void OnMarkerRemove(List<Marker> markers)
    {
        foreach (var m in markers)
        {
            BaseMarkerUI ui = mMarkerUIInstance[m.guid];
            if (ui)
            {
                Destroy(ui.gameObject);
            }
            mMarkerUIInstance.Remove(m.guid);
        }
    }
}
