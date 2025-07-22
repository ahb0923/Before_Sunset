using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D interactCursor;
    public Texture2D outOfRangeCursor;

    [Header("Interaction Layers")]
    public LayerMask interactableLayerMask = -1;

    private Camera _mainCamera;
    private BoxCollider2D _playerCollider;
    private IInteractable _currentTarget;

    void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_playerCollider == null)
            _playerCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        Collider2D col = Physics2D.OverlapPoint(mousePos2D, interactableLayerMask);

        if (col != null && col.TryGetComponent(out IInteractable interactable))
        {
            float range = (interactable is OreController || interactable is JewelController) ? 1.5f : 5.0f;
            if (interactable.IsInteractable(transform.position, range, _playerCollider))
            {
                Cursor.SetCursor(interactCursor, new Vector2(-0.5f, -0.5f), CursorMode.Auto);
                _currentTarget = interactable;
                return;
            }
            else
            {
                Cursor.SetCursor(outOfRangeCursor, new Vector2(-0.5f, -0.5f), CursorMode.Auto);
                _currentTarget = null;
                return;
            }
        }
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        _currentTarget = null;
    }

    public IInteractable GetCurrentTarget() => _currentTarget;
}
