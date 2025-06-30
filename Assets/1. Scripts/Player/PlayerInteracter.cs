using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Camera mainCamera;
    public Texture2D defaultCursor;
    public Texture2D interactCursor;
    public Texture2D outOfRangeCursor;
    public float interactionRange = 5.0f;

    private Texture2D currentCursor;
    private Vector2 lastMousePos;
    private Vector3 lastPlayerPos;

    private PlayerStateHandler stateHandler;

    void Awake()
    {
        stateHandler = GetComponent<PlayerStateHandler>();
    }

    void Update()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        Vector3 currentPlayerPos = transform.position;

        if (currentMousePos != lastMousePos || currentPlayerPos != lastPlayerPos)
        {
            UpdateCursor(currentMousePos, currentPlayerPos);
            lastMousePos = currentMousePos;
            lastPlayerPos = currentPlayerPos;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryInteract(currentPlayerPos);
        }
    }

    void UpdateCursor(Vector2 screenPos, Vector3 playerPos)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        float currentRange = stateHandler != null && stateHandler.IsInMiningArea ? 0.5f : interactionRange;

        Collider2D col = Physics2D.OverlapPoint(mousePos2D);
        if (col != null)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (interactable.IsInteractable(playerPos, currentRange))
                {
                    SetCustomCursor(interactCursor);
                }
                else
                {
                    SetCustomCursor(outOfRangeCursor);
                }
                return;
            }
        }

        SetCustomCursor(defaultCursor);
    }

    void TryInteract(Vector3 playerPos)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        float currentRange = stateHandler != null && stateHandler.IsInMiningArea ? 0.5f : interactionRange;

        Collider2D col = Physics2D.OverlapPoint(mousePos2D);
        if (col != null)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable != null && interactable.IsInteractable(playerPos, currentRange))
            {
                interactable.Interact();
            }
        }
    }

    void SetCustomCursor(Texture2D cursor)
    {
        if (currentCursor != cursor)
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
            currentCursor = cursor;
        }
    }
}
