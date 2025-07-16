using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BasePlayer _player;
    private PlayerInputActions _actions;
    private EquipmentDatabase _equippedPickaxe;

    [SerializeField] private LayerMask _miningLayer;
    private Vector2 _moveDir;
    private Vector3 _clickDir;

    private Coroutine _swingCoroutine;
    public bool IsSwing => _swingCoroutine != null;

    #region Event Cancel
    private void OnDisable()
    {
        _actions.Player.Disable();

        _actions.Player.Move.started -= OnMoveStarted;
        _actions.Player.Move.performed -= OnMovePerformed;
        _actions.Player.Move.canceled -= OnMoveCanceled;
        _actions.Player.Swing.started -= OnSwingStarted;
    }
    #endregion

    #region Move Event
    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        _player.Animator.SetBool(BasePlayer.MOVE, true);
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveDir = context.ReadValue<Vector2>().normalized;
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveDir = Vector2.zero;
        _player.Animator.SetBool(BasePlayer.MOVE, false);
    }
    #endregion

    #region Swing Event
    private void OnSwingStarted(InputAction.CallbackContext context)
    {
        if (_player.IsInHome || IsSwing) return;

        _swingCoroutine = StartCoroutine(C_Swing());
    }
    #endregion

    private void FixedUpdate()
    {
        if (IsSwing) return;

        Move();
    }

    /// <summary>
    /// 플레이어 컨트롤러 초기화 + 인풋 시스템 이벤트 등록
    /// </summary>
    public void Init(BasePlayer player)
    {
        _player = player;
        _equippedPickaxe = player.Stat.Pickaxe;

        _actions = player.InputActions;
        _actions.Player.Move.started += OnMoveStarted;
        _actions.Player.Move.performed += OnMovePerformed;
        _actions.Player.Move.canceled += OnMoveCanceled;
        _actions.Player.Swing.started += OnSwingStarted;

        _actions.Player.Enable();
    }

    /// <summary>
    /// 플레이어 이동
    /// </summary>
    private void Move()
    {
        SetAnimationDirection(_moveDir);
        _player.Rigid.MovePosition(_player.Rigid .position + _moveDir * Time.fixedDeltaTime * _player.Stat.MoveSpeed);
    }

    /// <summary>
    /// 스윙 애니메이션 실행
    /// </summary>
    private IEnumerator C_Swing()
    {
        // 클릭 월드 포지션 구하기
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        clickPos.z = 0f;

        // 플레이어 기준으로 클릭의 방향 벡터 구하기
        _clickDir = (clickPos - _player.Animator.transform.position).normalized;
        if (Mathf.Abs(_clickDir.x) > Mathf.Abs(_clickDir.y))
            _clickDir = Vector2.right * (_clickDir.x > 0 ? 1 : -1);
        else
            _clickDir = Vector2.up * (_clickDir.y > 0 ? 1 : -1);

        _player.Animator.SetFloat(BasePlayer.X, _clickDir.x);
        _player.Animator.SetFloat(BasePlayer.Y, _clickDir.y);
        _player.Animator.SetTrigger(BasePlayer.SWING);

        // 애니메이션 끝나는 걸 기다렸다가 채광 시도
        yield return Helper_Coroutine.WaitSeconds(1f / _equippedPickaxe.speed);
        TryMineOre();
    }

    /// <summary>
    /// 채광 시도
    /// </summary>
    private void TryMineOre()
    {
        if (_equippedPickaxe == null) return;

        Collider2D hit = Physics2D.OverlapBox(_player.Animator.transform.position + _clickDir, new Vector2(1f, 1f), 0f, _miningLayer);
        if (hit != null)
        {
            if (hit.TryGetComponent<OreController>(out var ore))
            {
                if (ore.CanBeMined(_equippedPickaxe.crushingForce))
                {
                    bool destroyed = ore.Mine(_equippedPickaxe.damage);
                    Debug.Log(destroyed ? "광석이 파괴됨!" : $"광석에 {_equippedPickaxe.damage}의 데미지 입힘");
                }
                else
                {
                    Debug.Log("곡괭이 파워 부족!");
                }
            }
            else if (hit.TryGetComponent<JewelController>(out var jewel))
            {
                jewel.OnMined();
                Debug.Log("쥬얼 파괴됨!");
            }
        }
        else
        {
            Debug.Log("채광 범위 내에 광석/보석 없음");
        }

        _swingCoroutine = null;
    }

    /// <summary>
    /// 애니메이션 방향 설정
    /// </summary>
    private void SetAnimationDirection(Vector2 moveDir)
    {
        if (moveDir == Vector2.zero) return;

        if (Mathf.Abs(moveDir.x) == Mathf.Abs(moveDir.y))
        {
            moveDir.x = moveDir.x > 0 ? 1f : -1f;
            moveDir.y = 0f;
        }
        _player.Animator.SetFloat(BasePlayer.X, moveDir.x);
        _player.Animator.SetFloat(BasePlayer.Y, moveDir.y);
    }
}
