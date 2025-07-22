using UnityEngine;

public class BasePlayer : MonoBehaviour, ISaveable
{
    public static readonly int MOVE = Animator.StringToHash("Move");
    public static readonly int SWING = Animator.StringToHash("Swing");
    public static readonly int X = Animator.StringToHash("DirX");
    public static readonly int Y = Animator.StringToHash("DirY");
    public static readonly int MINING = Animator.StringToHash("MiningSpeed");

    [Header("Player Sprites")]
    [SerializeField] private SpriteRenderer _playerRenderer;
    [SerializeField] private SpriteRenderer _weaponRenderer;
    [SerializeField] private SpriteRenderer _effectRenderer;
    [SerializeField] private SpriteRenderer _effectDownRenderer;

    public Rigidbody2D Rigid { get; private set; }
    public BoxCollider2D PlayerCollider { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerStatHandler Stat { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInputActions InputActions { get; private set; }

    public bool IsInBase { get; private set; }

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<BoxCollider2D>();
        Controller = GetComponent<PlayerController>();
        InputHandler = GetComponent<PlayerInputHandler>();
        Stat = GetComponent<PlayerStatHandler>();
        Animator = GetComponentInChildren<Animator>();
        InputActions = new PlayerInputActions();
    }

    private void Start()
    {
        Stat.Init(this);
        Controller.Init(this);
        InputHandler.Init(this);

        IsInBase = true;
    }

    private void LateUpdate()
    {
        if (_playerRenderer == null || _weaponRenderer == null || _effectRenderer == null || _effectDownRenderer == null)
            return;

        // 플레이어 & 무기 & 스윙 이펙트 & 스윙 다운 이펙트 sorting order 적용
        RenderUtil.SetSortingOrderByY(_playerRenderer, transform.position.y);
        RenderUtil.SetSortingOrderByY(_weaponRenderer, transform.position.y + 0.01f);
        RenderUtil.SetSortingOrderByY(_effectRenderer, transform.position.y - 0.01f);
        RenderUtil.SetSortingOrderByY(_effectDownRenderer, transform.position.y + 0.02f);
    }

    /// <summary>
    /// 플레이어가 어느 맵에 위치한지 세팅
    /// </summary>
    public void SetPlayerInBase(bool isInBase)
    {
        IsInBase = isInBase;
    }

    /// <summary>
    /// 플레이어 위치 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        data.playerPosition = transform.position;
    }

    /// <summary>
    /// 플레이어 위치 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;
    }
}
