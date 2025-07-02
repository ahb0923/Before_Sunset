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
        // 플레이어가 바라보는 방향 가져오기
        float angle = transform.rotation.eulerAngles.z;
        Vector2 dir = Vector2.right;

        if (Mathf.Approximately(angle, 90f)) dir = Vector2.up;
        else if (Mathf.Approximately(angle, 180f)) dir = Vector2.left;
        else if (Mathf.Approximately(angle, 270f)) dir = Vector2.down;

        // 광석 감지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, LayerMask.GetMask("Ore"));

        if (hit.collider != null)
        {
            OreController ore = hit.collider.GetComponent<OreController>();
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
