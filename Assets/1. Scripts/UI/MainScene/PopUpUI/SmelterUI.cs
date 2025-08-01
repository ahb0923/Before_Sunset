using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmelterUI : MonoBehaviour, ICloseableUI
{
    [SerializeField] private Smelter _currentSmelter;
    [SerializeField] private TextMeshProUGUI _smelterNameText;
    [SerializeField] public SmelterSlot smelterInputSlot;
    [SerializeField] public SmelterSlot smelterOutputSlot;
    [SerializeField] private Image _smelterMaterialSlot1;
    [SerializeField] private Image _smelterMaterialSlot2;
    [SerializeField] private Image _smeltProgressBar;
    [SerializeField] private Button _receiveButton;
    [SerializeField] private Button _closeButton;

    private RectTransform _rect;

    private const string SMELTER_NAME_TEXT = "SmelterNameText";
    private const string SMELTER_INPUT_SLOT = "SmelterInputSlot";
    private const string SMELTER_OUTPUT_SLOT = "SmelterOutputSlot";
    private const string SMELTER_MATERIAL_SLOT1 = "SmelterMaterialSlot1";
    private const string SMELTER_MATERIAL_SLOT2 = "SmelterMaterialSlot2";
    private const string SMELT_PROGRESS_BAR = "SmeltProgressBar";
    private const string RECEIVE_BUTTON = "ReceiveButton";
    private const string CLOSE_BUTTON = "CloseSmelterButton";

    private void Reset()
    {
        _smelterNameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, SMELTER_NAME_TEXT);
        smelterInputSlot = Helper_Component.FindChildComponent<SmelterSlot>(this.transform, SMELTER_INPUT_SLOT);
        smelterOutputSlot = Helper_Component.FindChildComponent<SmelterSlot>(this.transform, SMELTER_OUTPUT_SLOT);
        _smelterMaterialSlot1 = Helper_Component.FindChildComponent<Image>(this.transform, SMELTER_MATERIAL_SLOT1);
        _smelterMaterialSlot2 = Helper_Component.FindChildComponent<Image>(this.transform, SMELTER_MATERIAL_SLOT2);
        _smeltProgressBar = Helper_Component.FindChildComponent<Image>(this.transform, SMELT_PROGRESS_BAR);
        _receiveButton = Helper_Component.FindChildComponent<Button>(this.transform, RECEIVE_BUTTON);
        _closeButton = Helper_Component.FindChildComponent<Button>(this.transform, CLOSE_BUTTON);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        smelterInputSlot.InitInputSlot(true);
        smelterOutputSlot.InitInputSlot(false);
        _receiveButton.onClick.AddListener(ReceiveItem);
        _closeButton.onClick.AddListener(Close);
        _smeltProgressBar.fillAmount = 0f;
        DeactivateReceiveButton();
    }

    private void Start()
    {
        _rect.CloseAndRestore();
    }

    public void Open(Smelter smelter)
    {
        if (_currentSmelter != null)
        {
            _currentSmelter.OnSmeltingProgress -= UpdateProgressBar;
        }
        
        
        _currentSmelter = smelter;
        AudioManager.Instance.PlaySFX("OpenSmelter");
        UIManager.Instance.OpenUI(this);
        SetSmelterUI();
        InventoryManager.Instance.Inventory.InventoryUI.Open();
        InventoryManager.Instance.Inventory.QuickSlotInventoryUI.Close();
        
        _currentSmelter.OnSmeltingProgress += UpdateProgressBar;

        if (!_currentSmelter.isSmelting)
        {
            _smeltProgressBar.fillAmount = 0f;
        }

        if (_currentSmelter.OutputItem != null)
        {
            ActivateReceiveButton();
        }
        
    }

    public void Open()
    {
        throw new System.NotImplementedException();
    }

    public void Close()
    {
        UIManager.Instance.CloseUI(this);
    }

    public void OpenUI()
    {
        _rect.OpenAtCenter();
    }

    public void CloseUI()
    {
        smelterInputSlot.ClearSlot();
        smelterOutputSlot.ClearSlot();
        DeactivateReceiveButton();
        InventoryManager.Instance.Inventory.InventoryUI.Close();
        InventoryManager.Instance.Inventory.QuickSlotInventoryUI.Open();
        
        _currentSmelter.OnSmeltingProgress -= UpdateProgressBar;
        
        _currentSmelter = null;
        _rect.CloseAndRestore();
    }

    private void UpdateProgressBar(float progress)
    {
        _smeltProgressBar.fillAmount = progress;
    }

    /// <summary>
    /// 제련소 UI 설정 메서드 및 슬롯에 현재 제련소를 참조시키는 메서드
    /// </summary>
    public void SetSmelterUI()
    {
        var data = _currentSmelter.smelterData;
        
        _smelterNameText.text = data.smelterName;
        SetSmelterMaterialSlot(data);

        smelterInputSlot.SetSmelterData(_currentSmelter);
        smelterOutputSlot.SetSmelterData(_currentSmelter);
    }

    /// <summary>
    /// 제련가능한 재료 표시해주는 메서드
    /// </summary>
    /// <param name="data"></param>
    private void SetSmelterMaterialSlot(SmelterDatabase data)
    {
        var image1 = DataManager.Instance.MineralData.GetSpriteById(data.smeltingIdList[0]);
        _smelterMaterialSlot1.sprite = image1;
        var image2 = DataManager.Instance.MineralData.GetSpriteById(data.smeltingIdList[1]);
        _smelterMaterialSlot2.sprite = image2;
    }

    /// <summary>
    /// 받기 버튼 활성화 메서드
    /// </summary>
    private void ActivateReceiveButton()
    {
        _receiveButton.interactable = true;
    }

    /// <summary>
    /// 받기 버튼 비활성화 메서드
    /// </summary>
    private void DeactivateReceiveButton()
    {
        _receiveButton.interactable = false;
    }
    
    /// <summary>
    /// 제련소 결과물슬롯의 아이템을 받을때 사용하는 메서드
    /// </summary>
    private void ReceiveItem()
    {
        smelterOutputSlot.ReceiveItem();
        DeactivateReceiveButton();
        _currentSmelter.TrySmelt();
    }

    /// <summary>
    /// 제련소 슬롯들의 UI 새로고침 메서드
    /// </summary>
    public void RefreshSlots()
    {
        smelterInputSlot.RefreshUI();
        smelterOutputSlot.RefreshUI();

        if (this.gameObject.activeSelf)
        {
            if (_currentSmelter.OutputItem != null)
                ActivateReceiveButton();
        }
    }
}
