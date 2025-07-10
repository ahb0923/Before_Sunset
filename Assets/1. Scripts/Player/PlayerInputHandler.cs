using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    public PlayerStateHandler _stateHandler;

    public Vector2 MoveInput { get; private set; }
    public bool IsSwing { get; private set; }

    public event Action OnInventoryToggle;
    public event Action OnBuildMode;
    public event Action OnUnBuildMode;

    private bool isReturnKeyHeld = false;
    private float returnKeyHeldTime = 0f;
    private bool isRecallStarted = false;
    [SerializeField] private float holdTimeToTriggerRecall = 2f;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCanceled;

        _inputActions.Player.Swing.performed += OnSwingPerformed;
        _inputActions.Player.Swing.canceled += OnSwingCanceled;

        _inputActions.Player.Inventory.performed += OnInventoryPerformed;
        _inputActions.Player.Build.performed += OnBuildPerformed;
        _inputActions.Player.UnBuild.performed += OnUnBuildPerformed;

        _inputActions.Player.ReturnHome.performed += OnReturnHomePerformed;
        _inputActions.Player.ReturnHome.canceled += OnReturnHomeCanceled;
        
        UIManager.Instance.RecallUI.OnCountdownFinished += OnRecallCountdownFinished;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;

        _inputActions.Player.Swing.performed -= OnSwingPerformed;
        _inputActions.Player.Swing.canceled -= OnSwingCanceled;

        _inputActions.Player.Inventory.performed -= OnInventoryPerformed;
        _inputActions.Player.Build.performed -= OnBuildPerformed;
        _inputActions.Player.UnBuild.performed -= OnUnBuildPerformed;

        _inputActions.Player.ReturnHome.performed -= OnReturnHomePerformed;
        _inputActions.Player.ReturnHome.canceled -= OnReturnHomeCanceled;

        UIManager.Instance.RecallUI.OnCountdownFinished -= OnRecallCountdownFinished;

        _inputActions.Player.Disable();
    }

    private void Update()
    {
        if (isRecallStarted) return;

        if (isReturnKeyHeld)
        {
            returnKeyHeldTime += Time.deltaTime;
            UIManager.Instance.RecallUI.UpdateHoldProgress(returnKeyHeldTime / holdTimeToTriggerRecall);

            if (returnKeyHeldTime >= holdTimeToTriggerRecall)
            {
                StartRecall();
            }
        }
    }

    private void StartRecall()
    {
        isRecallStarted = true;
        isReturnKeyHeld = false;

        if (AuraHandler.Instance != null)
            AuraHandler.Instance.Show();

        UIManager.Instance.RecallUI.StartRecallCountdown();
    }

    private IEnumerator DelayedRecall()
    {
        yield return StartCoroutine(ScreenFadeController.Instance.FadeInOut(() =>
        {
            UIManager.Instance.RecallUI.CloseRecall();
            MapManager.Instance.ReturnToHomeMap();
        }));

        isRecallStarted = false;
        returnKeyHeldTime = 0f;


        if (AuraHandler.Instance != null)
            AuraHandler.Instance.Hide();

        if (_stateHandler != null)
            _stateHandler.ExitMiningArea();
    }

    private void OnReturnHomePerformed(InputAction.CallbackContext context)
    {
        if (isRecallStarted) return;
        if (_stateHandler != null && _stateHandler.IsInMiningArea)
        {
            if (IsOnMiningTrigger())
            {
                Debug.Log("광산 트리거 위에서는 귀환할 수 없습니다.");
                return;
            }

            isReturnKeyHeld = true;
            returnKeyHeldTime = 0f;

            UIManager.Instance.RecallUI.ShowRecallIcon();
        }
    }

    private void OnReturnHomeCanceled(InputAction.CallbackContext context)
    {
        if (isRecallStarted) return;
        if (_stateHandler != null && _stateHandler.IsInMiningArea)
        {
            isReturnKeyHeld = false;
            returnKeyHeldTime = 0f;
            UIManager.Instance.RecallUI.ShowRecallIcon();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnSwingPerformed(InputAction.CallbackContext context)
    {
        if (_stateHandler != null && _stateHandler.IsInMiningArea)
        {
            IsSwing = true;
        }
        else
        {
            IsSwing = false;
        }
    }

    private void OnSwingCanceled(InputAction.CallbackContext context)
    {
        IsSwing = false;
    }

    private void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        OnInventoryToggle?.Invoke();
    }

    private void OnBuildPerformed(InputAction.CallbackContext context)
    {
        OnBuildMode?.Invoke();
    }

    private void OnUnBuildPerformed(InputAction.CallbackContext context)
    {
        OnUnBuildMode?.Invoke();
    }

    private void OnRecallCountdownFinished()
    {
        StartCoroutine(DelayedRecall());
    }

    public bool IsRecallInProgress()
    {
        return isRecallStarted;
    }

    private bool IsOnMiningTrigger()
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var col in results)
        {
            if (col.TryGetComponent<MiningHandler>(out var handler))
            {
                return true;
            }
        }
        return false;
    }
}
