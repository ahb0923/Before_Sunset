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
        // 추가 예정
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
    #endregion

    #region Event Unsubscriptions
    private void OnDisable()
    {
        _actions.Interaction.Disable();

        _actions.Interaction.Inventory.started -= OnInventoryStarted;
        _actions.Interaction.Build.started -= OnBuildStarted;
        _actions.Interaction.DestroyMode.started -= OnDestroyModeStarted;
        _actions.Interaction.ReturnHome.performed -= OnReturnHomeStarted;
    }
    #endregion

    private void Update()
    {
        if (_player.IsInBase || _isRecallStarted || !_isHeldReturnKey) return;

        _heldTimer += Time.deltaTime;
        UIManager.Instance.RecallUI.UpdateHoldProgress(_heldTimer / _returnKeyHeldTime);

        if(_heldTimer >= _returnKeyHeldTime)
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
        _actions.Interaction.Inventory.started += OnInventoryStarted;
        _actions.Interaction.Build.started += OnBuildStarted;
        _actions.Interaction.DestroyMode.started += OnDestroyModeStarted;
        _actions.Interaction.ReturnHome.started += OnReturnHomeStarted;
        _actions.Interaction.ReturnHome.canceled += OnReturnHomeCanceled;
        _actions.Interaction.Enable();

        UIManager.Instance.RecallUI.OnCountdownFinished += OnRecallCountdownFinished;

        _isRecallStarted = false;
        _isHeldReturnKey = false;
        _heldTimer = 0f;
    }

    /// <summary>
    /// 귀환 키보드 일정 시간 눌렀을 때, 귀환 시작
    /// </summary>
    private void StartRecall()
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
                MapManager.Instance.ReturnToHomeMap();

            _player.SetPlayerInBase(true);
        }));

        QuestManager.Instance?.AddQuestAmount(QUEST_TYPE.GoToBase);
        _isRecallStarted = false;
        _isRecallInProgress = false;
    }
}
