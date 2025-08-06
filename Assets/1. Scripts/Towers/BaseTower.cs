using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum BUILDING_TYPE
{
    Normal,
    Upgrade
}
public enum TOWER_TYPE
{
    CooperTower,
    IronTower,
    DiaprismTower,
    HealTower,
    MagnetTower,
    TopazTower,
    RubyTower,
    AquamarineTower,
    Electricline
}
public class BaseTower : MonoBehaviour, IPoolable, IInteractable
{
    [Header(" [에디터 할당] ")]
    public TOWER_TYPE towerType;
    public int towerId;


    public Vector2Int buildSize = new Vector2Int(3, 3); // 1x1 또는 2x2 등


    public Collider2D mainCollider;
    public Collider2D _attackCollider;
    public Collider2D _buildCollider;
    public Collider2D _interactCollider;


    // state 는 ai로 접근
    public TowerAI ai;
    // stat Handler => 타워 스탯 관련
    public TowerStatHandler statHandler;
    // 공격 센서
    public TowerAttackSensor attackSensor;
    // ui 관련
    public TowerUI ui;
    // 건설 관련 (buildManager에서 이용)
    public BuildInfo buildInfo;
    // 발사체
    public GameObject projectile;

    // 공격 전략
    public IAttackStrategy attackStrategy;


    private void Reset()
    {
        mainCollider = Helper_Component.GetComponent<Collider2D>(gameObject);

        // 이동 필요
        _attackCollider = Helper_Component.FindChildComponent<Collider2D>(transform, "AttackArea");

        // 이동 필요
        _interactCollider = Helper_Component.FindChildComponent<Collider2D>(transform, "InteractArea");

        // 이동 필요?
        _buildCollider = Helper_Component.FindChildComponent<Collider2D>(transform, "BuildArea");


        ai = Helper_Component.GetComponent<TowerAI>(gameObject);
        statHandler = Helper_Component.GetComponent<TowerStatHandler>(gameObject);
        attackSensor = Helper_Component.GetComponentInChildren<TowerAttackSensor>(gameObject);
        ui = Helper_Component.GetComponentInChildren<TowerUI>(gameObject);
        buildInfo = Helper_Component.GetComponentInChildren<BuildInfo>(gameObject);
    }
    

    public void Init()
    {
        statHandler.Init(this, towerId);
        ui.Init(this);
        ai.Init(this);
        attackSensor.Init(this);
        InitAttackStrategy();
        buildInfo.Init(towerId, buildSize);
        ui.OffAttackArea();
    }

    public void InitAttackStrategy()
    {
        switch (towerType)
        {
            case TOWER_TYPE.CooperTower:
                attackStrategy = new AttackStrategy_CooperTower();
                break;
            case TOWER_TYPE.IronTower:
                attackStrategy = new AttackStrategy_IronTower();
                break;
            case TOWER_TYPE.DiaprismTower:
                attackStrategy = new AttackStrategy_DiaprismTower();
                break;
            case TOWER_TYPE.HealTower:
                attackStrategy = new AttackStrategy_HealTower();
                break;
            case TOWER_TYPE.MagnetTower:
                attackStrategy = new AttackStrategy_MagnetTower();
                break;
            case TOWER_TYPE.TopazTower:
                attackStrategy = new AttackStrategy_TopazTower();
                break;
            case TOWER_TYPE.RubyTower:
                attackStrategy = new AttackStrategy_RubyTower();
                break;
            case TOWER_TYPE.AquamarineTower:
                attackStrategy = new AttackStrategy_AquamarineTower();
                break;
        }
    }


    // ============<< IPoolable >>============
    public int GetId()
    {
        return towerId;
    }

    /// <summary>
    /// 풀링에서 오브젝트 생성 시 단 1번 실행
    /// </summary>
    public void OnInstantiate()
    {
        Init();
    }

    /// <summary>
    /// 풀링에서 가져올 때 호출
    /// </summary>
    public void OnGetFromPool()
    {
        gameObject.SetActive(true);
        ai.ResetStateMachine();
        ai.SetState(TOWER_STATE.Construction, true);
        ui.ResetHpBar();

        PoolManager.Instance.GetFromPool(10002, transform.position+Vector3.up);
        RenderUtil.SetSortingOrderByY(ui.icon);
    }

    /// <summary>
    /// 풀링으로 반환할 때 호출
    /// </summary>
    public void OnReturnToPool()
    {
        ui.OffAttackArea();
        ai.SetState(TOWER_STATE.None, true);
        ui.icon.color = Color.white;
        gameObject.SetActive(false);
    }
    // =======================================

    public void Interact()
    {
        // 파괴 옵션 on일 경우
        if (BuildManager.Instance.IsOnDestroy && !UIManager.Instance.DismantleUI.isActiveAndEnabled)
        {
            UIManager.Instance.DismantleUI.OpenDismantleUI(InteractManager.Instance.GetCurrentTarget());
            Debug.Log("dd : "+InteractManager.Instance.GetCurrentTarget());
            return;
        }

        if (!UIManager.Instance.UpgradeUI.isActiveAndEnabled)
        {
            ui.OnAttackArea();
            AudioManager.Instance.PlaySFX("UpgradeTower");
            UIManager.Instance.TowerUpgradeUI.OpenUpgradeUI(this);
        }
    }

    public bool IsInteractable(Vector3 playerPos, float range, BoxCollider2D playerCollider)
    {
        return true;
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    // 파괴 옵션 on일 경우
    //    if (BuildManager.Instance.isOnDestroy)
    //    {
    //        UIManager.Instance.DismantleUI.OpenDismantleUI(this);
    //        return;
    //    }

    //    // 우클릭 업그레이드 UI
    //    if (eventData.button == PointerEventData.InputButton.Right)
    //    {
    //        if (BuildManager.Instance.IsPlacing) return;
    //        UIManager.Instance.TowerUpgradeUI.OpenUpgradeUI(this);
    //    }

    //    // 좌클릭 전깃줄 연결 시도
    //    else if (eventData.button == PointerEventData.InputButton.Left)
    //    {
    //        if (LineDragConnector.Instance.IsDragging) return;

    //        if (towerType == TOWER_TYPE.Electricline && TryGetComponent(out ElectriclineTower wireTower))
    //        {
    //            if (wireTower.IsConnected)
    //                ToastManager.Instance.ShowToast("이미 연결된 타워입니다!");
    //            else
    //            {
    //                if (!LineDragConnector.Instance.IsDragging)
    //                    LineDragConnector.Instance.BeginDrag(wireTower);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 파괴 메서드
    /// </summary>
    public void DestroyTower()
    {
        var id = statHandler.ID;
        var towerData = DataManager.Instance.TowerData.GetById(id);
        var req = statHandler.AccumulatedCosts;

        float hpRatio = statHandler.CurrHp / statHandler.MaxHp;
        float refundRatio = 0f;

        refundRatio = GetRefundRatio(hpRatio);

        // 환급
        RefundResources(req, refundRatio);
        
        // 퀘스트 매니지먼트
        QuestManager.Instance.AddQuestAmount(QUEST_TYPE.DestroyBuilding);

        ui.OffAttackArea();

        // 실제 파괴 처리
        statHandler.OnDamaged(new Damaged
        {
            Value = 99999,
            Attacker = gameObject,
            Victim = gameObject
        });
    }
    /// <summary>
    /// 환급 비율 계산 메서드
    /// </summary>
    /// <param name="hpRatio">체력 비율 계산</param>
    /// <param name="upgradeOnly">업그레이드 타워인지 베이스 타워인지</param>
    /// <returns>환급 비율</returns>
    public float GetRefundRatio(float hpRatio)
    {
        if (hpRatio >= 0.8f)
            return 0.9f;
        else if (hpRatio >= 0.5f)
            return 0.7f;
        else
            return 0.35f;
    }

    /// <summary>
    /// 인벤토리에 해체된 아이템 넣어주기
    /// </summary>
    /// <param name="cost">buildrequirements정보</param>
    /// <param name="refundRatio">환급 비율</param>
    private void RefundResources(Dictionary<string, int> cost, float refundRatio)
    {
        foreach (var kvp in cost)
        {
            int refundAmount = Mathf.FloorToInt(kvp.Value * refundRatio);
            if (refundAmount > 0)
            {
                InventoryManager.Instance.Inventory.AddItem(
                    DataManager.Instance.ItemData.GetIdByName(kvp.Key),
                    refundAmount
                );
            }
        }
    }

    public int GetObejctSize()
    {
        switch (towerType)
        {
            case TOWER_TYPE.CooperTower:
            case TOWER_TYPE.IronTower:
            case TOWER_TYPE.DiaprismTower:
            case TOWER_TYPE.HealTower:
            case TOWER_TYPE.MagnetTower:
                return 1;
            case TOWER_TYPE.TopazTower:
            case TOWER_TYPE.RubyTower:
            case TOWER_TYPE.AquamarineTower:
                return 3;
            default: 
                return -1;
        }
    }
}
