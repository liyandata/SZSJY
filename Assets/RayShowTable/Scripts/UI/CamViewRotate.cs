using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//拖拽控制相机的俯仰角度
public class CamViewRotate : DragRotate
{
	public CamViewRotateEvent onValueChanged = new CamViewRotateEvent ();
    public Slider slider;
    public Image camViewImage;
	protected override void OnAngleChange(float value)
	{
		base.OnAngleChange(value);
		if (onValueChanged != null)
        {
            //将角度做一下转换，默认 = 0 ，绕x轴旋转 >0 低头， 
            onValueChanged.Invoke(value);
        }

        if(camViewImage)
        {
            camViewImage.rectTransform.rotation = Quaternion.Euler(0, 0, -value);
        }
	}

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        if (slider)
        {
            slider.interactable = false;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (slider)
        {
            slider.interactable = true;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        UpdatePointPosition();
    }
}


public class CamViewRotateEvent: UnityEvent<float>
{
	public CamViewRotateEvent ()
	{
		
	}
}