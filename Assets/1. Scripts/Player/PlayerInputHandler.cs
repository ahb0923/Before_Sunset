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

    #region About Event Handling
    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        // 이동 관련
        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCanceled;

        // 스윙 관련
        _inputActions.Player.Swing.started += OnSwingStarted;
        _inputActions.Player.Swing.canceled += OnSwingCanceled;
    }
    private void OnDisable()
    {
        // 이동 관련
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;

        // 스윙 관련
        _inputActions.Player.Swing.started -= OnSwingStarted;
        _inputActions.Player.Swing.canceled -= OnSwingCanceled;
    }
    private void OnDestroy()
    {
        // 이동 관련
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;

        // 스윙 관련
        _inputActions.Player.Swing.started -= OnSwingStarted;
        _inputActions.Player.Swing.canceled -= OnSwingCanceled;
    }
    #endregion

    /// <summary>
    /// 방향키로 움직이고 있을 때, 벡터 값을 저장
    /// </summary>
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// 모든 방향키가 눌리고 있지 않을 때, 0 벡터 저장
    /// </summary>
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    /// <summary>
    /// 광산 지역에서 왼쪽 마우스 클릭 시에, 스윙 불 값 true로 변경
    /// </summary>
    private void OnSwingStarted(InputAction.CallbackContext context)
    {
        if (!_stateHandler.IsInMiningArea) return;
        
        IsSwing = true;
    }

    /// <summary>
    /// 광산 지역에서 왼쪽 마우스 클릭 시에, 스윙 불 값 true로 변경
    /// </summary>
    private void OnSwingCanceled(InputAction.CallbackContext context)
    {
        IsSwing = false;
    }
}
