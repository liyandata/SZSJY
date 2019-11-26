using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RadialSliderHandle : DragRotate {

    public RadialSlider radialSlider = null;


    protected override void OnAngleChange(float newAngle)
    {
        //base.OnAngleChange(value);
        if (radialSlider)
        {
            radialSlider.rawValue01 = (newAngle - minValue) / angleRange;
            //Debug.Log("RadialSlider  rawValue01 = " + radialSlider.Value);
        }
    }


    void OnSliderValueChange(float sliderValue)
    {
        if(radialSlider != null)
        {
            m_rotateAngle = radialSlider.rawValue01 * angleRange + minValue;
        }
        base.OnAngleChange(sliderValue);
    }

    protected override void Initialize()
    {
        base.Initialize();
        if(radialSlider)
        {
            radialSlider.ValueChangedCallback += OnSliderValueChange;
        }
    }

    protected override void CleanUp()
    {
        base.CleanUp();
        if (radialSlider)
        {
            radialSlider.ValueChangedCallback -= OnSliderValueChange;
        }
    }
}
