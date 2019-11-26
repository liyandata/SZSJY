using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RadialSlider: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    //slider的数值 , min - max
	public float Value {
		get{ return m_value01 * range + minValue; }
		set {
			float newValue = Mathf.Clamp(value,minValue,maxValue);
            rawValue01 = (newValue - minValue) / (range == 0 ? 1f: range);
		}
	}
    //slider的数值 , 0 - 1
    public float rawValue01
    {
        get { return m_value01; }
        set
        {
            m_value01 = Mathf.Clamp01(value);
            SetFillRect(m_value01);
            if (ValueChangedCallback != null)
            {
                ValueChangedCallback(Value);
            }
        }
    }

 
    public float minValue = 0f;
    public float maxValue = 1f;

    protected float range { get { return maxValue - minValue; } }

    public Image fillRect = null;
	[SerializeField][Range(0,1)]
    protected float m_value01 = 0;
	bool isPointerDown = false;


	public System.Action<float> ValueChangedCallback = delegate {
		
	};

	void Start ()
	{
		if (fillRect) {
			fillRect.fillMethod = Image.FillMethod.Radial360;
			SetFillRect (rawValue01);
		}
	}


    //Called when the pointer enters our GUI component.
    //Start tracking the mouse
    public void OnPointerEnter(PointerEventData eventData)
    {
        //StartCoroutine("TrackPointer");
    }

    // Called when the pointer exits our GUI component.
    // Stop tracking the mouse
    public void OnPointerExit(PointerEventData eventData)
    {
        //StopCoroutine("TrackPointer");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        //Debug.Log("mousedown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        //Debug.Log("mousedown");
    }

    // mainloop
    IEnumerator TrackPointer()
    {
        var ray = GetComponentInParent<GraphicRaycaster>();
        if (ray != null)
        {
            while (Application.isPlaying)
            {

                // TODO: if mousebutton down
                if (isPointerDown)
                {

                    Vector2 localPos; // Mouse position  
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, ray.eventCamera, out localPos);
                    rawValue01 = (Mathf.Atan2(-localPos.y, localPos.x) * 180f / Mathf.PI + 180f) / 360f;
              }
                yield return 0;
            }
        }
        else
            UnityEngine.Debug.LogWarning("Could not find GraphicRaycaster and/or StandaloneInputModule");
    }

    void SetFillRect (float fillAmount)
	{
		if (fillRect) {
			fillRect.fillAmount = fillAmount;
		}
	}



}
