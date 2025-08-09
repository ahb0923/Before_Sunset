using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractManager : MonoSingleton<InteractManager>
{
    private static readonly List<RaycastResult> _uiRayResults = new();

    [Header("Cursor Textures")]
    public Texture2D defaultCursor;
    public Texture2D dismantleCursor;
    public Texture2D interactCursor;
    public Texture2D outOfRangeCursor;

    [Header("Interaction Layers")]
    public LayerMask interactableLayerMask = -1;

    [SerializeField] private InteractImage aimObject;
    public InteractImage AimObject { get => aimObject; }

    private Camera _mainCamera;
    private BoxCollider2D _playerCollider;
    private IInteractable _prevTarget;
    private IInteractable _currentTarget;


    private Vector2 _defaultHotspot = Vector2.zero;
    private Texture2D _currentCursor;
    public IInteractable GetCurrentTarget() => _currentTarget;
    public IInteractable GetPrevTarget() => _prevTarget;

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

        if (IsPointerOverRealUI())
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
    public bool IsPointerOverRealUI()
    {
        if (EventSystem.current == null) return false;

        var pointer = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        _uiRayResults.Clear();
        EventSystem.current.RaycastAll(pointer, _uiRayResults);

        // 스크림(혹은 무시할 UI)만 걸린 경우는 제외
        for (int i = _uiRayResults.Count - 1; i >= 0; i--)
        {
            var go = _uiRayResults[i].gameObject;
            if (go.GetComponent<IgnoreCursorUI>() != null)
                _uiRayResults.RemoveAt(i);
        }

        return _uiRayResults.Count > 0;
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
        float range = (interactable is OreController) ? 1.0f : 5.0f;

        if (!ReferenceEquals(_currentTarget, interactable))
        {
            _prevTarget = _currentTarget;
            _currentTarget = interactable;
        }

        if (interactable.IsInteractable(MapManager.Instance.Player.transform.position, range, _playerCollider))
        {
            aimObject.gameObject.SetActive(true);
            aimObject.SetNearCursor(interactable);
        }
        else
        {
            aimObject.gameObject.SetActive(true);
            aimObject.SetFarCursor(interactable);
        }
    }
    
    /// <summary> 커서 설정 + 현재 타겟 갱신 </summary>
    private void SetCursor(Texture2D texture, Vector2 hotspot, IInteractable target)
    {
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
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

    public void OffPrevTargetAttackArea()
    {
        if(_prevTarget is BaseTower prevTower && !ReferenceEquals(_prevTarget, _currentTarget))
            _prevTarget.OffAttackArea();
    }
}
