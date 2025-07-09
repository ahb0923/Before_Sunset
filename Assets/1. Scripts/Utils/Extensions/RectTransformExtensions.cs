using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    private static readonly Dictionary<RectTransform, Vector2> _originalPositions = new();

    public static void OpenAtCenter(this RectTransform rect)
    {
        if (!_originalPositions.ContainsKey(rect))
        {
            _originalPositions[rect] = rect.anchoredPosition;
        }

        rect.anchoredPosition = Vector2.zero;
        rect.gameObject.SetActive(true);
    }

    public static void CloseAndRestore(this RectTransform rect)
    {
        if (_originalPositions.TryGetValue(rect, out var originalPos))
        {
            rect.anchoredPosition = originalPos;
        }

        rect.gameObject.SetActive(false);
    }
}