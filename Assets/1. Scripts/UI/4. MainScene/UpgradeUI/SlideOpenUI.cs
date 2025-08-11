using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlideOpenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _toggleButton;
    [SerializeField] private RectTransform _toggleButtonRectTransform;
    [SerializeField] private GameObject _upgradeContainer;
    [SerializeField] private LayoutElement _layoutElement;

    [Header("설정")]
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _rotationDuration = 0.5f;
    [SerializeField] private float _slotHeight = 120f;
    [SerializeField] private float _space = 10f;

    private bool _isOpen = false;
    private Tween _windowTween;
    private Tween _buttonTween;
    
    private const string TOGGLE_BUTTON = "UpgradeBar";
    private const string TOGGLE_RECT = "ToggleImage";
    private const string UPGRADE_CONTAINER = "UpgradeContainer";

    private void Reset()
    {
        _toggleButton = Helper_Component.FindChildComponent<Button>(this.transform, TOGGLE_BUTTON);
        _toggleButtonRectTransform = Helper_Component.FindChildComponent<RectTransform>(this.transform, TOGGLE_RECT);
        _upgradeContainer = Helper_Component.FindChildGameObjectByName(this.gameObject, UPGRADE_CONTAINER);
        _layoutElement = GetComponent<LayoutElement>();
    }

    private void Start()
    {
        _layoutElement.preferredHeight = 0;
        _upgradeContainer.SetActive(false);
        _toggleButton.onClick.AddListener(ToggleCategory);
    }

    private void ToggleCategory()
    {
        if (_windowTween != null && _windowTween.IsActive())
        {
            _windowTween.Kill();
            _buttonTween.Kill();
        }

        _isOpen = !_isOpen;

        if (_isOpen)
        {
            _upgradeContainer.SetActive(true);

            float targetHeight = GetTargetHeight();

            _windowTween = DOTween.To(
                () => _layoutElement.preferredHeight,
                value => _layoutElement.preferredHeight = value,
                targetHeight,
                _animationDuration
            ).SetEase(Ease.OutCubic).SetUpdate(true);
        }
        else
        {
            _windowTween = DOTween.To(
                    () => _layoutElement.preferredHeight,
                    value => _layoutElement.preferredHeight = value,
                    _layoutElement.minHeight,
                    _animationDuration
                ).SetEase(Ease.InCubic).SetUpdate(true)
                .OnComplete(() =>
                {
                    _upgradeContainer.SetActive(false);
                });
        }

        float targetRotation = _isOpen ? 180f : 0f;
        
        _buttonTween = _toggleButtonRectTransform.DORotate(new Vector3(0, 0, targetRotation), _rotationDuration)
            .SetEase(Ease.OutCubic).SetUpdate(true);
    }

    private float GetTargetHeight()
    {
        int childCount = _upgradeContainer.transform.childCount;
        return childCount * _slotHeight + _layoutElement.minHeight + _space;
    }
}
