using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private PlayerStateHandler _stateHandler;

    public Vector2 MoveInput { get; private set; }
    public bool IsSwing { get; private set; }

    public event Action OnInventoryToggle;
    public event Action OnBuildMode;
    public event Action OnUnBuildMode;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _stateHandler = GetComponent<PlayerStateHandler>();
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

        _inputActions.Player.Disable();
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
}
