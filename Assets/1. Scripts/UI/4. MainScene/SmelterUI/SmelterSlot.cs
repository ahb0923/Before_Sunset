using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SmelterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmount;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private Image _highlightImage;

    private Smelter _currentSmelter;
    private Tween _tween;
    public bool IsInputSlot { get; private set; }

    private const string ITEM_ICON = "IconImage";
    private const string ITEM_AMOUNT = "AmountText";
    private const string HIGHLIGHT_IMAGE = "HighlightImage";
    
    private void Reset()
    {
        _itemImage = Helper_Component.FindChildComponent<Image>(this.transform, ITEM_ICON);
        _itemAmount = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, ITEM_AMOUNT);
        _highlight = Helper_Component.FindChildGameObjectByName(this.gameObject, HIGHLIGHT_IMAGE);
        _highlightImage = Helper_Component.FindChildComponent<Image>(this.transform, HIGHLIGHT_IMAGE);
    }

    /// <summary>
    /// 제련소 슬롯 용도 초기화 메서드
    /// </summary>
    /// <param name="isInputSlot">재료 넣는 칸이라면 true</param>
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
    
    /// <summary>
    /// 제련소 정보를 참조해올때 사용하는 메서드
    /// </summary>
    /// <param name="smelter"></param>
    public void SetSmelterData(Smelter smelter)
    {
        _currentSmelter = smelter;
        RefreshUI();
    }
    
    /// <summary>
    /// 슬롯에 담겨있는 아이템을 인벤토리에 넣어주는 메서드
    /// </summary>
    public void ReceiveItem()
    {
        var item = _currentSmelter.OutputItem;
        
        if (item == null)
            return;
        
        int id = item.Data.id;
        int quantity = item.stack;
        
        var inventory = InventoryManager.Instance.Inventory;
        inventory.AddItem(id, quantity);

        _currentSmelter.SetOutputItem(null);
        RefreshUI();
        inventory.RefreshInventories();
    }

    /// <summary>
    /// 제련소 정보 비워주는 메서드
    /// </summary>
    public void ClearSlot()
    {
        _currentSmelter = null;
    }

    /// <summary>
    /// 슬롯 UI 새로고침 메서드
    /// </summary>
    public void RefreshUI()
    {
        if (_currentSmelter == null)
        {
            return;
        }
        
        if (IsInputSlot && _currentSmelter.InputItem == null)
        {
            _itemImage.enabled = false;
            _itemImage.sprite = null;
            _itemAmount.text = "";
            return;
        }
        else if (!IsInputSlot && _currentSmelter.OutputItem == null)
        {
            _itemImage.enabled = false;
            _itemImage.sprite = null;
            _itemAmount.text = "";
            return;
        }


        if (IsInputSlot && _currentSmelter.InputItem != null)
        {
            _itemImage.enabled = true;
            SetImage(_currentSmelter.InputItem);
            _itemAmount.text = _currentSmelter.InputItem.stack.ToString();
            return;
        }
        else if (!IsInputSlot && _currentSmelter.OutputItem != null)
        {
            _itemImage.enabled = true;
            SetImage(_currentSmelter.OutputItem);
            _itemAmount.text = _currentSmelter.OutputItem.stack.ToString();
            return;
        }
    }

    /// <summary>
    /// 슬롯 UI 의 재료칸 이미지 설정 메서드
    /// </summary>
    /// <param name="item"></param>
    private void SetImage(Item item)
    {
        if (item == null)
        {
            return;
        }
        
        if (item.Data.id >= 100 && item.Data.id < 200)
            _itemImage.sprite = DataManager.Instance.MineralData.GetSpriteById(item.Data.id);
        else if (item.Data.id >= 200 && item.Data.id < 300)
            _itemImage.sprite = DataManager.Instance.JewelData.GetSpriteById(item.Data.id);
    }

    /// <summary>
    /// 제련소 슬롯에 넣을수 있는 아이템인지 체크하는 메서드
    /// </summary>
    /// <param name="item"></param>
    /// <returns>가능하다면 true 반환</returns>
    public bool CanInputItem(Item item)
    {
        if (item == null)
        {
            return true;
        }
        
        var canInputItems = _currentSmelter.smelterData.smeltingIdList;
        
        for (int i = 0; i < canInputItems.Count; i++)
        {
            if (canInputItems[i] == item.Data.id)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 제련소의 아이템 설정 메서드
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        _currentSmelter.SetInputItem(item);
    }

    private void OnDisable()
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
        _highlightImage.color = new Color(1f, 1f, 0f, 0f);
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

        if (IsInputSlot)
        {
            if (_currentSmelter.InputItem != null)
                TooltipManager.Instance.ShowTooltip(
                    _currentSmelter.InputItem.Data.itemName, _currentSmelter.InputItem.Data.context);
        }
        else
        {
            if (_currentSmelter.OutputItem != null)
                TooltipManager.Instance.ShowTooltip(
                    _currentSmelter.OutputItem.Data.itemName, _currentSmelter.OutputItem.Data.context);
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
        if (!IsInputSlot)
            return;

        var item = _currentSmelter.InputItem;
        if (item == null)
        {
            return;
        }
        
        DragManager.OriginSmelterSlot = this;
        DragManager.DraggingItem = item;
        
        DragManager.DraggingIcon = new GameObject("Dragging Icon");
        DragManager.DraggingIcon.transform.SetParent(transform.root);
        var image = DragManager.DraggingIcon.AddComponent<Image>();
        
        if (item.Data.id >= 100 && item.Data.id < 200)
            image.sprite = DataManager.Instance.MineralData.GetSpriteById(item.Data.id);
        else if (item.Data.id >= 200 && item.Data.id < 300)
            image.sprite = DataManager.Instance.JewelData.GetSpriteById(item.Data.id);
        
        image.raycastTarget = false;
        DragManager.DraggingIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        
        _currentSmelter.SetInputItem(null);
        RefreshUI();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragManager.DraggingIcon != null)
        {
            Vector2 pos = new Vector2(-10, 10);
            DragManager.DraggingIcon.transform.position = (Vector2)Input.mousePosition + pos;
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
                _currentSmelter.SetInputItem(DragManager.DraggingItem);
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
        var smelterItem = _currentSmelter.InputItem;
        
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
            if (smelterItem == null)
            {
                _currentSmelter.SetInputItem(DragManager.DraggingItem);
            }
            //해당칸의 아이템이 겹칠 수 있다면, 겹치기.
            else if (smelterItem.Data.itemName == DragManager.DraggingItem.Data.itemName && smelterItem.Stackable)
            {
                if (smelterItem.IsMaxStack || DragManager.DraggingItem.IsMaxStack)
                {
                    inventory.Items[DragManager.OriginItemSlot.SlotIndex] = smelterItem;
                    _currentSmelter.SetInputItem(DragManager.DraggingItem);
                }
                else
                {
                    MergeItem(smelterItem);
                    if (DragManager.DraggingItem != null)
                    {
                        inventory.Items[DragManager.OriginItemSlot.SlotIndex] = DragManager.DraggingItem;
                    }
                }
            }
            //해당칸의 아이템이 겹칠 수 없다면, 서로 칸을 바꿔줌.
            else
            {
                inventory.Items[DragManager.OriginItemSlot.SlotIndex] = smelterItem;
                _currentSmelter.SetInputItem(DragManager.DraggingItem);
            }
        }
        else if (DragManager.OriginSmelterSlot != null)
        {
            if (IsInputSlot == false)
            {
                DragManager.OriginSmelterSlot.SetItem(DragManager.DraggingItem);
                
                RefreshUI();
                DragManager.OriginSmelterSlot.RefreshUI();
        
                DragManager.Clear();
                return;
            }
            
            if (smelterItem == null)
            {
                _currentSmelter.SetInputItem(DragManager.DraggingItem);
            }
        }


        inventory.InventoryUI.RefreshUI(inventory.Items);
        inventory.QuickSlotInventoryUI.RefreshUI(inventory.Items);
        RefreshUI();
        
        if (DragManager.DraggingItem != null)
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
            _currentSmelter.SetInputItem(item);
            DragManager.DraggingItem = null;
        }
        else
        {
            item.stack = max;
            _currentSmelter.SetInputItem(item);
            DragManager.DraggingItem.stack = total - max;
        }
    }
}
