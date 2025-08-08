using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EssenceUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _image1;
    [SerializeField] private Image _image2;
    [SerializeField] private Image _image3;
    [SerializeField] private Image _image4;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    private const string ESSENCE_PROGRESS_IMAGE1 = "EssenceProgressImage1";
    private const string ESSENCE_PROGRESS_IMAGE2 = "EssenceProgressImage2";
    private const string ESSENCE_PROGRESS_IMAGE3 = "EssenceProgressImage3";
    private const string ESSENCE_PROGRESS_IMAGE4 = "EssenceProgressImage4";
    private const string ESSENCE_AMOUNT_TEXT = "EssenceAmountText";

    private void Reset()
    {
        _image1 = Helper_Component.FindChildComponent<Image>(this.transform, ESSENCE_PROGRESS_IMAGE1);
        _image2 = Helper_Component.FindChildComponent<Image>(this.transform, ESSENCE_PROGRESS_IMAGE2);
        _image3 = Helper_Component.FindChildComponent<Image>(this.transform, ESSENCE_PROGRESS_IMAGE3);
        _image4 = Helper_Component.FindChildComponent<Image>(this.transform, ESSENCE_PROGRESS_IMAGE4);
        _amountText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, ESSENCE_AMOUNT_TEXT);
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        var piece = UpgradeManager.Instance.EssencePiece;
        if (piece is >= 0 and < 7)
        {
            DisableImage();
            _image1.enabled = true;
        }
        else if (piece is >= 7 and < 15)
        {
            DisableImage();
            _image2.enabled = true;
        }
        else if (piece is >= 15 and < 22)
        {
            DisableImage();
            _image3.enabled = true;
        }
        else if (piece is >= 22 and < 30)
        {
            DisableImage();
            _image4.enabled = true;
        }
        
        _amountText.text = FormatNumber(UpgradeManager.Instance.Essence);
    }

    private void DisableImage()
    {
        _image1.enabled = false;
        _image2.enabled = false;
        _image3.enabled = false;
        _image4.enabled = false;
    }

    private string FormatNumber(int num)
    {
        if (num >= 100)
            return "x 99+";
        else
            return "x " + num.ToString("D2");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var piece = UpgradeManager.Instance.EssencePiece;
        string amount = $"{piece} / 30";
        
        TooltipManager.Instance.ShowTooltip("정수조각", amount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}