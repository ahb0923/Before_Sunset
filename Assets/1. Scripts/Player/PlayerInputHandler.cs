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

    public Action OnInventoryToggle;
    public Action OnBuildMode;
    public Action OnDestroyMode;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        // 이동
        inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => MoveInput = Vector2.zero;

        // 스윙
        inputActions.Player.Swing.performed += _ => IsSwing = true;
        inputActions.Player.Swing.canceled += _ => IsSwing = false;

        // 인벤토리
        inputActions.Player.Inventory.performed += _ => OnInventoryToggle?.Invoke();

        // 건설
        inputActions.Player.Build.performed += _ => OnBuildMode?.Invoke();

        // 해체
        inputActions.Player.Destroy.performed += _ => OnDestroyMode?.Invoke();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}
