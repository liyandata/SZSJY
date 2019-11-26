using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//环形菜单
public class RadialMenu : BaseMenu
{
	public MenuItem menuItemPrefab = null;
	public float radius = 200f;
	//第一个菜单起始角度
	public float startAngle = 270f;
	//指针物体
	public GameObject pointer = null;
    //当前项高亮底图
    public Image currentItemHighlight = null;
    //待选项高亮底图
    public Image nextItemHighlight = null;

    //已创建的菜单数量
    public int menuItemCount {
		get{ return m_menuItems.Count; }
	}

	//预设的菜单数量
	public int presetMenuItemCout {
		get{ return itemImages != null ? itemImages.Length : 0; }
	}

	//当前菜单序号 0- n
	public override int currentItemIndex {
		get { return m_currentItemIndex; }
		protected set { m_currentItemIndex = value; }
	}
	protected int m_currentItemIndex = -1;

	protected List<MenuItem> m_menuItems = new List<MenuItem> ();

    //8.24,菜单选择方式修改，原先通过marker当前角度得到menu的选项，现在是通过marker的累计转动角度出发选项改变
    //固定转动某一角度出发，这在项数比较少或者比较多的时候体验更好
    float deltaAngleTotal = 0;
    
    //触发选项吸附的旋转角度
    float snapTriggerAngle
    {
        get { float result =  180f / (menuItemCount + 1);  return Mathf.Min(50f,result); }
    }

    float itemAngle { get { return menuItemCount > 0 ? 360f / menuItemCount : 0; }  }


	public override void CreateMenuItems (bool reCreate = false, int initItemIndex = 0)
	{

		if (menuItemPrefab == null || itemImages == null)
			return;
		if (menuItemCount > 0) {
			if (reCreate == false)
				return;
			else {
				DestroyMenuItems ();
                m_currentItemIndex = -1;
            }
		}
		int i = 0;
		foreach (var img in itemImages) {
			MenuItem item = Instantiate(menuItemPrefab);
			item.transform.SetParent (transform);
            item.transform.localScale = menuItemPrefab.transform.localScale;
            item.image.sprite = img;
			item.gameObject.SetActive (true);
			item.name = string.Format ("MenuItem{0}", i.ToString ());
            //添加回调
            item.MenuItemSelectCallbak += OnMenuItemClick;
            m_menuItems.Add (item);
			i++;
		}
		SetRadialLayout ();
        SelectItem(initItemIndex);
	}

	public override void DestroyMenuItems ()
	{
		foreach (var item in m_menuItems) {
            item.MenuItemSelectCallbak -= OnMenuItemClick;
            Destroy (item.gameObject);
		}
		m_menuItems.Clear ();
	}

    //菜单项被选择时的回调
    void OnMenuItemClick(MenuItem item)
    {
        int index = GetMenuItemIndex(item);
        if (index == -1) return;
        SelectItem(index);
    }

	protected void SetRadialLayout ()
	{
		float angleStep = itemAngle;

		float angle = startAngle;
		for (int i = 0; i < m_menuItems.Count; i++) {
			var item = m_menuItems [i];
			RectTransform t = (RectTransform)item.transform;
			if (t != null) {
				Vector3 pos = new Vector3 (Mathf.Cos (-angle * Mathf.Deg2Rad), Mathf.Sin (-angle * Mathf.Deg2Rad), 0);
				t.localPosition = pos * radius;
				t.anchorMin = t.anchorMax = t.pivot = new Vector2 (0.5f, 0.5f);
				angle += angleStep;
			}
		}
	}

	//选择菜单项
	public override void SelectItem (int index, bool invokeCallback = true)
	{
		if (index == currentItemIndex || index < 0 || index >= menuItemCount)
			return;
        //Debug.Log("SelectItem = " + index);
        MenuItem item = GetMenuItem (index);
		if (item != null) {
            if (invokeCallback && MenuSelectCallback != null) {
				MenuSelectCallback.Invoke (index);
			}
		}
        HightlightSelectedItem(index);
        HightlighNextItem(-1);
        PointToMenuItem(index);
        currentItemIndex = index;
	}

    //高亮当前项
    protected void HightlightSelectedItem(int index)
    {
        if (currentItemHighlight == null) return;
        if (index < 0 || index >= menuItemCount)
        {
            currentItemHighlight.fillAmount = 0f;
            return;
        }
        float anglePerItem = 360f / menuItemCount;
        currentItemHighlight.fillAmount = 1f / menuItemCount;
        float rotateAngle = startAngle + anglePerItem * index - anglePerItem * 0.5f;
        currentItemHighlight.transform.rotation = Quaternion.Euler(0,0,-rotateAngle);
    }

    //高亮待选项
    protected void HightlighNextItem(int index ,float alpha = 1)
    {
        if (nextItemHighlight == null) return;
        if (index < 0 || index >= menuItemCount)
        {
            nextItemHighlight.fillAmount = 0f;
            return;
        }
        float anglePerItem = 360f / menuItemCount;
        nextItemHighlight.fillAmount = 1f / menuItemCount;
        float rotateAngle = startAngle + anglePerItem * index - anglePerItem * 0.5f;
        nextItemHighlight.transform.rotation = Quaternion.Euler(0, 0, -rotateAngle);
        Color c = nextItemHighlight.color;
        float maxAlpha = 0.5f;
        c.a = maxAlpha * alpha;
        nextItemHighlight.color = c;
    }

    MenuItem GetMenuItem (int index)
	{
		if (index < 0 || index >= menuItemCount)
			return null;
		return m_menuItems [index];
	}

    int GetMenuItemIndex(MenuItem item)
    {
        return m_menuItems.FindIndex(a=>a==item);
    }

    int CorrectItemIndex(int index)
    {
        if (menuItemCount == 0) return 0;
        index = index % menuItemCount;
        if (index < 0) return index + menuItemCount;
        else return index;
    }


    private void SetSelectState(MenuItem item, bool selected)
    {
        if (item == null) return;
        Color selectedColor = new Color(1.0f, 0.4f, 0.4f);
        Color unSelectedColor = Color.white;
        Image img = item.image;
        if (img)
        {
            img.color = selected ? selectedColor : unSelectedColor;
        }
    }

    //根据角度得到对应的menu序号
    //protected int GetMenuIndexByAngle (float angle)
    //{
    //	if (menuItemCount == 0)
    //		return -1;
    //	float step = 360f / menuItemCount;
    //	//菜单1 对应的角度范围应该是 -0.5f * step 到 0.5f * step
    //	angle = MathUtil.StandardizeAngle (angle + 0.5f * step);
    //	int result = Mathf.FloorToInt (angle / step);
    //	return result;
    //}

    //传入的是marker的旋转角度
    public override void RotateSelectMenu (float deltaAngle)
	{
        //如果触发了吸附那就不处理
        //吸附成功后一小段时间内也不处理旋转
        if (isSnapping || (Time.time - rotateCDTimer < rotateCD))
        {
            return;
        }

        //这里需要保证snaptriggerAngle大于单菜单项的角度
        if (Mathf.Abs(deltaAngleTotal + deltaAngle) > snapTriggerAngle)
        {
            //开始吸附
            int snapIndex = (deltaAngleTotal + deltaAngle) > 0 ? currentItemIndex + 1 : currentItemIndex - 1;
            snapIndex = CorrectItemIndex(snapIndex);
            SnapToItem(snapIndex);
        }
        else
        {
            ApplyRotate(deltaAngle);
            UpdateSnapCurrent(deltaAngle);
        }
    }

    void ApplyRotate(float deltaAngle)
    {
        if (pointer != null)
        {
            pointer.transform.Rotate(new Vector3(0, 0, -deltaAngle));
        }

        //deltaAngleTotal是当前项中心的累计偏移角度
        deltaAngleTotal += deltaAngle;
        int current = currentItemIndex;
        int next = current;

        if (deltaAngleTotal <= -itemAngle)
        {
            current -= 1;
            deltaAngleTotal += itemAngle;
        }
        else if (deltaAngleTotal >= itemAngle)
        {
            current += 1;
            deltaAngleTotal -= itemAngle;
        }

        if (deltaAngleTotal < 0)
        {
            next -= 1;
        }
        else if (deltaAngleTotal > 0)
        {
            next += 1;
        }

        //处理序号跨界
        next = CorrectItemIndex(next);
        current = CorrectItemIndex(current);

        SelectItem(current);

        float alpha = Mathf.Abs(deltaAngleTotal) / itemAngle;
        HightlighNextItem(next, alpha);
    }



    //吸附触发时间
    float snapTriggerTime = 0.5f;
    float snapTimer = 0;
    bool isSnapping = false;
    //吸附成功后的一小段时间内不再移动，优化体验
    float rotateCDTimer = 0f;
    const float rotateCD = 0.3f;

    //吸附回原菜单项的处理
    void UpdateSnapCurrent(float deltaAngle)
    {
        if (deltaAngleTotal == 0) return;
        if (Mathf.Abs(deltaAngle) > 0.3f)
        {
            snapTimer = 0;
            return;
        }
        else
        {
            snapTimer += Time.deltaTime;
        }

        //用户一段时间内不操作，并且当前有偏移角度的话，吸附回原菜单项
        if (snapTimer >  snapTriggerTime)
        {
            //Debug.Log("Trigger Snap Current");
            SnapToItem(currentItemIndex);
        }

    }

    //吸附到指定菜单项
    void SnapToItem(int index)
    {
        if(isSnapping == false)
        {
            isSnapping = true;
            StartCoroutine(SnapToItemCoroutine(index));
        }
    }

    //参数：要改变的角度
    IEnumerator SnapToItemCoroutine(int index)
    {
        //Debug.Log("SnapToItemCoroutine  begin  ------------>");
        isSnapping = true;
        float targetDeltaAngle = (index - currentItemIndex) * itemAngle;
        if (Mathf.Abs(targetDeltaAngle - deltaAngleTotal) > 180f)
        {
            if (targetDeltaAngle > 0)
            {
                targetDeltaAngle -= 360f;
            }
            else if (targetDeltaAngle < 0)
            {
                targetDeltaAngle += 360f;
            }
        }
        //Debug.LogFormat("currentItem = {0}, targetItem = {1} , targetDeltaAngle = {2}", currentItemIndex, index, targetDeltaAngle);

        if (targetDeltaAngle == deltaAngleTotal)
        {
            //Debug.Log("SnapToItemCoroutine  break");
            isSnapping = false;
            yield break;
        }
       

        float from = deltaAngleTotal;
        float to = targetDeltaAngle;

        
        //Debug.LogFormat("targetDeltaAngle = {0} , deltaAngleTotal = {1}", to, from);
        float duration = 0.3f;
        float speed = (to -  from)  / duration;
        //speed = Mathf.Max(120f, speed);
        float startTime = Time.time;

        while (true)
        {
            yield return null;
            float delta = (Time.time - startTime) * speed;
            if(Mathf.Abs(delta) > Mathf.Abs(to - from))
            {
                delta = to - from;
            }
            from += delta;
            ApplyRotate(delta);
            if (Time.time - startTime > duration || from == to) break;
        }
        SelectItem(index);
        isSnapping = false;
        rotateCDTimer = Time.time;
        //Debug.Log("SnapToItemCoroutine end <------------");
    }

    //设置指针指向指定菜单项
    void PointToMenuItem(int index)
    {
        if (index < 0 || index >= menuItemCount) return;
        if (pointer != null)
        {
            //pointerOriginAngle指的是指针图片不旋转的时候指向的角度
            float pointerOriginAngle = 270;
            float angle = index * itemAngle + startAngle - pointerOriginAngle;
            pointer.transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
        deltaAngleTotal = 0;
    }
}
