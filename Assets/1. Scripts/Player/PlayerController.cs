using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BasePlayer _player;
    private PlayerInteractor _interactor;
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
        if (IsSwing || IsRecalling) return;

        Move();
    }

    /// <summary>
    /// 플레이어 컨트롤러 초기화 + 인풋 시스템 이벤트 등록
    /// </summary>
    public void Init(BasePlayer player)
    {
        _player = player;
        _interactor = GetComponent<PlayerInteractor>();
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
        _player.Rigid.MovePosition(_player.Rigid.position + _moveDir * Time.fixedDeltaTime * _player.Stat.MoveSpeed);
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

        IInteractable target = _interactor.GetCurrentTarget();

        if (target != null && !(target is OreController) && !(target is JewelController))
        {
            TryInteractTarget(target);
            _swingCoroutine = null;
            yield break;
        }

        // 플레이어 기준으로 클릭의 방향 벡터 구하기
        _clickDir = (clickPos - _player.Animator.transform.position).normalized;
        _clickDir = Mathf.Abs(_clickDir.x) > Mathf.Abs(_clickDir.y)
            ? Vector2.right * Mathf.Sign(_clickDir.x)
            : Vector2.up * Mathf.Sign(_clickDir.y);

        _player.Animator.SetFloat(BasePlayer.X, _clickDir.x);
        _player.Animator.SetFloat(BasePlayer.Y, _clickDir.y);
        _player.Animator.SetTrigger(BasePlayer.SWING);

        if (target is OreController)
        {
            AudioManager.Instance.PlayRandomSFX("HittingARock", 4);
        }

        // 애니메이션 끝나는 걸 기다렸다가 채광 시도
        yield return Helper_Coroutine.WaitSeconds(1f / _equippedPickaxe.speed);

        target = _interactor.GetCurrentTarget();
        TryInteractTarget(target);

        _swingCoroutine = null;
    }

    /// <summary>
    /// 채광 시도
    /// </summary>
    private void TryInteractTarget(IInteractable target)
    {
        if (target == null) return;

        float range = (target is OreController || target is JewelController) ? 1.5f : 5.0f;

        if (!target.IsInteractable(_player.transform.position, range, _player.PlayerCollider))
            return;

        if (target is OreController ore)
        {
            int wallLayerMask = LayerMask.GetMask("Wall");
            Vector2 playerPos = _player.transform.position;
            Vector2 orePos = ore.transform.position;

            if (Physics2D.Linecast(playerPos, orePos, wallLayerMask))
            {
                Debug.Log("벽에 막혀 채굴할 수 없습니다.");
                return;
            }

            if (_player.Stat.Pickaxe.crushingForce < ore._data.def)
            {
                Debug.Log("곡괭이 힘이 부족합니다.");
                return;
            }

            ore.Mine(_player.Stat.Pickaxe.damage);
        }
        else if (target is JewelController jewel)
        {
            int wallLayerMask = LayerMask.GetMask("Wall");
            Vector2 playerPos = _player.transform.position;
            Vector2 jewelPos = jewel.transform.position;

            if (Physics2D.Linecast(playerPos, jewelPos, wallLayerMask))
            {
                Debug.Log("벽에 막혀 채굴할 수 없습니다.");
                return;
            }
        }

        target.Interact();
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
