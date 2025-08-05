using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("ClearResult")]
    [SerializeField] private RectTransform _clearResultRect;
    [SerializeField] private TextMeshProUGUI _bestRecordText;
    [SerializeField] private TextMeshProUGUI _currentRecordText;
    [SerializeField] private GameObject _slotArea;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private List<RewardSlot> _slots = new List<RewardSlot>();
    [SerializeField] private TextMeshProUGUI _shardAmountText;
    [SerializeField] private Button _rewardButton;

    [Header("FailResult")]
    [SerializeField] private RectTransform _failResultRect;
    [SerializeField] private TextMeshProUGUI _failBestRecordText;
    [SerializeField] private TextMeshProUGUI _failCurrentRecordText;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _exitButton;
    
    private RectTransform _rect;
    public bool IsOpen { get; private set; }
    public Dictionary<int, bool> StageRewarded { get; private set; } = new Dictionary<int, bool>();
    
    private const string BEST_RECORD_TEXT = "BestRecordText";
    private const string FAIL_BEST_RECORD_TEXT = "FailBestRecordText";
    private const string CURRENT_RECORD_TEXT = "CurrentRecordText";
    private const string FAIL_CURRENT_RECORD_TEXT = "FailCurrentRecordText";
    private const string SHARD_AMOUNT_TEXT = "ShardAmountText";
    private const string CLEAR_RESULT = "ClearResult";
    private const string REWARD_BUTTON = "RewardButton";
    private const string SLOT_AREA = "ClearRewardSlotArea";
    private const string SLOT_PREFAB = "Slots/RewardSlot";
    private const string FAIL_RESULT = "FailResult";
    private const string LOAD_BUTTON = "LoadButton";
    private const string EXIT_BUTTON = "ExitButton";

    private void Reset()
    {
        _clearResultRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, CLEAR_RESULT);
        _bestRecordText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BEST_RECORD_TEXT);
        _currentRecordText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, CURRENT_RECORD_TEXT);
        _slotArea = Helper_Component.FindChildGameObjectByName(this.gameObject, SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(SLOT_PREFAB);
        _shardAmountText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, SHARD_AMOUNT_TEXT);
        _rewardButton = Helper_Component.FindChildComponent<Button>(this.transform, REWARD_BUTTON);
        
        _failResultRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, FAIL_RESULT);
        _failBestRecordText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, FAIL_BEST_RECORD_TEXT);
        _failCurrentRecordText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, FAIL_CURRENT_RECORD_TEXT);
        _loadButton = Helper_Component.FindChildComponent<Button>(this.transform, LOAD_BUTTON);
        _exitButton = Helper_Component.FindChildComponent<Button>(this.transform, EXIT_BUTTON);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _rewardButton.onClick.AddListener(CloseClear);
        _loadButton.onClick.AddListener(CloseOnLoad);
        _exitButton.onClick.AddListener(CloseOnExit);
    }

    private void Start()
    {
        int count = DataManager.Instance.ClearRewardData.GetAllItems().Count;

        for (int i = 0; i < count; i++)
        {
            StageRewarded.Add(i+1, false);
        }
        
        InitSlots();
    }

    private void InitSlots(int stage = 1)
    {
        var count = DataManager.Instance.ClearRewardData.GetAllItems()[stage-1].jewelReward.Count;

        if (count <= _slots.Count)
            return;
        
        int needCount = count - _slots.Count;
        
        for (int i = 0; i < needCount; i++)
        {
            var slot = Instantiate(_slotPrefab, _slotArea.transform);
            _slots.Add(slot.GetComponent<RewardSlot>());
        }
    }

    public void OpenClear(int stage)
    {
        TimeManager.Instance.PauseGame(true);

        IsOpen = true;
        InitSlots(stage);
        RefreshSlots(stage);
        RefreshShard(stage);
        RefreshClear();
        _rect.OpenAtCenter();
        _clearResultRect.OpenAtCenter();
    }

    private void CloseClear()
    {
        IsOpen = false;
        foreach (var slot in _slots)
        {
            if (slot.JewelName == null)
            {
                continue;
            }

            int id = DataManager.Instance.JewelData.GetByName(slot.JewelName).id;
            InventoryManager.Instance.Inventory.AddItem(id, slot.Amount);
        }
        StageRewarded[TimeManager.Instance.Day] = true;
        
        _clearResultRect.CloseAndRestore();
        _rect.CloseAndRestore();
        
        TimeManager.Instance.PauseGame(false);
        TimeManager.Instance.NextDay();
    }

    public void OpenFail()
    {
        TimeManager.Instance.PauseGame(true);

        IsOpen = true;
        RefreshFail();
        _rect.OpenAtCenter();
        _failResultRect.OpenAtCenter();
    }

    private void CloseOnLoad()
    {
        IsOpen = false;
        _failResultRect.CloseAndRestore();
        _rect.CloseAndRestore();
        TimeManager.Instance.PauseGame(false);

        if(TimeManager.Instance.Day != 1)
        {
            SaveManager.Instance.LoadGameFromAutoSlot();
        }
        else 
        {
            // 1스테이지에서 죽으면 새 게임 시작
            SaveManager.Instance.LoadGameFromSlot();
        }
    }
    
    private void CloseOnExit()
    {
        IsOpen = false;
        _failResultRect.CloseAndRestore();
        _rect.CloseAndRestore();
        TimeManager.Instance.PauseGame(false);
        SceneManager.LoadScene("StartScene");
    }

    private void RefreshClear()
    {
        _bestRecordText.text = "";
        _currentRecordText.text = $"{TimeManager.Instance.Day:D2}일";
    }

    private void RefreshSlots(int stage)
    {
        var data = DataManager.Instance.ClearRewardData.GetAllItems()[stage];

        for (int i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            slot.Clear();
            
            if (data.jewelReward[i] != null)
            {
                var amount = Random.Range(data.minQuantity, data.maxQuantity);
                slot.Refresh(data.jewelReward[i], amount);
            }
        }
    }

    private void RefreshShard(int stage)
    {
        var data = DataManager.Instance.ClearRewardData.GetAllItems()[stage];
        _shardAmountText.text = data.essenceShardReward.ToString("D2");
    }
    
    private void RefreshFail()
    {
        _failBestRecordText.text = "";
        _failCurrentRecordText.text = $"{TimeManager.Instance.Day:D2}주";
    }
}
