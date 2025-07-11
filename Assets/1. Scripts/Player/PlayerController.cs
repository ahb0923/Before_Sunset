using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject buildModeUI;
    [SerializeField] private GameObject unbuildModeUI;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PlayerInputHandler _input;

    private EquipmentDataHandler equipmentDataHandler;

    private bool _isSwinging = false;

    // 현재 장착 곡괭이 데이터 (초기값은 null)
    private EquipmentData _equippedPickaxe;

    private async void Start()
    {
        _input.OnInventoryToggle += ToggleInventory;
        _input.OnBuildMode += EnterBuildMode;
        _input.OnUnBuildMode += EnterUnBuildMode;

        if (equipmentDataHandler == null)
            equipmentDataHandler = new EquipmentDataHandler();

        await equipmentDataHandler.LoadAsyncLocal();

        _equippedPickaxe = equipmentDataHandler.GetById(700);

        if (_equippedPickaxe == null)
            Debug.LogError("초기 곡괭이 데이터가 없습니다!");
        else
            animator.speed = _equippedPickaxe.speed;
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

    private void LateUpdate()
    {
        RenderUtil.SetSortingOrderByY(_spriteRenderer);
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
            _spriteRenderer.flipX = lookDir.x < 0;
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

        yield return new WaitForSeconds(1.2f / (_equippedPickaxe?.speed ?? 1f)); // 곡괭이 속도 반영

        animator.SetBool("isSwinging", false);
        _isSwinging = false;
    }

    private void TryMineOre()
    {
        if (_equippedPickaxe == null) return;

        Vector2 playerPos2D = (Vector2)transform.position + Vector2.down * 0.1f;

        Vector2 dir = Vector2.right;
        if (animator.GetBool("isUp")) dir = Vector2.up;
        else if (animator.GetBool("isDown")) dir = Vector2.down;
        else if (animator.GetBool("isSide")) dir = _spriteRenderer.flipX ? Vector2.left : Vector2.right;

        float range = _equippedPickaxe.range;

        int oreLayerMask = LayerMask.GetMask("Ore", "Jewel");
        RaycastHit2D hit = Physics2D.Raycast(playerPos2D, dir, range, oreLayerMask);

        Debug.DrawRay(playerPos2D, dir * range, Color.red, 1f);

        if (hit.collider != null)
        {
            OreController ore = hit.collider.GetComponent<OreController>();
            if (ore != null)
            {
                if (ore.CanBeMined(_equippedPickaxe.crushingForce))
                {
                    bool destroyed = ore.Mine(_equippedPickaxe.damage);
                    Debug.Log(destroyed ? "광석이 파괴됨!" : "광석에 데미지 입힘");
                }
                else
                {
                    Debug.Log("곡괭이 파워 부족!");
                }
                return;
            }

            JewelController jewel = hit.collider.GetComponent<JewelController>();
            if (jewel != null)
            {
                jewel.OnMined();
                return;
            }

            Debug.Log("채굴 대상 없음");
        }
        else
        {
            Debug.Log("채광 범위 내에 광석/보석 없음");
        }
    }



    private void ToggleInventory()
    {
        InventoryManager.Instance.Inventory.Toggle();
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
