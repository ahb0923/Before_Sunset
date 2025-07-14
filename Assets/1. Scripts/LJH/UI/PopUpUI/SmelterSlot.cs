using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEditor;

public class SmelterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemAmount;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private Image _highlightImage;

    private SmelterDatabase _currentData;
    private Tween _tween;
    public bool IsInputSlot { get; private set; }
    public Item CurrentItem { get; private set; }

    private const string ITEM_ICON = "IconImage";
    private const string ITEM_AMOUNT = "AmountText";
    private const string HIGHLIGHT_IMAGE = "HighlightImage";
    
    private void Reset()
    {
        _itemIcon = Helper_Component.FindChildComponent<Image>(this.transform, ITEM_ICON);
        _itemAmount = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, ITEM_AMOUNT);
        _highlight = Helper_Component.FindChildGameObjectByName(this.gameObject, HIGHLIGHT_IMAGE);
        _highlightImage = Helper_Component.FindChildComponent<Image>(this.transform, HIGHLIGHT_IMAGE);
    }

    private void Awake()
    {
        ClearSlot();
    }

    public void InitInputSlot(bool isInputSlot)
    {
        if (isInputSlot)
        {
            IsInputSlot = true;
        }
        else
        {
            IsInputSlot = false;
        }
    }
    
    public void SetSmelterData(SmelterDatabase data)
    {
        _currentData = data;
    }

    /// <summary>
    /// 제련소 널체크 메서드
    /// </summary>
    /// <returns></returns>
    public bool IsOutputEmpty()
    {
        if (CurrentItem == null)
            return true;
        else return false;
    }
    
    /// <summary>
    /// 제련된 아이템을 제련소 슬롯에 넣어줄때 사용 메서드
    /// </summary>
    /// <param name="item">만들어진 주괴</param>
    /// <param name="amount">만들어진 수량</param>
    public void AddToSmelterItem(Item item, int amount)
    {
        if (CurrentItem == null)
        {
            CurrentItem = item;
            CurrentItem.stack += amount;
        }
        else
        {
            CurrentItem.stack += amount;
        }
    }

    /// <summary>
    /// 슬롯에 담겨있는 아이템을 인벤토리에 넣어주는 메서드
    /// </summary>
    public void ReceiveItem()
    {
        if (CurrentItem == null)
        {
            return;
        }
        
        int id = CurrentItem.Data.id;
        int quantity = CurrentItem.stack;
        
        var inventory = InventoryManager.Instance.Inventory;
        inventory.AddItem(id, quantity);
        
        CurrentItem = null;
        RefreshUI();
        inventory.RefreshInventories();
    }

    public void ClearSlot()
    {
        CurrentItem = null;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (CurrentItem == null)
        {
            _itemIcon.sprite = null;
            _itemAmount.text = "";
        }
        else
        {
            // _itemIcon.sprite = CurrentItem.Data.sprite;
            _itemAmount.text = CurrentItem.stack.ToString();
        }
    }

    public bool CanInputItem(Item item)
    {
        if (item == null)
        {
            return true;
        }
        
        var canInputItems = _currentData.smeltingIdList;
        
        for (int i = 0; i < _currentData.smeltingIdList.Count; i++)
        {
            if (canInputItems[i] == item.Data.id)
            {
                return true;
            }
        }
        return false;
    }

    public void SetItem(Item item)
    {
        CurrentItem = item;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"PointerEnter Object: {eventData.pointerEnter.name}");
        if (_tween != null)
        {
            _tween.Kill();
        }
        
        _highlight.SetActive(true);
        _highlightImage.color = new Color(1f, 1f, 0f, 0f);

        _tween = _highlightImage.DOFade(0.3f, 0.2f);
        
        if (CurrentItem != null)
        {
            TooltipManager.Instance.ShowTooltip(CurrentItem.Data.itemName, CurrentItem.Data.context);
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

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurrentItem == null)
        {
            return;
        }
        
        DragManager.OriginSmelterSlot = this;
        DragManager.DraggingItem = CurrentItem;
        
        DragManager.DraggingIcon = new GameObject("Dragging Icon");
        DragManager.DraggingIcon.transform.SetParent(transform.root);
        var image = DragManager.DraggingIcon.AddComponent<Image>();
        
        /*image.sprite = item.Data.icon;*/
        
        image.raycastTarget = false;
        DragManager.DraggingIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        
        CurrentItem = null;
        RefreshUI();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragManager.DraggingIcon != null)
        {
            DragManager.DraggingIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragManager.DraggingIcon != null)
        {
            Destroy(DragManager.DraggingIcon);
            DragManager.DraggingIcon = null;
        }

        if (DragManager.DraggingItem != null)
            if (eventData.pointerEnter?.GetComponentInParent<ItemSlot>() == null
                && eventData.pointerEnter?.GetComponentInParent<SmelterSlot>() == null)
            {
                CurrentItem = DragManager.DraggingItem;
                DragManager.OriginSmelterSlot.RefreshUI();
            }

        DragManager.DraggingItem = null;
        DragManager.OriginSmelterSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragManager.DraggingItem == null)
        {
            return;
        }
        
        var inventory = InventoryManager.Instance.Inventory;
        
        if (!CanInputItem(DragManager.DraggingItem) || IsInputSlot == false)
        {
            if (DragManager.OriginItemSlot != null)
            {
                inventory.Items[DragManager.OriginItemSlot.SlotIndex] = DragManager.DraggingItem;
                
                inventory.InventoryUI.RefreshUI(inventory.Items);
                inventory.QuickSlotInventoryUI.RefreshUI(inventory.Items);
                RefreshUI();

                DragManager.Clear();
                return;
            }
        }

        // 드래그 중인 아이템이 아이템슬롯에서 왔을 경우
        if (DragManager.OriginItemSlot != null)
        {
            //해당칸이 비어있다면, 드래그 중인 아이템으로 넣어줌.
            if (CurrentItem == null)
            {
                CurrentItem = DragManager.DraggingItem;
            }
            //해당칸의 아이템이 겹칠 수 있다면, 겹치기.
            else if (CurrentItem.Data.itemName == DragManager.DraggingItem.Data.itemName && CurrentItem.Stackable)
            {
                if (CurrentItem.IsMaxStack || DragManager.DraggingItem.IsMaxStack)
                {
                    inventory.Items[DragManager.OriginItemSlot.SlotIndex] = CurrentItem;
                    CurrentItem = DragManager.DraggingItem;
                }
                else
                {
                    MergeItem(CurrentItem);
                    if (DragManager.DraggingItem != null)
                    {
                        inventory.Items[DragManager.OriginItemSlot.SlotIndex] = DragManager.DraggingItem;
                    }
                }
            }
            //해당칸의 아이템이 겹칠 수 없다면, 서로 칸을 바꿔줌.
            else
            {
                inventory.Items[DragManager.OriginItemSlot.SlotIndex] = CurrentItem;
                CurrentItem = DragManager.DraggingItem;
            }
        }
        else if (DragManager.OriginSmelterSlot != null)
        {
            if (IsInputSlot == false)
            {
                CurrentItem = null;
                DragManager.OriginSmelterSlot.SetItem(DragManager.DraggingItem);
                
                RefreshUI();
                DragManager.OriginSmelterSlot.RefreshUI();
        
                DragManager.Clear();
                return;
            }
            
            if (CurrentItem == null)
            {
                CurrentItem = DragManager.DraggingItem;
            }
        }


        inventory.InventoryUI.RefreshUI(inventory.Items);
        inventory.QuickSlotInventoryUI.RefreshUI(inventory.Items);
        RefreshUI();
        
        TooltipManager.Instance.UpdateTooltip(DragManager.DraggingItem.Data.itemName, DragManager.DraggingItem.Data.context);

        DragManager.Clear();
    }
    
    /// <summary>
    /// 두 아이템 슬롯의 아이템 수량을 합칠때 사용할 메서드
    /// </summary>
    /// <param name="item"></param>
    private void MergeItem(Item item)
    {
        var max = item.MaxStack;
        var total = DragManager.DraggingItem.stack + item.stack;

        if (total <= max)
        {
            item.stack = total;
            DragManager.DraggingItem = null;
        }
        else
        {
            item.stack = max;
            DragManager.DraggingItem.stack = total - max;
        }
    }
}
