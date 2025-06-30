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
    /// <param name="t">자신</param>
    /// <param name="x">x좌표</param>
    public static void SetX(this Transform t, float x)
    {
        Vector3 pos = t.position;
        pos.x = x;
        t.position = pos;
    }

    /// <summary>
    /// Transform의 Y만 변경
    /// </summary>
    /// <param name="t">자신</param>
    /// <param name="y">y좌표</param>
    public static void SetY(this Transform t, float y)
    {
        Vector3 pos = t.position;
        pos.y = y;
        t.position = pos;
    }

    /// <summary>
    /// 두개의 트랜스폼 사이의 근접도 체크
    /// </summary>
    /// <param name="a">첫번째 Transform</param>
    /// <param name="b">두번째 Transform</param>
    /// <param name="tolerance">오차 허용치, 기본값 0.01f</param>
    /// <returns>두 Transform의 거리 차이가 오차 허용치 보다 작으면 true, 크면 false</returns>
    public static bool IsCloseTo(this Transform a, Transform b, float tolerance = 0.01f)
    {
        return Vector3.Distance(a.position, b.position) <= tolerance;
    }
}

