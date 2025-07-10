using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _highlightImage;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    
    private Tween _tween;
    
    public int SlotIndex { get; private set; }
    
    private const string ITEM_IMAGE = "IconImage";
    private const string HIGHLIGHT_IMAGE = "HighlightImage";
    private const string ITEM_AMOUNT_TEXT = "AmountText";
    //private const string DRAGGING_ICON = "DraggingIcon";
    
    private static GameObject _draggingItemIcon;
    private static Item _draggingItem;
    private static ItemSlot _draggingOriginSlot;
    
    private void Reset()
    {
        _itemImage = Helper_Component.FindChildComponent<Image>(this.transform, ITEM_IMAGE);
        _highlightImage = Helper_Component.FindChildComponent<Image>(this.transform, HIGHLIGHT_IMAGE);
        _highlight = Helper_Component.FindChildGameObjectByName(this.gameObject, HIGHLIGHT_IMAGE);
        _itemAmountText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, ITEM_AMOUNT_TEXT);
    }

    public void InitIndex(int index)
    {
        SlotIndex = index;
    }
    
    /// <summary>
    /// 아이템 슬롯의 UI 새로고침 메서드
    /// </summary>
    public void RefreshUI()
    {
        var item = InventoryManager.Instance.Inventory.Items[SlotIndex];

        if (item == null)
        {
            _itemImage.enabled = false;
            _itemImage.sprite = null;
            SetAmount(null);
            return;
        }
        else
        {
            _itemImage.enabled = true;
            
            
            /*_itemImage.sprite = item.Data.icon;*/
            
            
        }
        
        SetAmount(item);
    }

    /// <summary>
    /// 아이템 슬롯의 수량 표시 메서드
    /// </summary>
    private void SetAmount(Item item)
    {
        if (item == null)
        {
            _itemAmountText.text = "";
        }
        else
        {
            if (item.Stackable)
            {
                _itemAmountText.text = item.stack.ToString();
            }
            else
            {
                _itemAmountText.text = "";
            }
        }
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
        
        var item = InventoryManager.Instance.Inventory.Items[SlotIndex];
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
        
        var item = InventoryManager.Instance.Inventory.Items[SlotIndex];
        if (item != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var item = InventoryManager.Instance.Inventory.Items[SlotIndex];
        
        //칸이 비었는지 확인후, 비어있지 않다면 작동.
        if (item == null)
        {
            return;
        }
        
        //드래그가 시작될 때, 해당 아이템 슬롯을 기억하고, 그 슬롯의 Item을 임시 저장.
        _draggingOriginSlot = this;
        _draggingItem = item;
        
        //드래그가 시작될 때, 마우스 포인터에 드래그 될 이미지를 표시.
        _draggingItemIcon = new GameObject("Dragging Icon");
        _draggingItemIcon.transform.SetParent(transform.root);
        var image = _draggingItemIcon.AddComponent<Image>();
        
        
        /*image.sprite = item.Data.icon;*/
        
        
        image.raycastTarget = false;
        _draggingItemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        
        //해당 슬롯을 비워서, 드래그 하는것을 더 명시적으로 보여줌.
        InventoryManager.Instance.Inventory.Items[SlotIndex] = null;
        RefreshUI();
    }

    //나중에 리펙토링 해 볼 예정
    // public void ShowIcon()
    // {
    //     _draggingItemIcon.SetActive(true);
    //     _draggingItemIcon.transform.SetParent(transform.root);
    //     
    //     _draggingItemIcon.Image.sprite = CurrentItem.Data.icon;
    //     _draggingItemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
    // }
    //
    // public void HideIcon()
    // {
    //     _draggingItemIcon.SetActive(false);
    // }
    
    public void OnDrag(PointerEventData eventData)
    {
        //드래그 하는 동안 마우스 위치에 아이템 이미지를 고정 시킴.
        if (_draggingItemIcon != null)
        {
            _draggingItemIcon.transform.position = Input.mousePosition;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        //끌고 다니던 아이템 이미지를 제거.
        if (_draggingItemIcon != null)
        {
            Destroy(_draggingItemIcon);
            _draggingItemIcon = null;
        }
        
        //아이템을 다시 원래 자리로 복구 시킴.
        if (_draggingItem != null && eventData.pointerEnter?.GetComponent<ItemSlot>() == null)
        {
            InventoryManager.Instance.Inventory.Items[_draggingOriginSlot.SlotIndex] = _draggingItem;
            _draggingOriginSlot.RefreshUI();
        }

        _draggingItem = null;
        _draggingOriginSlot = null;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        //드래그 중이던 아이템이 없으면, 작동 X.
        if (_draggingItem == null)
        {
            return;
        }
        
        var inventory = InventoryManager.Instance.Inventory;
        var targetItem = inventory.Items[SlotIndex];

        //해당칸이 비어있다면, 드래그 중인 아이템으로 넣어줌.
        if (targetItem == null)
        {
            if (SlotIndex != _draggingOriginSlot.SlotIndex)
            {
                inventory.Items[SlotIndex] = _draggingItem;
                inventory.Items[_draggingOriginSlot.SlotIndex] = null;
            }
            else
            {
                inventory.Items[SlotIndex] = _draggingItem;
            }
        }
        //해당칸의 아이템이 겹칠 수 있다면, 겹치기.
        else if (targetItem.Data.itemName == _draggingItem.Data.itemName && targetItem.Stackable)
        {
            if (targetItem.IsMaxStack || _draggingItem.IsMaxStack)
            {
                inventory.Items[_draggingOriginSlot.SlotIndex] = targetItem;
                inventory.Items[SlotIndex] = _draggingItem;
            }
            else
            {
                MergeItem(targetItem);
            }
        }
        //해당칸의 아이템이 겹칠 수 없다면, 서로 칸을 바꿔줌.
        else
        {
            inventory.Items[_draggingOriginSlot.SlotIndex] = targetItem;
            inventory.Items[SlotIndex] = _draggingItem;
        }
        
        inventory.InventoryUI.RefreshUI(inventory.Items);
        inventory.QuickSlotInventoryUI.RefreshUI(inventory.Items);

        _draggingItem = null;
        _draggingOriginSlot = null;
    }
    
    /// <summary>
    /// 두 아이템 슬롯의 아이템 수량을 합칠때 사용할 메서드
    /// </summary>
    /// <param name="item"></param>
    private void MergeItem(Item item)
    {
        var max = item.MaxStack;
        var total = _draggingItem.stack + item.stack;

        if (total <= max)
        {
            item.stack = total;
            _draggingItem = null;
        }
        else
        {
            item.stack = max;
            _draggingItem.stack = total - max;
        }
    }

    private void OnDisable()
    {
        _highlight.SetActive(false);
    }
}