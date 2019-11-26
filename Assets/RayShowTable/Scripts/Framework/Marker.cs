using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker
{
    public Vector2 position { get { return mPosition; } }
    public float angle { get { return mAngle; } }
    public int code { get { return mCode; } }
    public int guid { get { return mGuid; } }

    //唯一guid，不同marker编码可以相同，guid不会相同
    protected int mGuid = 0;
    //marker编码
    protected int mCode = 0;
    //旋转角度
    protected float mAngle = 0.0f;
    //位置
    protected Vector2 mPosition = Vector2.zero;
    //过期时间，marker一段时间内没有接收到数据更新将被认为过期，过期后将会自动销毁
    protected const float expiredTime = 3.0f;
    protected float mPreUpdateTime = 0;

    public virtual bool IsExpired()
    {
        //如果marker数据一段时间不更新则认为过期，可以移除
        return Time.time - mPreUpdateTime > expiredTime;
    }

    public Marker(Vector2 position, float angle, int code, int guid)
    {
        mPosition = position;
        mAngle = angle;
        mCode = code;
        mGuid = guid;
        mPreUpdateTime = Time.time;
    }

    public virtual void Update(Vector2 newPos, float newAngle)
    {
        mPosition = newPos;
        mAngle = newAngle;
        mPreUpdateTime = Time.time;
    }

   
}