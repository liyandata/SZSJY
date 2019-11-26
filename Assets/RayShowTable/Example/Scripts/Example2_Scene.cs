using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example2_Scene : SceneMonoSingleton<Example2_Scene> {


    public GameObject testObj;

    //设置测试物体的颜色
    public void SetObjColor(int colorIndex)
    {
        if (testObj == null) return;
        MeshRenderer mr = testObj.GetComponent<MeshRenderer>();
        if (mr)
        {
            Color c = Color.white;
            switch (colorIndex)
            {
                case 0:
                    c = Color.red;
                    break;
                case 1:
                    c = Color.green;
                    break;
                case 2:
                    c = Color.blue;
                    break;
                default:
                    break;
            }
            mr.material.color = c;
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
