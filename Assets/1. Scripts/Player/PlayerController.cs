using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public Animator animator;
    public Camera mainCamera;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerInputHandler input;

    private bool isSwinging = false;

    public GameObject buildModeUI;
    public GameObject unbuildModeUI;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        input.OnInventoryToggle += ToggleInventory;
        input.OnBuildMode += EnterBuildMode;
        input.OnUnBuildMode += EnterUnBuildMode;
    }

    private void OnDestroy()
    {
        input.OnInventoryToggle -= ToggleInventory;
        input.OnBuildMode -= EnterBuildMode;
        input.OnUnBuildMode -= EnterUnBuildMode;
    }

    private void Update()
    {
        HandleLookAndFlip();
        HandleMovement();
        HandleSwing();
    }

    private void HandleLookAndFlip()
    {
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

    private void HandleMovement()
    {
        if (isSwinging)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        Vector2 move = input.MoveInput;
        rb.velocity = move * moveSpeed;
        animator.SetBool("isWalking", move.sqrMagnitude > 0.01f);
    }

    private void HandleSwing()
    {
        if (input.IsSwing && !isSwinging)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;
        animator.SetBool("isSwinging", true);
        yield return new WaitForSeconds(1.2f);
        animator.SetBool("isSwinging", false);
        isSwinging = false;
    }

    private float GetSnappedAngle(float angle)
    {
        angle = (angle + 360f) % 360f;

        if (angle >= 45f && angle < 135f) return 90f;
        else if (angle >= 135f && angle < 225f) return 180f;
        else if (angle >= 225f && angle < 315f) return 270f;
        else return 0f;
    }

    private void ToggleInventory()
    {
        Debug.Log("인벤토리 열기/닫기");
    }

    private void EnterBuildMode()
    {
        if (buildModeUI != null)
            buildModeUI.SetActive(!buildModeUI.activeSelf);
    }

    private void EnterUnBuildMode()
    {
        if (unbuildModeUI != null)
            unbuildModeUI.SetActive(!unbuildModeUI.activeSelf);
    }
}
