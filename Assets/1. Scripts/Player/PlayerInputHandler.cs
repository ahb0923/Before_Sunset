using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public bool IsSwing { get; private set; }

    public event Action OnInventoryToggle;
    public event Action OnBuildMode;
    public event Action OnUnBuildMode;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        // 이동
        inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => MoveInput = Vector2.zero;

        // 스윙
        inputActions.Player.Swing.performed += _ => IsSwing = true;
        inputActions.Player.Swing.canceled += _ => IsSwing = false;

        // 이벤트 입력
        inputActions.Player.Inventory.performed += _ => OnInventoryToggle?.Invoke();
        inputActions.Player.Build.performed += _ => OnBuildMode?.Invoke();
        inputActions.Player.UnBuild.performed += _ => OnUnBuildMode?.Invoke();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}
