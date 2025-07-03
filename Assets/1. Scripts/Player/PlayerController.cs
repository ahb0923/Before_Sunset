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

    private void HandleLookAndFlip()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 lookDir = (mouseWorld - transform.position).normalized;

        bool isUp = false;
        bool isDown = false;
        bool isSide = false;

        if (Mathf.Abs(lookDir.x) > Mathf.Abs(lookDir.y))
        {
            isSide = true;
            _spriteRenderer.flipX = lookDir.x > 0;
        }
        else
        {
            _spriteRenderer.flipX = false;
            isUp = lookDir.y > 0;
            isDown = !isUp;
        }

        animator.SetBool("isUp", isUp);
        animator.SetBool("isDown", isDown);
        animator.SetBool("isSide", isSide);
    }

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
        CircleCollider2D playerCollider = GetComponent<CircleCollider2D>();
        if (playerCollider == null) return;

        Vector2 playerPos2D = (Vector2)transform.position;
        float playerRadius = playerCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        float miningRange = 0.5f;
        float checkRadius = playerRadius + miningRange;

        Vector2 dir = Vector2.right;
        if (animator.GetBool("isUp")) dir = Vector2.up;
        else if (animator.GetBool("isDown")) dir = Vector2.down;
        else if (animator.GetBool("isSide")) dir = _spriteRenderer.flipX ? Vector2.left : Vector2.right;

        Vector2 checkPos = playerPos2D + dir * checkRadius;
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, miningRange, LayerMask.GetMask("Ore"));

        foreach (var hit in hits)
        {
            OreController ore = hit.GetComponent<OreController>();
            if (ore != null)
            {
                if (ore.CanBeMined(pickaxePower))
                {
                    bool destroyed = ore.Mine(miningDamage);
                    Debug.Log(destroyed ? "광석이 파괴됨!" : "광석에 데미지 입힘");
                }
                else
                {
                    Debug.Log("곡괭이 파워 부족!");
                }
                break;
            }
        }
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
