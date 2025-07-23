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

    private BasePlayer _player;
    private CircleCollider2D playerCollider;

    private IInteractable currentTarget;
    public IInteractable CurrentTarget => currentTarget;

    void Awake()
    {
        _player = GetComponent<BasePlayer>();
        playerCollider = GetComponent<CircleCollider2D>();
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
    }

    void UpdateCursor(Vector2 screenPos, Vector3 playerPos)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        float currentRange = GetEffectiveRange();

        Collider2D col = Physics2D.OverlapPoint(mousePos2D);
        if (col != null && col.TryGetComponent<IInteractable>(out var interactable))
        {
            float distanceToEdge = GetDistanceToColliderEdge(col, playerPos);
            if (distanceToEdge <= currentRange)
            {
                currentTarget = interactable;
                SetCustomCursor(interactCursor);
            }
            else
            {
                currentTarget = null;
                SetCustomCursor(outOfRangeCursor);
            }
            return;
        }

        currentTarget = null;
        SetCustomCursor(defaultCursor);
    }

    void SetCustomCursor(Texture2D cursor)
    {
        if (currentCursor != cursor)
        {
            Vector2 hotspot = Vector2.zero;

            if (cursor != defaultCursor)
            {
                hotspot = new Vector2(cursor.width / 2f, cursor.height / 2f);
            }
            else
            {
                hotspot = Vector2.zero;
            }

            Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
            currentCursor = cursor;
        }
    }


    float GetEffectiveRange()
    {
        return (_player != null && !_player.IsInBase) ? 1f : interactionRange;
    }

    float GetDistanceToColliderEdge(Collider2D col, Vector3 fromPos)
    {
        Vector2 fromPos2D = new Vector2(fromPos.x, fromPos.y);
        Vector2 closest = col.ClosestPoint(fromPos2D);
        float distance = Vector2.Distance(fromPos2D, closest);

        if (playerCollider != null)
        {
            float playerRadius = playerCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            distance -= playerRadius;
        }

        return Mathf.Max(0f, distance);
    }
}
