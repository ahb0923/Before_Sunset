using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper_Transform
{
    /// <summary>
    /// Transform의 세팅 초기화
    /// </summary>
    /// <param name="transform">자신</param>
    public static void ResetTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 목표를 바라보게 회전 
    /// </summary>
    /// <param name="t">자신</param>
    /// <param name="targetPosition">목표</param>
    public static void LookAt2D(this Transform t, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - t.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        t.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Transform의 X만 변경
    /// </summary>
    public static void SetX(this Transform t, float x)
    {
        Vector3 pos = t.position;
        pos.x = x;
        t.position = pos;
    }

    /// <summary>
    /// Transform의 Y만 변경
    /// </summary>
    public static void SetY(this Transform t, float y)
    {
        Vector3 pos = t.position;
        pos.y = y;
        t.position = pos;
    }

}

