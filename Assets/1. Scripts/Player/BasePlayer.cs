using UnityEngine;

public class BasePlayer : MonoBehaviour
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
    public PlayerController Controller { get; private set; }
    public PlayerStatHandler Stat { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInputActions InputActions { get; private set; }

    public bool IsInHome { get; private set; }

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        Controller = GetComponent<PlayerController>();
        Stat = GetComponent<PlayerStatHandler>();
        Animator = GetComponentInChildren<Animator>();
        InputActions = new PlayerInputActions();

        Stat.Init(this);
        Controller.Init(this);

        IsInHome = true;
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

    public void SetHomeOnPlayer(bool isHome)
    {
        IsInHome = isHome;
    }
}
