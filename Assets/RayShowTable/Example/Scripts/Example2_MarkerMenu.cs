using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example2_MarkerMenu : MarkerUI_RadialMenu {

    protected override void OnMenuSelect(int menuIndex)
    {
        Example2_Scene.Instance.SetObjColor(menuIndex);
    }
}
