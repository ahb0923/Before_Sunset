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

    [Header("# Test")]
    [SerializeField] private GameTimeUI _gameTimeUI; // �̰Ŵ� UI �Ŵ������� �������� ������� �ٲ�� �� ��

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
        _gameTimeUI.SetDayPieces(Day);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void NextDay()
    {
        _dailyTimer = 0;
        
        if(Day == _maxDay)
        {
            if (true) //���͸� ��� óġ�ߴٸ�
            {
                NextStage();
            }
            else
            {
                // �й�
            }
        }
        else
        {
            Day++;
        }

        _gameTimeUI.SetDayPieces(Day);
    }

    /// <summary>
    /// ���� ���������� ����
    /// </summary>
    private void NextStage()
    {
        if(Stage == _maxStage)
        {
            // �¸�
        }
        else
        {
            Day = 1;
            Stage++;
        }

        _gameTimeUI.SetStageText(Stage);
    }

    /// <summary>
    /// ���� ���� �߿��� �Ͻ�������, �Ͻ����� �߿��� ���� ���� ���·� ����
    /// </summary>
    public void ControlPause()
    {
         IsGamePause = !IsGamePause;
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