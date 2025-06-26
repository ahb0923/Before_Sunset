using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RenderUtil
{
    /// <summary>
    /// Y 좌표에 따라 SpriteRenderer의 sortingOrder를 자동으로 설정합니다.
    /// </summary>
    public static void SetSortingOrderByY(SpriteRenderer renderer)
    {
        renderer.sortingOrder = (int)(renderer.transform.position.y * -100);
    }
}