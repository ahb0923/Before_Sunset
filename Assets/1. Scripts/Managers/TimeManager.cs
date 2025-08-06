using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>, ISaveable
{
    [Header("# Time Setting")]
    [SerializeField] private int _realMinDayLength = 5;
    private int _realSecDayLength => _realMinDayLength * 60;
    public int MaxStage { get; private set; }

    private float _dailyTimer;
    public bool IsNight => _dailyTimer >= _realSecDayLength;
    public float DailyPercent => _dailyTimer / _realSecDayLength;

    public int Day { get; private set; }

    private bool _isSpawned;
    private bool _isSpawnOver;
    private bool _isRecallOver;
    public bool IsStageClear => _isSpawned && _isSpawnOver && !DefenseManager.Instance.MonsterSpawner.IsMonsterAlive;

    private void Start()
    {
        MaxStage = DataManager.Instance.ClearRewardData.GetAllItems().Count + 1;

        // 새 게임이면, 초기화 진행
        if (GlobalState.Index == -1)
            InitGameTime();
    }

    private void Update()
    {
        _dailyTimer += Time.deltaTime;

        if (DailyPercent >= 0.9f && !_isRecallOver)
        {
            Recall();
            _isRecallOver = true;
        }

        // 밤이 되면 몬스터 스폰
        if (IsNight && !_isSpawned)
        {
            _isSpawned = true;
            DefenseManager.Instance.MonsterSpawner.SpawnAllMonsters();
            AudioManager.Instance.PlayBGM("DefenseBase");
        }

        // 해당 날짜 클리어 
        if (IsStageClear)
        {
            _isSpawnOver = false;
            if(Day == MaxStage)
            {
                GameManager.Instance.GoToEndScene();
                return;
            }

            UIManager.Instance.ResultUI.Open(Day - 1, true);
            AudioManager.Instance.PlayBGM("NormalBase");
        }
    }

    /// <summary>
    /// 게임 시간 초기화
    /// </summary>
    public void InitGameTime(float dailyTime = 0f, int day = 1)
    {
        _dailyTimer = dailyTime;
        Day = day;

        UIManager.Instance.GameTimeUI.SetDayText();
    }

    /// <summary>
    /// 다음 날로 변경
    /// </summary>
    public void NextDay()
    {
        _dailyTimer = 0f;
        _isSpawned = false;
        _isRecallOver = false;

        MapManager.Instance.ResetAllMaps();
        SpawnManager.Instance.OnStageChanged();

        if (Day != MaxStage)
        {
            Day++;
        }

        UIManager.Instance.AutoSaveLoadSlot.Save();
        UIManager.Instance.GameTimeUI.SetDayText();
    }

    /// <summary>
    /// 모든 몬스터 스폰이 끝났을 때 호출
    /// </summary>
    public void OnSpawnOver()
    {
        _isSpawnOver = true;
    }

    /// <summary>
    /// true면 일시 정지, false면 일시 정지 해제
    /// </summary>
    public void PauseGame(bool doPause)
    {
        Time.timeScale = doPause ? 0f : 1f;
        Debug.Log($"게임 일시 정지 : {doPause}");
    }

    /// <summary>
    /// 반나절 스킵
    /// </summary>
    public void SkipHalfDay()
    {
        if (IsNight)
        {
            ToastManager.Instance.ShowToast("몬스터 웨이브 중에는 스킵이 불가능합니다!");
        }
        else
        {
            _dailyTimer = _realSecDayLength;
        }

        QuestManager.Instance?.SetQuestAmount(QUEST_TYPE.TimeSkip, -1, 1);
    }

    /// <summary>
    /// 시간 데이터 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        data.timeData = new TimeSaveData(Day, _dailyTimer);
    }

    /// <summary>
    /// 시간 데이터 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        InitGameTime(data.timeData.dailyTime, data.timeData.day);
    }

    /// <summary>
    /// 강제 귀환 메서드 → 이거 플레이어 쪽으로 옮겨야 함
    /// </summary>
    private void Recall()
    {
        BasePlayer player = FindObjectOfType<BasePlayer>();
        if (player.IsInBase)
        {
            return;
        }

        player.InputHandler.StartRecall();
    }
}