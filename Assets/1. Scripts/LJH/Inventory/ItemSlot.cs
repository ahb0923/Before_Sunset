using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    
    private const string ITEM_IMAGE = "IconImage";
    private const string ITEM_AMOUNT_TEXT = "AmountText";
    
    private static GameObject _draggingItemIcon;
    private static Item _draggingItem;
    private static ItemSlot _draggingOriginSlot;
    
    [field: SerializeField] public bool IsEmpty { get; private set; } = true;
    public Item CurrentItem { get; private set; }

    private void Reset()
    {
        _itemImage = UtilityLJH.FindChildComponent<Image>(this.transform, ITEM_IMAGE);
        _itemAmountText = UtilityLJH.FindChildComponent<TextMeshProUGUI>(this.transform, ITEM_AMOUNT_TEXT);
    }

    public bool CanStack(Item item)
    {
        return !IsEmpty && CurrentItem.Data.itemName == item.Data.itemName && !CurrentItem.IsMaxStack;
    }

    public void StackItem(Item item)
    {
        CurrentItem.stack += item.Data.quantity;

        if (CurrentItem.stack > CurrentItem.Data.maxStack)
        {
            var left = CurrentItem.stack - CurrentItem.Data.maxStack;
            CurrentItem.stack = CurrentItem.Data.maxStack;

            item.stack = left;
            //MainInventory.AddItem(item);
        }

        UpdateUI();
    }
    
    public bool CanMerge(Item item)
    {
        return !IsEmpty && CurrentItem.Data.itemName == item.Data.itemName && !item.IsMaxStack;
    }

    public void MergeItem(Item item)
    {
        CurrentItem.stack += item.stack;

        if (CurrentItem.IsMaxStack)
        {
            var left = CurrentItem.stack - CurrentItem.Data.maxStack;
            CurrentItem.stack = CurrentItem.Data.maxStack;
            
            item.stack = left;
            
            _draggingOriginSlot.SetItem(item);
        }
        else
        {
            _draggingOriginSlot.SetItem(null);
        }
        
        UpdateUI();
    }

    public void SetItem(Item item)
    {
        CurrentItem = item;

        if (CurrentItem.Data.stackable)
        {
            CurrentItem.stack += item.Data.quantity;
        }
        
        IsEmpty = item == null;
        UpdateUI();
    }

    public void UpdateUI()
    {
        _itemImage.sprite = IsEmpty ? null : CurrentItem.Data.icon;
        _itemImage.enabled = !IsEmpty;
        SetAmount();
    }

    private void SetAmount()
    {
        if (CurrentItem == null)
        {
            _itemAmountText.text = "";
            return;
        }
        
        if (CurrentItem.Data.stackable)
        {
            _itemAmountText.text = CurrentItem.stack.ToString();
        }
        else
        {
            _itemAmountText.text = "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //칸이 비었는지 확인후, 비어있지 않다면 작동.
        if (IsEmpty)
        {
            return;
        }
        
        //드래그가 시작될 때, 해당 아이템 슬롯을 기억하고, 그 슬롯의 Item을 임시 저장.
        _draggingOriginSlot = this;
        _draggingItem = CurrentItem;
        
        //드래그가 시작될 때, 마우스 포인터에 드래그 될 이미지를 표시.
        _draggingItemIcon = new GameObject("Dragging Icon");
        _draggingItemIcon.transform.SetParent(transform.root);
        Image image = _draggingItemIcon.AddComponent<Image>();
        image.sprite = CurrentItem.Data.icon;
        image.raycastTarget = false;
        _draggingItemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        
        //해당 슬롯을 비워서, 드래그 하는것을 더 명시적으로 보여줌.
        SetItem(null);
    }
    
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
        
        //일시적으로 아이템을 다시 원래 자리로 복구 시킴.
        if (_draggingItem != null && eventData.pointerEnter?.GetComponent<ItemSlot>() == null)
        {
            _draggingOriginSlot.SetItem(_draggingItem);
        }
        
        // _draggingItem = null;
        // _draggingOriginSlot = null;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        //드래그 중이던 아이템이 없으면, 작동 X.
        if (_draggingItem == null)
        {
            return;
        }

        //해당칸이 비어있다면, 드래그 중인 아이템으로 넣어줌.
        if (IsEmpty)
        {
            SetItem(_draggingItem);
        }
        //해당칸의 아이템이 겹칠 수 있다면, 겹치기.
        else if (CanMerge(_draggingItem))
        {
            MergeItem(_draggingItem);
        }
        //해당칸의 아이템이 겹칠 수 없다면, 서로 칸을 바꿔줌.
        else
        {
            Item tempItem = CurrentItem;
            SetItem(_draggingItem);

            _draggingOriginSlot.SetItem(tempItem);
        }

        _draggingItem = null;
        _draggingOriginSlot = null;
    }
}
