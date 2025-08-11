using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemToastText;
    [SerializeField] private RectTransform _itemToastRect;
    
    [SerializeField] private float _showDuration = 1f;
    [SerializeField] private float _fadeDuration = 0.5f;
    
    public Sequence fadeSequence;

    private void Reset()
    {
        _itemToastText = GetComponent<TextMeshProUGUI>();
        _itemToastRect = GetComponent<RectTransform>();
    }

    public void ShowToast(string itemName)
    {
        InitToast();
        _itemToastText.text = $"{itemName} 을(를) 획득 하였습니다";
        LayoutRebuilder.ForceRebuildLayoutImmediate(_itemToastRect);
        
        fadeSequence = DOTween.Sequence();
        fadeSequence.AppendInterval(_showDuration);
        fadeSequence.Append(_itemToastText.DOFade(0f, _fadeDuration).OnComplete(() =>
        {
            fadeSequence.Kill();
            fadeSequence = null;
            UIManager.Instance.ItemToastUI.ReturnToast(this.gameObject);
        }));
    }

    private void InitToast()
    {
        if (fadeSequence != null)
        {
            fadeSequence.Kill();
            fadeSequence = null;
        }
        gameObject.transform.SetAsLastSibling();
        _itemToastText.alpha = 1f;
    }
}
