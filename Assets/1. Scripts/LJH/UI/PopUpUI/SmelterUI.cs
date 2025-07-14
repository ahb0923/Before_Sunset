using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmelterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _smelterNameText;
    [SerializeField] public SmelterSlot smelterInputSlot;
    [SerializeField] public SmelterSlot smelterOutputSlot;
    [SerializeField] private Image _smelterMaterialSlot1;
    [SerializeField] private Image _smelterMaterialSlot2;
    [SerializeField] private Button _receiveButton;
    [SerializeField] private Button _closeButton;
    
    private RectTransform _rect;

    private const string SMELTER_NAME_TEXT = "SmelterNameText";
    private const string SMELTER_INPUT_SLOT = "SmelterInputSlot";
    private const string SMELTER_OUTPUT_SLOT = "SmelterOutputSlot";
    private const string SMELTER_MATERIAL_SLOT1 = "SmelterMaterialSlot1";
    private const string SMELTER_MATERIAL_SLOT2 = "SmelterMaterialSlot2";
    private const string RECEIVE_BUTTON = "ReceiveButton";
    private const string CLOSE_BUTTON = "CloseSmelterButton";

    private void Reset()
    {
        _smelterNameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, SMELTER_NAME_TEXT);
        smelterInputSlot = Helper_Component.FindChildComponent<SmelterSlot>(this.transform, SMELTER_INPUT_SLOT);
        smelterOutputSlot = Helper_Component.FindChildComponent<SmelterSlot>(this.transform, SMELTER_OUTPUT_SLOT);
        _smelterMaterialSlot1 = Helper_Component.FindChildComponent<Image>(this.transform, SMELTER_MATERIAL_SLOT1);
        _smelterMaterialSlot2 = Helper_Component.FindChildComponent<Image>(this.transform, SMELTER_MATERIAL_SLOT2);
        _receiveButton = Helper_Component.FindChildComponent<Button>(this.transform, RECEIVE_BUTTON);
        _closeButton = Helper_Component.FindChildComponent<Button>(this.transform, CLOSE_BUTTON);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        smelterInputSlot.InitInputSlot(true);
        smelterOutputSlot.InitInputSlot(false);
        _receiveButton.onClick.AddListener(ReceiveItem);
        _closeButton.onClick.AddListener(CloseSmelter);
        _receiveButton.interactable = false;
    }

    private void Start()
    {
        _rect.CloseAndRestore();
    }

    public void OpenSmelter()
    {
        _rect.OpenAtCenter();
        InventoryManager.Instance.Inventory.InventoryUI.Open();
        InventoryManager.Instance.Inventory.QuickSlotInventoryUI.Close();
    }

    public void CloseSmelter()
    {
        _rect.CloseAndRestore();
        InventoryManager.Instance.Inventory.InventoryUI.Close();
        InventoryManager.Instance.Inventory.QuickSlotInventoryUI.Open();
    }

    public void SetSmelterUI(SmelterData data)
    {
        _smelterNameText.text = data.smelterName;
        smelterInputSlot.SetSmelterData(data);
        smelterOutputSlot.SetSmelterData(data);
        // SetSmelterMaterialSlot(data);
    }

    // private void SetSmelterMaterialSlot(SmelterData data)
    // {
    //     List<Image> slotIcons = new List<Image>();
    //     slotIcons.Add(_smelterMaterialSlot1);
    //     slotIcons.Add(_smelterMaterialSlot2);
    //     
    //     for (int i = 0; i < data.smeltingIdList.Count; i++)
    //     {
    //         var image = DataManager.Instance.ItemData.GetId(data.smeltingIdList[i]).sprite;
    //         slotIcons[i] = image;
    //     }
    // }

    public void AddItemToSmelter(Item item, int amount)
    {
        smelterOutputSlot.AddToSmelterItem(item, amount);
        _receiveButton.interactable = true;
    }
    
    private void ReceiveItem()
    {
        smelterOutputSlot.ReceiveItem();
        _receiveButton.interactable = false;
    }
}
