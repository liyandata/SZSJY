using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//激活多屏显示
public class DisplayActivate : MonoBehaviour {

	void Awake () {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
	}
}
