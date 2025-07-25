using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>, ISaveable
{
    [Header("# Time Setting")]
    [SerializeField] private int _realMinDayLength = 12;
    [SerializeField] private bool _isSec; // test용
    private int _realSecDayLength => _realMinDayLength * (_isSec ? 1 : 60);
    [SerializeField] private int _maxDay = 5;
    public int MaxDay => _maxDay;
    [SerializeField] private int _maxStage = 3;

    private float _dailyTimer;
    public bool IsNight => _dailyTimer > _realSecDayLength * 0.5f;
    public float DailyPercent => _dailyTimer / _realSecDayLength;

    public int Day { get; private set; }
    public int Stage { get; private set; }

    private bool _isSpawned;
    private bool _isSpawnOver;
    public bool IsStageClear => _isSpawnOver && !DefenseManager.Instance.MonsterSpawner.IsMonsterAlive;

    private void Start()
    {
        // 일단 start에서 초기화 → 나중에 게임 매니저에서 관리
        InitGameTime();
    }

    private void Update()
    {
        _dailyTimer += Time.deltaTime;

        if(_dailyTimer >= _realSecDayLength)
        {
            NextDay();
        }

        if(Day == _maxDay - 1 && IsNight && !_isSpawned)
        {
            _isSpawned = true;
            DefenseManager.Instance.MonsterSpawner.SpawnAllMonsters();
        }
    }

    /// <summary>
    /// 게임 시간 초기화
    /// </summary>
    public void InitGameTime(float dailyTime = 0f, int day = 1, int stage = 1)
    {
        _dailyTimer = dailyTime;
        Day = day;
        Stage = stage;

        UIManager.Instance.GameTimeUI.SetDayPieces();
        UIManager.Instance.GameTimeUI.SetStageText();
    }

    /// <summary>
    /// 다음 날로 변경
    /// </summary>
    public void NextDay()
    {
        _dailyTimer = 0;
        
        if(Day == _maxDay)
        {
            if (IsStageClear)
            {
                NextStage();
            }
            else
            {
                // 패배 시에 일시 정지 & 엔티티 움직임 정지
                PauseGame(true);
            }
        }
        else
        {
            Day++;
        }

        UIManager.Instance.GameTimeUI.SetDayPieces();
    }

    /// <summary>
    /// 다음 스테이지로 변경
    /// </summary>
    private void NextStage()
    {
        if(Stage == _maxStage)
        {
            PauseGame(true);
            // 승리
        }
        else
        {
            Day = 1;
            Stage++;
        }

        _isSpawned = false;
        _isSpawnOver = false;
        UIManager.Instance.GameTimeUI.SetStageText();
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
    /// 반나절 스킵 : 낮 → 밤 / 밤 → 낮
    /// </summary>
    public void SkipHalfDay()
    {
        if (IsNight)
        {
            NextDay();
        }
        else
        {
            _dailyTimer = _realSecDayLength * 0.5f;
        }
    }

    /// <summary>
    /// 시간 데이터 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        data.timeData = new TimeSaveData(Stage, Day, _dailyTimer, IsNight);
    }

    /// <summary>
    /// 시간 데이터 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        InitGameTime(data.timeData.dailyTime, data.timeData.day, data.timeData.stage);
    }
}