using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
{
    [Header("# Time Setting")]
    [SerializeField] private int _realMinDayLength = 12;
    [SerializeField] private bool _isSec; // test��
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
        // �ϴ� start���� �ʱ�ȭ �� ���߿� ���� �Ŵ������� ����
        InitGameTime();
    }

    private void Update()
    {
        // ���� �Ŵ������� ������ ���۵Ǹ� Ÿ�̸� ���ư����� �ؾ� ��
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
    /// ���� �ð� �ʱ�ȭ
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
    /// ���� ���� ����
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
                // �й� �ÿ� �Ͻ� ���� & ��ƼƼ ������ ����
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
    /// ���� ���������� ����
    /// </summary>
    private void NextStage()
    {
        if(Stage == _maxStage)
        {
            ControlPause(true);
            // �¸�
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
    /// ��� ���� ������ ������ �� ȣ��
    /// </summary>
    public void OnSpawnOver()
    {
        _isSpawnOver = true;
    }

    /// <summary>
    /// ���� ���� �߿��� �Ͻ�������, �Ͻ����� �߿��� ���� ���� ���·� ����
    /// </summary>
    public void ControlPause()
    {
        ControlPause(!IsGamePause);
    }
    public void ControlPause(bool doPause)
    {
        IsGamePause = doPause;
        Debug.Log($"���� �Ͻ� ���� : {IsGamePause}");
    }

    /// <summary>
    /// ���� ���� �� �ϴ� ���� ����<br/>
    /// �� �̰Ŵ� ���߿� ���� �Ŵ����� �Űܵ� �� ��
    /// </summary>
    public void TestGameOver()
    {
    }

    /// <summary>
    /// �ݳ��� ��ŵ : �� �� �� / �� �� ��
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