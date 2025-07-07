using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
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

    public bool IsGamePause { get; private set; }

    private void Start()
    {
        // 일단 start에서 초기화 → 나중에 게임 매니저에서 관리
        InitGameTime();
    }

    private void Update()
    {
        // 게임 매니저에서 게임이 시작되면 타이머 돌아가도록 해야 함
        if (IsGamePause) return;

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
    public void InitGameTime()
    {
        _dailyTimer = 0f;
        Day = 1;
        Stage = 1;

        IsGamePause = false;
        UIManager.Instance.GameTimeUI.SetDayPieces();
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
                TestGameOver();
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
            ControlPause(true);
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
    /// 게임 진행 중에는 일시정지로, 일시정지 중에는 게임 진행 상태로 변경
    /// </summary>
    public void ControlPause()
    {
        ControlPause(!IsGamePause);
    }
    public void ControlPause(bool doPause)
    {
        IsGamePause = doPause;
        Debug.Log($"게임 일시 정지 : {IsGamePause}");
    }

    /// <summary>
    /// 게임 오버 시 일단 몬스터 정지<br/>
    /// ※ 이거는 나중에 게임 매니저에 옮겨도 될 듯
    /// </summary>
    public void TestGameOver()
    {
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
}