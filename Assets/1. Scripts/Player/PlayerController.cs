using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BasePlayer _player;
    private PlayerInputActions _actions;
    private EquipmentDatabase _equippedPickaxe;

    private Vector2 _moveDir;
    private Vector3 _clickDir;

    private Coroutine _swingCoroutine;
    public bool IsSwing => _swingCoroutine != null;

    private bool IsRecalling => PlayerInputHandler._isRecallInProgress;

    #region Event Subscriptions
    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        if (PlayerInputHandler._isRecallInProgress) return;

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

    private void OnSwingStarted(InputAction.CallbackContext context)
    {
        if (_player.IsInBase || IsSwing || IsRecalling) return;

        if (_player.Animator.GetFloat(BasePlayer.MINING) != _player.Stat.Pickaxe.speed)
            _player.Animator.SetFloat(BasePlayer.MINING, _player.Stat.Pickaxe.speed);

        _swingCoroutine = StartCoroutine(C_Swing());
    }
    #endregion

    #region Event Unsubscriptions
    private void OnDisable()
    {
        _actions.Player.Disable();
        _actions.Player.Move.started -= OnMoveStarted;
        _actions.Player.Move.performed -= OnMovePerformed;
        _actions.Player.Move.canceled -= OnMoveCanceled;
        _actions.Player.Swing.started -= OnSwingStarted;
    }
    #endregion

    private void FixedUpdate()
    {
        if(IsSwing || IsRecalling) return;

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
        // UI 클릭 체크
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            _swingCoroutine = null;
            yield break;
        }

        // 클릭 월드 포지션 구하기
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        clickPos.z = 0f;

        Collider2D col = Physics2D.OverlapPoint(clickPos);
        IInteractable target = null;
        if (col != null)
            col.TryGetComponent<IInteractable>(out target);

        // 플레이어 기준으로 클릭의 방향 벡터 구하기
        _clickDir = (clickPos - _player.Animator.transform.position).normalized;
        _clickDir = Mathf.Abs(_clickDir.x) > Mathf.Abs(_clickDir.y)
            ? Vector2.right * Mathf.Sign(_clickDir.x)
            : Vector2.up * Mathf.Sign(_clickDir.y);

        _player.Animator.SetFloat(BasePlayer.X, _clickDir.x);
        _player.Animator.SetFloat(BasePlayer.Y, _clickDir.y);
        _player.Animator.SetTrigger(BasePlayer.SWING);

        // 애니메이션 끝나는 걸 기다렸다가 채광 시도
        yield return Helper_Coroutine.WaitSeconds(1f / _equippedPickaxe.speed);

        TryMineTarget(target);

        _swingCoroutine = null;
    }

    /// <summary>
    /// 채광 시도
    /// </summary>
    private void TryMineTarget(IInteractable target)
    {
        if (target == null) return;

        if (_equippedPickaxe == null) return;

        if (target is OreController ore)
        {
            ore.Init(_player);
            ore.Interact();
        }
        else if (target is JewelController jewel)
        {
            jewel.Interact();
        }
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
