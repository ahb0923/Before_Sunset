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

        float currentRange = GetEffectiveRange();

        Collider2D col = Physics2D.OverlapPoint(mousePos2D);
        if (col != null)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distanceToEdge = GetDistanceToColliderEdge(col, playerPos);
                if (distanceToEdge <= currentRange)
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

        float currentRange = GetEffectiveRange();

        Collider2D col = Physics2D.OverlapPoint(mousePos2D);
        if (col != null)
        {
            var interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distanceToEdge = GetDistanceToColliderEdge(col, playerPos);
                if (distanceToEdge <= currentRange)
                {
                    interactable.Interact();
                }
            }
        }
    }

    void SetCustomCursor(Texture2D cursor)
    {
        if (currentCursor != cursor)
        {
            // hotspot 조절 필요하면 여기서 조절하세요. (기본은 좌상단 (0,0))
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
            currentCursor = cursor;
        }
    }

    float GetEffectiveRange()
    {
        return (stateHandler != null && stateHandler.IsInMiningArea) ? 0.5f : interactionRange;
    }

    float GetDistanceToColliderEdge(Collider2D col, Vector3 fromPos)
    {
        Vector2 fromPos2D = new Vector2(fromPos.x, fromPos.y);

        if (col is CircleCollider2D circle)
        {
            Vector2 center = (Vector2)circle.transform.position + circle.offset;
            float radius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);
            float distToCenter = Vector2.Distance(fromPos2D, center);
            float distanceToEdge = Mathf.Max(0f, distToCenter - radius);
            return distanceToEdge;
        }
        else if (col is BoxCollider2D box)
        {
            Vector2 closest = col.ClosestPoint(fromPos2D);
            float distanceToEdge = Vector2.Distance(fromPos2D, closest);
            return distanceToEdge;
        }
        else
        {
            Vector2 closest = col.ClosestPoint(fromPos2D);
            float distanceToEdge = Vector2.Distance(fromPos2D, closest);
            return distanceToEdge;
        }
    }
}
