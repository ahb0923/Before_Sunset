using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private BasePlayer _player;
    private PlayerInputActions _actions;

    [SerializeField] private float _returnKeyHeldTime = 1f;

    private bool _isRecallStarted;
    private bool _isHeldReturnKey;
    private float _heldTimer;

    public static bool _isRecallInProgress { get; private set; } = false;

    #region Event Subscriptions
    private void OnInventoryStarted(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.Inventory.Toggle();
    }

    private void OnBuildStarted(InputAction.CallbackContext context)
    {
        UIManager.Instance.CraftArea.Toggle();
    }

    private void OnDestroyModeStarted(InputAction.CallbackContext context)
    {
        BuildManager.Instance.IsOnDestroy = !BuildManager.Instance.IsOnDestroy;
    }

    private void OnReturnHomeStarted(InputAction.CallbackContext context)
    {
        _isHeldReturnKey = true;
    }

    private void OnReturnHomeCanceled(InputAction.CallbackContext context)
    {
        _heldTimer = 0f;
        UIManager.Instance.RecallUI.UpdateHoldProgress(0f);
        _isHeldReturnKey = false;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Vector3 clickWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        clickWorldPos.z = 0f;

        // interactable 레이어에서 충돌체 검색
        LayerMask interactableLayer = LayerMask.GetMask("Tower", "Core", "Smelter", "Interactable");
        Collider2D col = Physics2D.OverlapPoint(clickWorldPos, interactableLayer);

        if (col == null) return;

        IInteractable target = col.GetComponent<IInteractable>();
        if (target == null) return;

        // 플레이어 위치에서 상호작용 가능한지 검사
        if (!target.IsInteractable(_player.transform.position, 5f, _player.PlayerCollider))
            return;

        target.Interact();
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
    }
    #endregion

    #region Event Unsubscriptions
    private void OnDisable()
    {
        _actions.Interaction.Disable();
        _actions.Player.Interact.performed -= OnInteractPerformed;
        _actions.Interaction.Inventory.started -= OnInventoryStarted;
        _actions.Interaction.Build.started -= OnBuildStarted;
        _actions.Interaction.DestroyMode.started -= OnDestroyModeStarted;
        _actions.Interaction.ReturnHome.performed -= OnReturnHomeStarted;
        _actions.Player.Dash.performed -= OnDashPerformed;
    }
    #endregion

    private void Update()
    {
        if (_player.IsInBase || _isRecallStarted || !_isHeldReturnKey) return;

        _heldTimer += Time.deltaTime;
        UIManager.Instance.RecallUI.UpdateHoldProgress(_heldTimer / _returnKeyHeldTime);

        if (_heldTimer >= _returnKeyHeldTime)
        {
            StartRecall();
        }
    }

    /// <summary>
    /// 키보드 인풋 핸들러 초기화
    /// </summary>
    public void Init(BasePlayer player)
    {
        _player = player;

        _actions = player.InputActions;
        _actions.Player.Interact.performed += OnInteractPerformed;
        _actions.Interaction.Inventory.started += OnInventoryStarted;
        _actions.Interaction.Build.started += OnBuildStarted;
        _actions.Interaction.DestroyMode.started += OnDestroyModeStarted;
        _actions.Interaction.ReturnHome.started += OnReturnHomeStarted;
        _actions.Interaction.ReturnHome.canceled += OnReturnHomeCanceled;
        _actions.Player.Dash.performed += OnDashPerformed;
        _actions.Interaction.Enable();

        UIManager.Instance.RecallUI.OnCountdownFinished += OnRecallCountdownFinished;

        _isRecallStarted = false;
        _isHeldReturnKey = false;
        _heldTimer = 0f;
    }

    /// <summary>
    /// 귀환 키보드 일정 시간 눌렀을 때, 귀환 시작
    /// </summary>
    public void StartRecall()
    {
        _isRecallStarted = true;
        _isRecallInProgress = true;
        _heldTimer = 0f;

        UIManager.Instance.RecallUI.StartRecallCountdown();
    }

    /// <summary>
    /// 귀환 카운트다운이 끝나고 실제 기지로 귀환
    /// </summary>
    private void OnRecallCountdownFinished()
    {
        StartCoroutine(C_Recall());
    }

    private IEnumerator C_Recall()
    {
        yield return StartCoroutine(ScreenFadeController.Instance.FadeInOut(() =>
        {
            UIManager.Instance.RecallUI.CloseRecall();

            if (GameManager.Instance.IsTutorial)
                transform.position = new Vector3(0, 2, 0);
            else
            {
                MapManager.Instance.ReturnToHomeMap();

                // 밤일 때는 맵 초기화
                if (TimeManager.Instance.IsNight)
                {
                    MapManager.Instance.ResetAllMaps();
                }
            }

            _player.SetPlayerInBase(true);
        }));

        QuestManager.Instance?.AddQuestAmount(QUEST_TYPE.GoToBase);
        _isRecallStarted = false;
        _isRecallInProgress = false;
    }
}
