using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractManager : MonoSingleton<InteractManager>
{
    [Header("Cursor Textures")]
    public Texture2D defaultCursor;
    public Texture2D dismantleCursor;
    public Texture2D interactCursor;
    public Texture2D outOfRangeCursor;

    [Header("Interaction Layers")]
    public LayerMask interactableLayerMask = -1;

    [SerializeField ] private InteractImage aimObject;
    public InteractImage AimObject { get => aimObject; }

    private Camera _mainCamera;
    private BoxCollider2D _playerCollider;
    private IInteractable _currentTarget;


    private Vector2 _defaultHotspot = Vector2.zero;
    private Texture2D _currentCursor;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null)
            DontDestroyOnLoad(this.gameObject);

        _mainCamera = Camera.main;

        if(defaultCursor != null)
            _currentCursor = defaultCursor;

        // 시작 시에 기본 커서
        SetCursor(_currentCursor, _defaultHotspot, null);
    }

    public void SetInGame()
    {
        _mainCamera = Camera.main;
        _playerCollider = MapManager.Instance.Player.transform.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (_mainCamera == null) return;

        // 게임창 포커스 잃었으면 기본 커서
        if (!Application.isFocused || IsMouseOutside())
        {
            SetCursor(null, Vector2.zero, null);
            return;
        }
        else 
            SetCursor(_currentCursor, _defaultHotspot, null);

        // UI 위면 기본 커서
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            SetCursor(_currentCursor, _defaultHotspot, null);
            return;
        }

        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, interactableLayerMask);
        aimObject.transform.position = mouseWorldPos;

        if (hit != null && hit.TryGetComponent(out IInteractable interactable))
        {
            aimObject.transform.position = hit.transform.position;
            HandleInteractable(interactable);
        }
        else
        {
            aimObject.gameObject.SetActive(false);
        }
    }

    public void SetCursorDestroyImage(bool isDestroy)
    {
        if (isDestroy) 
            _currentCursor = dismantleCursor;
        else
            _currentCursor = defaultCursor;
    }

    /// <summary> 상호 작용 시 커서 세팅 </summary>
    private void HandleInteractable(IInteractable interactable)
    {
        float range = (interactable is OreController) ? 1.5f : 5.0f;

        if (interactable.IsInteractable(MapManager.Instance.Player.transform.position, range, _playerCollider))
        {
            if (interactable != null) _currentTarget = interactable;
            aimObject.gameObject.SetActive(true);
            aimObject.SetNearCursor(interactable);
        }
        else
        {
            if (interactable != null) _currentTarget = interactable;
            aimObject.gameObject.SetActive(true);
            aimObject.SetFarCursor(interactable);
        }
    }
    
    /// <summary> 커서 설정 + 현재 타겟 갱신 </summary>
    private void SetCursor(Texture2D texture, Vector2 hotspot, IInteractable target)
    {
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        _currentTarget = target;
    }

    /// <summary> 커서 중앙 계산 </summary>
    private Vector2 GetCursorCenter(Texture2D tex)
    {
        if (tex == null) return Vector2.zero;
        return new Vector2(tex.width * 0.5f, tex.height * 0.5f);
    }

    /// <summary> 마우스가 화면 밖인지 체크 </summary>
    private bool IsMouseOutside()
    {
        Vector2 pos = Input.mousePosition;
        return pos.x < 0 || pos.y < 0 || pos.x > Screen.width || pos.y > Screen.height;
    }

    private void SetTargetingImage(Transform obj)
    {
        
    }

    public IInteractable GetCurrentTarget() => _currentTarget;
}
