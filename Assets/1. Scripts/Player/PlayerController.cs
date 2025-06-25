using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    //public Animator animator;
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

        //animator.SetFloat("LookX", lookDir.x);
        //animator.SetFloat("LookY", lookDir.y);

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        float snappedAngle = GetSnappedAngle(angle);
        transform.rotation = Quaternion.Euler(0f, 0f, snappedAngle);

        if (snappedAngle == 180f)
            transform.localScale = new Vector3(1f, -1f, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);

        if (isSwinging)
        {
            rb.velocity = Vector2.zero;
            //animator.SetBool("isWalking", false);
            return;
        }

        Vector2 move = input.MoveInput;
        rb.velocity = move * moveSpeed;

        //animator.SetBool("isWalking", move.sqrMagnitude > 0.01f);

        if (input.IsSwing && !isSwinging)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;
        //animator.SetBool("isSwinging", true);
        yield return new WaitForSeconds(0.5f);
        //animator.SetBool("isSwinging", false);
        isSwinging = false;
    }
    private float GetSnappedAngle(float angle)
    {
        angle = (angle + 360f) % 360f;

        if (angle >= 45f && angle < 135f)
            return 90f;
        else if (angle >= 135f && angle < 225f)
            return 180f;
        else if (angle >= 225f && angle < 315f)
            return 270f;
        else
            return 0f;
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
