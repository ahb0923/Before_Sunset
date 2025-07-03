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

    [Header("# Test")]
    [SerializeField] private GameTimeUI _gameTimeUI; // 이거는 UI 매니저에서 가져오는 방식으로 바꿔야 할 듯

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
        _gameTimeUI.SetDayPieces(Day);
    }

    /// <summary>
    /// 다음 날로 변경
    /// </summary>
    public void NextDay()
    {
        _dailyTimer = 0;
        
        if(Day == _maxDay)
        {
            if (true) //몬스터를 모두 처치했다면
            {
                NextStage();
            }
            else
            {
                // 패배
            }
        }
        else
        {
            Day++;
        }

        _gameTimeUI.SetDayPieces(Day);
    }

    /// <summary>
    /// 다음 스테이지로 변경
    /// </summary>
    private void NextStage()
    {
        if(Stage == _maxStage)
        {
            // 승리
        }
        else
        {
            Day = 1;
            Stage++;
        }

        _gameTimeUI.SetStageText(Stage);
    }

    /// <summary>
    /// 게임 진행 중에는 일시정지로, 일시정지 중에는 게임 진행 상태로 변경
    /// </summary>
    public void ControlPause()
    {
         IsGamePause = !IsGamePause;
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