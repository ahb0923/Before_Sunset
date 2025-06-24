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
        input.OnDestroyMode += EnterDestroyMode;
    }

    private void OnDestroy()
    {
        input.OnInventoryToggle -= ToggleInventory;
        input.OnBuildMode -= EnterBuildMode;
        input.OnDestroyMode -= EnterDestroyMode;
    }

    private void Update()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 lookDir = (mouseWorld - transform.position).normalized;

        animator.SetFloat("LookX", lookDir.x);
        animator.SetFloat("LookY", lookDir.y);

        if (lookDir.x > 0.1f) sr.flipX = false;
        else if (lookDir.x < -0.1f) sr.flipX = true;

        if (isSwinging)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        Vector2 move = input.MoveInput;
        rb.velocity = move * moveSpeed;

        animator.SetBool("isWalking", move.sqrMagnitude > 0.01f);

        if (input.IsSwing && !isSwinging)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;
        animator.SetBool("isSwinging", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isSwinging", false);
        isSwinging = false;
    }

    private void ToggleInventory()
    {
        Debug.Log("인벤토리 열기/닫기");
        // 인벤토리 UI 토글 처리 코드 여기에
    }

    private void EnterBuildMode()
    {
        Debug.Log("건설 모드 진입");
        // 건설 모드 진입 처리 코드 여기에
    }

    private void EnterDestroyMode()
    {
        Debug.Log("해체 모드 진입");
        // 해체 모드 진입 처리 코드 여기에
    }
}
