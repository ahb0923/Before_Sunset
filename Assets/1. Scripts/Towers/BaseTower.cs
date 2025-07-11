using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    MagnetTower
}
public class BaseTower : MonoBehaviour, IPoolable 
{
    [Header(" [에디터 할당] ")]
    public TOWER_TYPE towerType;
    public int towerId;


    public Vector2Int size = new Vector2Int(1, 1); // 1x1 또는 2x2 등


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
    }
    

    public void Init()
    {
        statHandler.Init(this, towerId);
        ui.Init(this);
        ai.Init(this);
        attackSensor.Init(this);
        InitAttackStrategy();
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
    }

    /// <summary>
    /// 풀링으로 반환할 때 호출
    /// </summary>
    public void OnReturnToPool()
    {
        ai.SetState(TOWER_STATE.None, true);
        gameObject.SetActive(false);
    }
    // =======================================

}
