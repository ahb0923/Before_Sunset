using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ToastUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _toastText;
    [SerializeField] private RectTransform _toastRect;
    
    [SerializeField] private float _delay = 0.5f;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _moveDistance = 50f;

    private RectTransform _rect;
    private Vector2 _originalPosition;
    private float _goalPosition;
    private Coroutine _toastCoroutine;
    private Tween _fadeTween;
    private Tween _moveTween;

    private const string TOAST_TEXT = "ToastText";

    private void Reset()
    {
        _toastText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TOAST_TEXT);
        _toastRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, TOAST_TEXT);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _originalPosition = _toastRect.anchoredPosition;
        _goalPosition = _originalPosition.y + _moveDistance;
    }

    public void ShowToast(string toastText)
    {
        Open();
        
        _fadeTween?.Kill();
        _moveTween?.Kill();
        if (_toastCoroutine != null)
            StopCoroutine(_toastCoroutine);
        
        _toastText.text = toastText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_toastRect);
        _toastText.alpha = 1f;
        _toastRect.anchoredPosition = _originalPosition;

        _toastCoroutine = StartCoroutine(C_ShowToast());
    }

    private IEnumerator C_ShowToast()
    {
        yield return Helper_Coroutine.WaitSeconds(_delay);
        
        _moveTween = _toastRect.DOAnchorPosY(_goalPosition, _fadeDuration).SetEase(Ease.OutCubic);
        _fadeTween = _toastText.DOFade(0f, _fadeDuration);

        yield return _fadeTween.WaitForCompletion();

        _toastCoroutine = null;
        Close();
        ToastManager.Instance.ReturnToast(this.gameObject);
    }

    private void Open()
    {
        _rect.OpenAtCenter();
    }

    private void Close()
    {
        _rect.CloseAndRestore();
    }
}
