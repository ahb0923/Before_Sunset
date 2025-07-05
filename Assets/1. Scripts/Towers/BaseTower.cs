using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum BUILDING_TYPE
{
    Normal,
    Slow
}
public enum TOWER_TYPE
{
    CooperTower,
    IronTower,
    DiaprismTower,
    HealTower,
    MagnetTower
}
public class BaseTower : MonoBehaviour
{
    [Header(" [에디터 할당] ")]
    [SerializeField] public TOWER_TYPE towerType;
    [SerializeField] public SpriteRenderer icon;
    [SerializeField] public List<Sprite> constructionIcon;
    [SerializeField] public Vector2Int size = new Vector2Int(1, 1); // 1x1 또는 2x2 등

    [SerializeField] private Collider2D _attackRangeCollider;
    [SerializeField] private Collider2D _buildCollider;
    [SerializeField] private Collider2D _mainCollider;

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


    private void Reset()
    {
        if (ai == null)
            ai = GetComponent<TowerAI>();

        if (statHandler == null)
            statHandler = new TowerStatHandler();
        statHandler.Init();

        if (attackSensor == null)
            attackSensor = GetComponentInChildren<TowerAttackSensor>();
        attackSensor.Init();

        if (ui == null)
            ui = GetComponentInChildren<TowerUI>();
        ui.Init();

        if (icon == null)
            icon = GetComponentInChildren<SpriteRenderer>();

        if (_mainCollider == null)
            _mainCollider = GetComponent<Collider2D>();
    }
    

    private void Awake()
    {
        if (ai == null)
            ai = GetComponent<TowerAI>();

        if (statHandler == null)
            statHandler = new TowerStatHandler();
        statHandler.Init();

        if (attackSensor == null)
            attackSensor = GetComponentInChildren<TowerAttackSensor>();
        attackSensor.Init();

        if (ui == null)
             ui = GetComponentInChildren<TowerUI>();
        ui.Init();

        if (icon == null)
            icon = GetComponentInChildren<SpriteRenderer>();

        if (_mainCollider == null)
            _mainCollider = GetComponent<Collider2D>();
    }
}
