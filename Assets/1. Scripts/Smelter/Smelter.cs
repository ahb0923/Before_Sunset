using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Smelter : MonoBehaviour, IPoolable, IDamageable
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

    [SerializeField] private Smelter_Interaction _interaction;

    public SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        smelterData = DataManager.Instance.SmelterData.GetById(smelterID);
        if(_interaction==null)
            _interaction = Helper_Component.GetComponentInChildren<Smelter_Interaction>(gameObject);
        _interaction.Init(this);
        if (_buildInfo == null)
            _buildInfo = Helper_Component.GetComponentInChildren<BuildInfo>(gameObject);
        _buildInfo.Init(smelterID, size);
    }

    /// <summary>
    /// 제련 시도 메서드
    /// </summary>
    public void TrySmelt(float smeltedTime = 0f)
    {
        if (_smeltCoroutine != null)
        {
            return;
        }
        
        if (!isSmelting)
        {
            StartSmelting(smeltedTime);
        }
    }

    /// <summary>
    /// 제련 시작 메서드
    /// </summary>
    private void StartSmelting(float smeltedTime = 0f)
    {
        if (InputItem == null)
        {
            return;
        }
        
        var mineralData = DataManager.Instance.MineralData.GetById(InputItem.Data.id);
        
        if (OutputItem == null || mineralData.ingotId == OutputItem.Data.id)
        {
            _smeltCoroutine = StartCoroutine(C_Smelt(mineralData, smeltedTime));
        }
        else
        {
            isSmelting = false;
        }
    }
    
    /// <summary>
    /// 제련 메서드
    /// </summary>
    /// <param name="mineralData"></param>
    /// <returns></returns>
    private IEnumerator C_Smelt(MineralDatabase mineralData, float smeltedTime)
    {   
        isSmelting = true;

        float duration = mineralData.smeltingTime;
        elapsed = smeltedTime;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            OnSmeltingProgress?.Invoke(progress);

            yield return null;
        }

        InputItem.stack--;
        if (InputItem.stack <= 0)
        {
            InputItem = null;
        }

        if (OutputItem == null)
        {
            OutputItem = new Item(DataManager.Instance.ItemData.GetId(mineralData.ingotId));
        }
        OutputItem.stack++;
        _smeltCoroutine = null;
        OnSmeltingProgress?.Invoke(0);
        RefreshUI();

        if (InputItem != null)
        {
            StartSmelting();
        }
        else
        {
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
    }

    public void OnDamaged(Damaged damaged)
    {
        // 1) 부서지는 애니메이션
        // 2) 여기서 제련중이던 아이템 필드에 드롭
        // 3) 있다면 제련중이던 실제 정보값들 저장 위치 초기화
        // 4) 파괴 될 경우 50% 의 아이템 손실


        StopAllCoroutines();
        PoolManager.Instance.ReturnToPool(smelterID, gameObject);
    }
}