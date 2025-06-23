using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public bool IsInteracting { get; private set; }

    public System.Action OnInventoryToggle;
    public System.Action OnBuildMode;
    public System.Action OnDestroyMode;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        // 이동
        inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => MoveInput = Vector2.zero;

        // 상호작용 (공격 등)
        inputActions.Player.Interaction.performed += _ => IsInteracting = true;
        inputActions.Player.Interaction.canceled += _ => IsInteracting = false;

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