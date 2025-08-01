using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private RectTransform _itemRect;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _highlightImage;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    
    [SerializeField] private float _scaleAmount = 1.2f;
    [SerializeField] private float _scaleDuration = 0.3f;
    private Tween _tween;
    private Sequence _sequence;
    
    public int SlotIndex { get; private set; }
    
    private const string ITEM_IMAGE = "IconImage";
    private const string HIGHLIGHT_IMAGE = "HighlightImage";
    private const string ITEM_AMOUNT_TEXT = "AmountText";

    private void Reset()
    {
        _itemRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, ITEM_IMAGE);
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
            SetImage(item);
        }
        
        SetAmount(item);
    }

    public void DisableHighlight()
    {
        _highlight.SetActive(false);
    }

    public void AddAnimation()
    {
        if (_sequence != null)
            return;
        
        if (this.gameObject.activeInHierarchy)
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_itemRect.DOScale(_scaleAmount, _scaleDuration).SetEase(Ease.InCubic));
            _sequence.Append(_itemRect.DOScale(1f, _scaleDuration).SetEase(Ease.OutCubic)).
                OnComplete(() =>
                {
                    _sequence.Kill();
                    _sequence = null;
                });
        }
    }

    private void OnDisable()
    {
        if (_sequence != null)
        {
            _sequence.Kill();
            _sequence = null;
        }
    }

    private void SetImage(Item item)
    {
        if (item.Data.id >= 100 && item.Data.id < 200)
            _itemImage.sprite = DataManager.Instance.MineralData.GetSpriteById(item.Data.id);
        else if (item.Data.id >= 200 && item.Data.id < 300)
            _itemImage.sprite = DataManager.Instance.JewelData.GetSpriteById(item.Data.id);
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
        
        TooltipManager.Instance.HideTooltip();
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
        DragManager.OriginItemSlot = this;
        DragManager.DraggingItem = item;
        
        //드래그가 시작될 때, 마우스 포인터에 드래그 될 이미지를 표시.
        DragManager.DraggingIcon = new GameObject("Dragging Icon");
        DragManager.DraggingIcon.transform.SetParent(transform.root);
        var image = DragManager.DraggingIcon.AddComponent<Image>();
        
        if (item.Data.id >= 100 && item.Data.id < 200)
            image.sprite = DataManager.Instance.MineralData.GetSpriteById(item.Data.id);
        else if (item.Data.id >= 200 && item.Data.id < 300)
            image.sprite = DataManager.Instance.JewelData.GetSpriteById(item.Data.id);
        
        image.raycastTarget = false;
        DragManager.DraggingIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        
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
        if (DragManager.DraggingIcon != null)
        {
            Vector2 pos = new Vector2(-10, 10);
            DragManager.DraggingIcon.transform.position = (Vector2)Input.mousePosition + pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //끌고 다니던 아이템 이미지를 제거.
        if (DragManager.DraggingIcon != null)
        {
            Destroy(DragManager.DraggingIcon);
            DragManager.DraggingIcon = null;
        }

        //아이템을 다시 원래 자리로 복구 시킴.
        if (DragManager.DraggingItem != null)
        {
            if (eventData.pointerEnter?.GetComponentInParent<ItemSlot>() == null
                && eventData.pointerEnter?.GetComponentInParent<SmelterSlot>() == null)
            {
                InventoryManager.Instance.Inventory.Items[DragManager.OriginItemSlot.SlotIndex] = DragManager.DraggingItem;
                DragManager.OriginItemSlot.RefreshUI();
            }
        }

        DragManager.Clear();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        //드래그 중이던 아이템이 없으면, 작동 X.
        if (DragManager.DraggingItem == null)
        {
            return;
        }
        
        var inventory = InventoryManager.Instance.Inventory;
        var targetItem = inventory.Items[SlotIndex];
        var smelterSlot = UIManager.Instance.SmelterUI.smelterInputSlot;
        
        // 드래그 중인 아이템이 아이템슬롯에서 왔을 경우
        if (DragManager.OriginItemSlot != null)
        {
            //해당칸이 비어있다면, 드래그 중인 아이템으로 넣어줌.
            if (targetItem == null)
            {
                if (SlotIndex != DragManager.OriginItemSlot.SlotIndex)
                {
                    inventory.Items[SlotIndex] = DragManager.DraggingItem;
                    inventory.Items[DragManager.OriginItemSlot.SlotIndex] = null;
                }
                else
                {
                    inventory.Items[SlotIndex] = DragManager.DraggingItem;
                }
            }
            //해당칸의 아이템이 겹칠 수 있다면, 겹치기.
            else if (targetItem.Data.itemName == DragManager.DraggingItem.Data.itemName && targetItem.Stackable)
            {
                if (targetItem.IsMaxStack || DragManager.DraggingItem.IsMaxStack)
                {
                    inventory.Items[DragManager.OriginItemSlot.SlotIndex] = targetItem;
                    inventory.Items[SlotIndex] = DragManager.DraggingItem;
                }
                else
                {
                    MergeItem(targetItem);
                }
            }
            //해당칸의 아이템이 겹칠 수 없다면, 서로 칸을 바꿔줌.
            else
            {
                inventory.Items[DragManager.OriginItemSlot.SlotIndex] = targetItem;
                inventory.Items[SlotIndex] = DragManager.DraggingItem;
            }
        }
        else if (DragManager.OriginSmelterSlot != null)
        {
            if (!smelterSlot.CanInputItem(targetItem))
            {
                smelterSlot.SetItem(DragManager.DraggingItem);
                inventory.InventoryUI.RefreshUI(inventory.Items);
                inventory.QuickSlotInventoryUI.RefreshUI(inventory.Items);
                smelterSlot.RefreshUI();

                DragManager.Clear();
                return;
            }

            if (targetItem == null)
            {
                inventory.Items[SlotIndex] = DragManager.DraggingItem;
                smelterSlot.SetItem(null);
            }
            //해당칸의 아이템이 겹칠 수 있다면, 겹치기.
            else if (targetItem.Data.itemName == DragManager.DraggingItem.Data.itemName && targetItem.Stackable)
            {
                if (targetItem.IsMaxStack || DragManager.DraggingItem.IsMaxStack)
                {
                    smelterSlot.SetItem(targetItem);
                    inventory.Items[SlotIndex] = DragManager.DraggingItem;
                }
                else
                {
                    MergeItem(inventory.Items[SlotIndex]);
                    if (DragManager.DraggingItem != null)
                    {
                        smelterSlot.SetItem(DragManager.DraggingItem);
                    }
                }
            }
            //해당칸의 아이템이 겹칠 수 없다면, 서로 칸을 바꿔줌.
            else
            {
                smelterSlot.SetItem(targetItem);
                inventory.Items[SlotIndex] = DragManager.DraggingItem;
            }
        }

        inventory.InventoryUI.RefreshUI(inventory.Items);
        inventory.QuickSlotInventoryUI.RefreshUI(inventory.Items);
        smelterSlot.RefreshUI();
        
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
            DragManager.DraggingItem = null;
        }
        else
        {
            item.stack = max;
            DragManager.DraggingItem.stack = total - max;
        }
    }
}