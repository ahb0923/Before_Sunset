using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject buildModeUI;
    [SerializeField] private GameObject unbuildModeUI;
    [SerializeField] private int pickaxePower = 1;
    [SerializeField] private int miningDamage = 1;


    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private PlayerInputHandler _input;
    private bool _isSwinging = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _input = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        _input.OnInventoryToggle += ToggleInventory;
        _input.OnBuildMode += EnterBuildMode;
        _input.OnUnBuildMode += EnterUnBuildMode;
    }

    private void OnDestroy()
    {
        _input.OnInventoryToggle -= ToggleInventory;
        _input.OnBuildMode -= EnterBuildMode;
        _input.OnUnBuildMode -= EnterUnBuildMode;
    }

    private void Update()
    {
        HandleLookAndFlip();
        HandleMovement();
        HandleSwing();
    }

    // 마우스 방향을 바라보도록 회전
    private void HandleLookAndFlip()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 lookDir = (mouseWorld - transform.position).normalized;

        animator.SetFloat("LookX", lookDir.x);
        animator.SetFloat("LookY", lookDir.y);

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        float snappedAngle = GetSnappedAngle(angle);
        transform.rotation = Quaternion.Euler(0f, 0f, snappedAngle);

        transform.localScale = (snappedAngle == 180f)
            ? new Vector3(1f, -1f, 1f)
            : new Vector3(1f, 1f, 1f);
    }

    // 이동 처리
    private void HandleMovement()
    {
        if (_isSwinging)
        {
            _rigidbody.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        Vector2 move = _input.MoveInput;
        _rigidbody.velocity = move * moveSpeed;
        animator.SetBool("isWalking", move.sqrMagnitude > 0.01f);
    }

    // 스윙 처리
    private void HandleSwing()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (_input.IsSwing && !_isSwinging)
        {
            StartCoroutine(C_Swing());
        }
    }

    private IEnumerator C_Swing()
    {
        _isSwinging = true;
        animator.SetBool("isSwinging", true);
        TryMineOre();
        yield return new WaitForSeconds(1.2f);
        animator.SetBool("isSwinging", false);
        _isSwinging = false;
    }
    private void TryMineOre()
    {
        if (_rigidbody == null) return;

        // 플레이어 위치 및 콜라이더 반지름 계산
        CircleCollider2D playerCollider = GetComponent<CircleCollider2D>();
        if (playerCollider == null)
        {
            Debug.LogWarning("플레이어에 CircleCollider2D가 필요합니다!");
            return;
        }

        Vector2 playerPos2D = (Vector2)transform.position;
        float playerRadius = playerCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        // 채굴 가능 거리 (플레이어 외곽선부터 얼마나 떨어져야 할지)
        float miningRange = 0.5f;

        // 플레이어 외곽 기준 반경
        float checkRadius = playerRadius + miningRange;

        // 플레이어가 바라보는 방향 (단위 벡터)
        float angle = transform.rotation.eulerAngles.z;
        Vector2 dir;
        if (Mathf.Approximately(angle, 90f)) dir = Vector2.up;
        else if (Mathf.Approximately(angle, 180f)) dir = Vector2.left;
        else if (Mathf.Approximately(angle, 270f)) dir = Vector2.down;
        else dir = Vector2.right;

        // 광석 탐색 위치는 플레이어 외곽선에서 miningRange만큼 떨어진 점
        Vector2 checkPos = playerPos2D + dir * checkRadius;

        // 특정 반경 내 광석 찾기 (레이어 "Ore"만)
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, miningRange, LayerMask.GetMask("Ore"));

        foreach (var hit in hits)
        {
            OreController ore = hit.GetComponent<OreController>();
            if (ore != null)
            {
                if (ore.CanBeMined(pickaxePower))
                {
                    bool destroyed = ore.Mine(miningDamage);
                    if (destroyed)
                        Debug.Log("광석이 파괴됨!");
                    else
                        Debug.Log("광석에 데미지 입힘");
                }
                else
                {
                    Debug.Log("곡괭이 파워 부족!");
                }
                // 첫 번째 광석만 처리하고 종료
                break;
            }
        }
    }


    // 마우스 방향을 4방향으로 스냅
    private float GetSnappedAngle(float angle)
    {
        angle = (angle + 360f) % 360f;

        if (angle >= 45f && angle < 135f) return 90f;
        if (angle >= 135f && angle < 225f) return 180f;
        if (angle >= 225f && angle < 315f) return 270f;
        return 0f;
    }

    private void ToggleInventory()
    {
        Debug.Log("인벤토리 열기/닫기");
    }

    private void EnterBuildMode()
    {
        if (buildModeUI != null)
        {
            bool isActive = !buildModeUI.activeSelf;
            buildModeUI.SetActive(isActive);
            if (isActive && unbuildModeUI != null)
                unbuildModeUI.SetActive(false);
        }
    }

    private void EnterUnBuildMode()
    {
        if (unbuildModeUI != null)
        {
            bool isActive = !unbuildModeUI.activeSelf;
            unbuildModeUI.SetActive(isActive);
            if (isActive && buildModeUI != null)
                buildModeUI.SetActive(false);
        }
    }
}
