using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlideOpenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _toggleButton;
    [SerializeField] private GameObject _upgradeContainer;
    [SerializeField] private LayoutElement _layoutElement;

    [Header("설정")]
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private float _slotHeight = 100f;

    private bool _isOpen = false;
    private Tween _currentTween;
    
    private const string TOGGLE_BUTTON = "ToggleButton";
    private const string UPGRADE_CONTAINER = "UpgradeContainer";

    private void Reset()
    {
        _toggleButton = Helper_Component.FindChildComponent<Button>(this.transform, TOGGLE_BUTTON);
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
        if (_currentTween != null && _currentTween.IsActive())
            _currentTween.Kill();

        _isOpen = !_isOpen;

        if (_isOpen)
        {
            _upgradeContainer.SetActive(true);

            float targetHeight = GetTargetHeight();

            _currentTween = DOTween.To(
                () => _layoutElement.preferredHeight,
                value => _layoutElement.preferredHeight = value,
                targetHeight,
                _animationDuration
            ).SetEase(Ease.OutCubic);
        }
        else
        {
            float currentHeight = _layoutElement.preferredHeight;

            _currentTween = DOTween.To(
                    () => _layoutElement.preferredHeight,
                    value => _layoutElement.preferredHeight = value,
                    0,
                    _animationDuration
                ).SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    _upgradeContainer.SetActive(false);
                });
        }
    }

    private float GetTargetHeight()
    {
        int childCount = _upgradeContainer.transform.childCount;
        return childCount * _slotHeight;
    }
}
