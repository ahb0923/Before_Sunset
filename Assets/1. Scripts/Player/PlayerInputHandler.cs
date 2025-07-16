using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _actions;

    #region Event Cancel
    private void OnDisable()
    {
        _actions.Interaction.Disable();

        _actions.Interaction.Inventory.started -= OnInventoryStarted;
        _actions.Interaction.Build.started -= OnBuildStarted;
        _actions.Interaction.DestroyMode.started -= OnDestroyModeStarted;
        _actions.Interaction.ReturnHome.started -= OnReturnHomeStarted;
    }
    #endregion

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

    }

    public void Init(BasePlayer player)
    {
        _actions = player.InputActions;
        _actions.Interaction.Inventory.started += OnInventoryStarted;
        _actions.Interaction.Build.started += OnBuildStarted;
        _actions.Interaction.DestroyMode.started += OnDestroyModeStarted;
        _actions.Interaction.ReturnHome.started += OnReturnHomeStarted;

        _actions.Interaction.Enable();
    }
}
