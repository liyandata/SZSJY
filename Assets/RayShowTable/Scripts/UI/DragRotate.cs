using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class DragRotate: MonoBehaviour, IInitializePotentialDragHandler,IDragHandler,IEndDragHandler
{
    //旋转中心
	public Transform rotateCenter = null;
    //角度取值范围
    public float minValue = 0f;
    public float maxValue = 359f;
    //起始角度
    public float startAngle = 0f;
	protected Vector3 offset = Vector3.zero;
    //旋转半径
	public float radius = 100;
    //当前角度值
    protected float m_rotateAngle = 0f;

    protected float angleRange
    {
        get
        {
            float range = maxValue - minValue;
            if (range <= 0f) range = 1f;
            return range;
        }
    }

    public float value01
    {
        get { return (m_rotateAngle - minValue) / (maxValue - minValue); }
    }


    //当前旋转值
    public float rotateAngle
    {
        get { return m_rotateAngle; }
        set { if(this.m_rotateAngle != value)
            {
                this.m_rotateAngle = value;
                OnAngleChange(value);
            }
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    protected virtual void Initialize()
    {
        offset = transform.position - rotateCenter.position;
        //Debug.Log("Position = " + transform.position);
        //Debug.Log("rotateCenter = " + rotateCenter.position);
        radius = offset.magnitude;
    }

    protected virtual void CleanUp()
    {

    }

    void Update ()
	{
		transform.position = rotateCenter.position + offset;
	}

	public void OnDrag (PointerEventData eventData)
	{
		HandleDrag (eventData);
	}
		
    //处理拖拽
	void HandleDrag (PointerEventData eventData)
	{
		if (rotateCenter == null)
			return;
		Vector2 localPos; 
		RectTransformUtility.ScreenPointToLocalPointInRectangle (rotateCenter as RectTransform, eventData.position, eventData.pressEventCamera, out localPos);
		if (localPos.magnitude < 2f)
			return;
		Vector2 dir = localPos.normalized;
        //Debug.Log ("dir = " + dir);
        float angle = MathUtil.ClockwiseAngle(Vector2.right, dir);
        if((angle + 360f )>=( minValue + startAngle) && (angle + 360f) <= (maxValue + startAngle))
        {
            angle += 360f;
        }
        angle = Mathf.Clamp(angle, minValue + startAngle, maxValue + startAngle);
        //Debug.Log("rotateAngle = " + (angle - startAngle));
        rotateAngle = angle - startAngle;
    }

	protected virtual void OnAngleChange (float value)
	{
        UpdatePointPosition();
    }

    protected void UpdatePointPosition()
    {
        //设置拖拽点位置，让其是绕圆心旋转
        offset = new Vector3(Mathf.Cos((rotateAngle + startAngle) * Mathf.Deg2Rad) * radius ,
            -Mathf.Sin((rotateAngle + startAngle) * Mathf.Deg2Rad) * radius , 0);
        transform.position = rotateCenter.position + offset;
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {

    }
}