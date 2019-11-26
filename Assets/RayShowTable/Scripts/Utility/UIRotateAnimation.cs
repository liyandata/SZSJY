using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotateAnimation : MonoBehaviour {

    //每秒旋转角度
    public float rotateSpeed = 120f;
    //是否顺时针旋转
    public bool clockwise = true;
    public float startAngle = 0;
    protected float currentAngle = 0;

	// Use this for initialization
	void Start () {
        currentAngle = startAngle;
    }
	
	// Update is called once per frame
	void Update () {
        currentAngle += rotateSpeed * Time.deltaTime * (clockwise ? -1f: 1f);
        transform.rotation = Quaternion.Euler(0f,0f,currentAngle);
	}
}
