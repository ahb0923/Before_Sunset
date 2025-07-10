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
    
    private float _highlightDuration = 0.1f;
    private float _highlightAlpha = 0.4f;
    
    public Coroutine highlightCoroutine;
    
    private const string ITEM_IMAGE = "IconImage";
    private const string HIGHLIGHT_IMAGE = "HighlightImage";
    //private const string DRAGGING_ICON = "DraggingIcon";

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        
        _highlightPickaxeSlot = this;
        highlightCoroutine = StartCoroutine(C_HighlightFadeIn());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }

        _highlightPickaxeSlot = null;
        highlightCoroutine = StartCoroutine(C_HighlightFadeOut());
    }

    private IEnumerator C_HighlightFadeIn()
    {
        _highlight.SetActive(true);

        float time = 0f;
        
        Color color = Color.yellow;
        color.a = 0f;
        _highlightImage.color = color;

        while (time < _highlightDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _highlightDuration);
            color.a = Mathf.Lerp(0f, _highlightAlpha, t);
            _highlightImage.color = color;
            yield return null;
        }
        
        color.a = _highlightAlpha;
        _highlightImage.color = color;
    }

    private IEnumerator C_HighlightFadeOut()
    {
        float time = 0f;
        
        Color color = Color.yellow;
        color.a = _highlightAlpha;
        _highlightImage.color = color;

        while (time < _highlightDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _highlightDuration);
            color.a = Mathf.Lerp(_highlightAlpha, 0f, t);
            _highlightImage.color = color;
            yield return null;
        }
        
        color.a = 0f;
        _highlightImage.color = color;
        
        _highlight.SetActive(false);
    }
    
    private void OnDisable()
    {
        _highlight.SetActive(false);
    }
}