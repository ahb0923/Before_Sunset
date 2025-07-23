using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickaxeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _highlightImage;
    [SerializeField] private GameObject _highlight;

    private Tween _tween;
    private const string ITEM_IMAGE = "IconImage";
    private const string HIGHLIGHT_IMAGE = "HighlightImage";

    private static PickaxeSlot _highlightPickaxeSlot;
    
    private void Reset()
    {
        _itemImage = Helper_Component.FindChildComponent<Image>(this.transform, ITEM_IMAGE);
        _highlightImage = Helper_Component.FindChildComponent<Image>(this.transform, HIGHLIGHT_IMAGE);
        _highlight = Helper_Component.FindChildGameObjectByName(this.gameObject, HIGHLIGHT_IMAGE);
    }
    
    /// <summary>
    /// 아이템 슬롯의 UI 새로고침 메서드
    /// </summary>
    public void RefreshUI()
    {
        var item = InventoryManager.Instance.Inventory.Pickaxe;
        if (item == null)
        {
            _itemImage.enabled = false;
            _itemImage.sprite = null;
            return;
        }
        else
        {
            _itemImage.enabled = true;
            
            
            /*_itemImage.sprite = item.Data.icon;*/
            
            
        }
    }

    public void DisableHighlight()
    {
        _highlight.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        
        _highlight.SetActive(true);
        _highlightImage.color = new Color(1f, 1f, 0f, 0f);

        _tween = _highlightImage.DOFade(0.3f, 0.2f);
        
        var item = InventoryManager.Instance.Inventory.Pickaxe;
        if (item != null)
        {
            TooltipManager.Instance.ShowTooltip(item.Data.itemName, item.Data.context);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }

        _tween = _highlightImage.DOFade(0f, 0.2f).OnComplete(() => _highlight.SetActive(false));
        
        TooltipManager.Instance.HideTooltip();
    }
}