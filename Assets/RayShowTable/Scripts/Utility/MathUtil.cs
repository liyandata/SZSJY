using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
	//角度标准化，转换成 0 - 360范围
	public static float StandardizeAngle (float angle)
	{
        if (angle >= 0f && angle < 360f)
            return angle;
        else
            return angle % 360f + (angle < 0f ? 360f : 0f);
	}

	//返回2个向量的夹角 0 - 360，即from绕Z轴旋转到to的角度，左手系,2d屏幕坐标下是逆时针
	public static float Angle (Vector2 from, Vector2 to)
	{
		if (from == Vector2.zero || to == Vector2.zero)
			return 0;
		float sin = from.x * to.y - to.x * from.y;
		float cos = from.x * to.x + to.y * from.y;
		float angle = Mathf.Atan2 (sin, cos) * Mathf.Rad2Deg;
		return StandardizeAngle(angle);
	}

    //返回2个向量的夹角 0 - 360，屏幕坐标下顺时针
    public static float ClockwiseAngle(Vector2 from, Vector2 to)
    {
        return StandardizeAngle(- Angle(from,to));
    }

    //将一个2d向量旋转指定角度
    public static Vector2 Rotate (this Vector2 v, float degree)
	{
		float sin = Mathf.Sin (degree * Mathf.Deg2Rad);
		float cos = Mathf.Cos (degree * Mathf.Deg2Rad);
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}

	//判断2个向量是否足够接近
	public static bool AlmostEqual (this Vector2 v1, Vector2 v2, float tolerence = 0.01f)
	{
		return (v1 - v2).magnitude < tolerence;
	}


    //屏幕坐标转成单位化坐标（0-1范围）
    public static Vector2 Screen2Normalize(Vector2 screenPos)
    {
        return new Vector2(screenPos.x/ Screen.width,screenPos.y/Screen.height);
    }

}
