using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastUI : MonoBehaviour
{
    [Header("토스트 UI")]
    [SerializeField] private Image _toastImage;
    [SerializeField] private RectTransform _toastBGRect;
    [SerializeField] private TextMeshProUGUI _toastText;
    [SerializeField] private RectTransform _toastRect;
    
    [Header("토스트 설정")]
    [SerializeField] private float _delay = 0.5f;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _moveDistance = 50f;

    private RectTransform _rect;
    private Vector2 _originalPosition;
    private float _goalPosition;
    private Sequence _sequence;
    
    private const string TOAST_BG = "ToastBG";
    private const string TOAST_TEXT = "ToastText";

    private void Reset()
    {
        _toastImage = Helper_Component.FindChildComponent<Image>(this.transform, TOAST_BG);
        _toastBGRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, TOAST_BG);
        _toastText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TOAST_TEXT);
        _toastRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, TOAST_TEXT);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _originalPosition = _toastBGRect.anchoredPosition;
        _goalPosition = _originalPosition.y + _moveDistance;
    }

    public void ShowToast(string toastText)
    {
        Open();
        
        _sequence?.Kill();
        _sequence = null;
        
        _toastText.text = toastText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_toastRect);
        
        var imgColor = _toastImage.color;
        imgColor.a = 0.7f;
        _toastImage.color = imgColor;
        _toastText.alpha = 1f;
        
        _toastBGRect.anchoredPosition = _originalPosition;
        
        _sequence = DOTween.Sequence();
        _sequence.SetUpdate(true);
        _sequence.AppendInterval(_delay);
        _sequence.Append(_toastText.DOFade(0f, _fadeDuration));
        _sequence.Join(_toastImage.DOFade(0f, _fadeDuration));
        _sequence.Join(_toastBGRect.DOAnchorPosY(_goalPosition, _fadeDuration).SetEase(Ease.OutCubic).OnComplete(OnToastComplete));
    }

    private void OnToastComplete()
    {
        _sequence?.Kill();
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
