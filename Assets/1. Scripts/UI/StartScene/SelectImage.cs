using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectImage : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private RectTransform _leftRect;
    [SerializeField] private RectTransform _rightRect;
    [SerializeField] private Image _leftImage;
    [SerializeField] private Image _rightImage;

    private Tween _leftTween, _rightTween;

    private void Reset()
    {
        _rect = GetComponent<RectTransform>();
        _leftRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, "SelectImageLeft");
        _rightRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, "SelectImageRight");
        _leftImage = Helper_Component.FindChildComponent<Image>(this.transform, "SelectImageLeft");
        _rightImage = Helper_Component.FindChildComponent<Image>(this.transform, "SelectImageRight");
    }

    public void ShowImage(Vector2 left, Vector2 right)
    {
        _rect.OpenAtCenter();
        _leftImage.color = Color.white;
        _rightImage.color = Color.white;

        _leftRect.anchoredPosition = left + Vector2.left * 40f;
        _rightRect.anchoredPosition = right + Vector2.right * 40f;
        
        _leftTween = _leftImage.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        _rightTween = _rightImage.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void HideImage()
    {
        _leftTween.Kill();
        _rightTween.Kill();
        _rect.CloseAndRestore();
    }
}