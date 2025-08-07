using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Smelter : MonoBehaviour, IPoolable, IDamageable, IInteractable
{
    public int smelterID;
    public SmelterDatabase smelterData;
    private BuildInfo _buildInfo;
    public Vector2Int size = new Vector2Int(1, 1);

    public Item InputItem { get; private set; }
    public Item OutputItem { get; private set; }
    
    private Coroutine _smeltCoroutine;
    public bool isSmelting;
    public event Action<float> OnSmeltingProgress;
    public float elapsed = 0f;

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private void Awake()
    {
        smelterData = DataManager.Instance.SmelterData.GetById(smelterID);
        if (_buildInfo == null)
            _buildInfo = Helper_Component.GetComponentInChildren<BuildInfo>(gameObject);
        _buildInfo.Init(smelterID, size);
        animator = Helper_Component.GetComponentInChildren<Animator>(gameObject);
    }

    /// <summary>
    /// 제련 시도 메서드
    /// </summary>
    public void TrySmelt(float smeltedTime = 0f)
    {
        if (isSmelting)
            return;

        if (InputItem == null)
            StopSmelting();
        
        if (!isSmelting && InputItem != null)
            StartSmelting(smeltedTime);
    }

    private void InitAnim()
    {
        animator.SetBool("IsDone", false);
        animator.SetBool("IsMelting", false);
        animator.SetBool("IsClear", false);
        animator.SetBool("IsCancel", false);
    }

    /// <summary>
    /// 제련 시작 메서드
    /// </summary>
    private void StartSmelting(float smeltedTime = 0f)
    {
        var mineralData = DataManager.Instance.MineralData.GetById(InputItem.Data.id);
        
        if (OutputItem == null || mineralData.ingotId == OutputItem.Data.id)
            _smeltCoroutine = StartCoroutine(C_Smelt(mineralData, smeltedTime));
    }
    
    public void StopSmelting()
    {
        if (_smeltCoroutine != null)
        {
            StopCoroutine(_smeltCoroutine);
            _smeltCoroutine = null;
        }

        if (OutputItem != null)
        {
            InitAnim();
            animator.SetBool("IsDone", true);
        }
        else
        {
            InitAnim();
            animator.SetBool("IsCancel", true);
        }
        isSmelting = false;

        OnSmeltingProgress?.Invoke(0);
        RefreshUI();
    }
    
    /// <summary>
    /// 제련 메서드
    /// </summary>
    /// <param name="mineralData"></param>
    /// <returns></returns>
    private IEnumerator C_Smelt(MineralDatabase mineralData, float smeltedTime)
    {
        isSmelting = true;
        InitAnim();
        animator.SetBool("IsMelting", true);

        //float duration = mineralData.smeltingTime;
        float duration = 1.0f;
        elapsed = smeltedTime;

        while (elapsed < duration)
        {
            if (InputItem == null) break;
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            OnSmeltingProgress?.Invoke(progress);

            yield return null;
        }

        if (InputItem == null)
        {
            if (OutputItem != null)
            {
                InitAnim();
                animator.SetBool("IsDone", true);
            }
            else
            {
                InitAnim();
                animator.SetBool("IsCancel", true);
            }
            
            isSmelting = false;
            OnSmeltingProgress?.Invoke(0);
            RefreshUI();
            yield break;
        }

        InputItem.stack--;
        if (InputItem.stack == 0)
            InputItem = null;

        if (OutputItem == null)
            OutputItem = new Item(DataManager.Instance.ItemData.GetById(mineralData.ingotId));
        OutputItem.stack++;
        
        OnSmeltingProgress?.Invoke(0);
        RefreshUI();

        if (InputItem != null)
        {
            StartSmelting();
        }
        else
        {
            InitAnim();
            animator.SetBool("IsDone", true);
            isSmelting = false;
        }
    }

    /// <summary>
    /// 제련 후 UI 새로고침할때 사용되는 메서드
    /// </summary>
    private void RefreshUI()
    {
        UIManager.Instance.SmelterUI.RefreshSlots();
    }

    /// <summary>
    /// 슬롯에서 제련소 아이템을 바꿔줄때 사용될 메서드
    /// </summary>
    /// <param name="item"></param>
    public void SetInputItem(Item item, float smeltedTime = 0f)
    {
        InputItem = item;
        TrySmelt();
    }

    /// <summary>
    /// 슬롯에서 제련소 아이템을 바꿔줄때 사용될 메서드
    /// </summary>
    /// <param name="item"></param>
    public void SetOutputItem(Item item)
    {
        OutputItem = item;
        TrySmelt();
    }

    public int GetId()
    {
        return smelterID;
    }

    public void OnInstantiate()
    {
        isSmelting = false;
    }

    public void OnGetFromPool()
    {
        RenderUtil.SetSortingOrderByY(spriteRenderer);
    }

    public void OnReturnToPool()
    {
        if (UIManager.Instance.SmelterUI.CurrentSmelter() == this && UIManager.Instance.SmelterUI.isActiveAndEnabled)
        {
            UIManager.Instance.SmelterUI.Close();
        }

        if (UIManager.Instance.DismantleUI.SelectedSmelter() == this && UIManager.Instance.DismantleUI.isActiveAndEnabled)
        {
            UIManager.Instance.DismantleUI.Close();
        }
    }

    [ContextMenu("부숴버리기~")]
    public void DestroySmelter()
    {
        var refundReq = DataManager.Instance.SmelterData.GetById(smelterID).buildRequirements;
        foreach (var kvp in refundReq)
        {
            int refundAmount = Mathf.FloorToInt(kvp.Value * 0.9f);
            if (refundAmount > 0)
            {
                InventoryManager.Instance.Inventory.AddItem(
                    DataManager.Instance.ItemData.GetIdByName(kvp.Key),
                    refundAmount
                );
            }
        }

        // 실제 파괴 처리
        OnDamaged(new Damaged
        {
            Value = 99999,
            Attacker = gameObject,
            Victim = gameObject
        });
    }
    public void OnDamaged(Damaged damaged)
    {
        // 1) 부서지는 애니메이션
        // 2) 여기서 제련중이던 아이템 필드에 드롭
        // 3) 있다면 제련중이던 실제 정보값들 저장 위치 초기화
        // 4) 파괴 될 경우 50% 의 아이템 손실
   
        if (InputItem != null)
        {
            //int inputDropAmount = damaged.Attacker != gameObject ? InputItem.stack : InputItem.stack /= 2;
            ItemDropManager.Instance.DropItem(InputItem.Data.id, InputItem.stack, transform);
        }

        if (OutputItem != null)
        {
            //int outputDropAmount = damaged.Attacker != gameObject ? OutputItem.stack : OutputItem.stack /= 2;
            ItemDropManager.Instance.DropItem(OutputItem.Data.id, OutputItem.stack, transform);
        }

        StopAllCoroutines();
        DefenseManager.Instance.RemoveObstacle(transform);
        PoolManager.Instance.ReturnToPool(smelterID, gameObject);
    }

    public void Interact()
    {
        if (BuildManager.Instance.IsOnDestroy)
        {
            UIManager.Instance.DismantleUI.OpenDismantleUI(this);
            return;
        }
        else
        {
            UIManager.Instance.SmelterUI.Open(this);
        }
    }

    public bool IsInteractable(Vector3 playerPos, float range, BoxCollider2D playerCollider)
    {
        return true;
    }

    public int GetObejctSize()
    {
        return 1;
    }
}