using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//marker ui 的默认样式
public class MarkerUI_Default : BaseMarkerUI
{
    public Text codeText;
    public Image bodyImg;


    //根据marker参数更新ui
    public override void UpdateUI()
    {
        if (mMarker == null || codeText == null || bodyImg == null)
        {
            Debug.LogError("Update Marker UI Error!");
            return;
        }
        codeText.text = mMarker.code.ToString();
        transform.position = mMarker.position;
        bodyImg.transform.rotation = Quaternion.Euler(0, 0, -mMarker.angle);
    }
}
