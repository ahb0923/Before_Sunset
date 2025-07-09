using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum TOWER_STATE
{
    None,
    Construction,
    Idle,
    Attack,
    Destroy
}
public enum TOWER_ATTACK_TYPE
{
    Projectile,
    Areaofeffect,
    Healing
}


public class TowerAI : StateBasedAI<TOWER_STATE>
{
    private BaseTower _tower;
    private bool _isDestroy = false;
    private bool _isNowBuilding = true;


    protected override TOWER_STATE InvalidState => TOWER_STATE.None;


    public void Init(BaseTower baseTower)
    {
        _tower = baseTower;
    }
    
    protected override void OnAwake()
    {
        _tower = GetComponent<BaseTower>();
    }

    protected override IEnumerator OnStart()
    {
        CurState = TOWER_STATE.Construction;
        yield break;
    }
    protected override void DefineStates()
    {
        AddState(TOWER_STATE.Construction, new StateElem
        {
            Entered = () => Debug.Log("건설 시작"),
            Doing = C_Construction,
            Exited = () =>
            {
                Debug.Log("건설 완료");
            }
        });
        AddState(TOWER_STATE.Attack, new StateElem
        {
            Entered = () => Debug.Log("공격 시작"),
            Doing = C_Attack,
        });

        AddState(TOWER_STATE.Idle, new StateElem
        {
            Entered = () => Debug.Log("건물 대기 중"),
            Doing = C_Idle,
        });

        AddState(TOWER_STATE.Destroy, new StateElem
        {
            Entered = () => Debug.Log("건물 파괴"),
            Doing = C_Destroy
        });
    }
    protected override bool IsAIEnded()
    {
        return _isDestroy;
    }

    protected override bool IsTerminalState(TOWER_STATE state)
    {
        return state == TOWER_STATE.Destroy;
    }
    public void SetState(TOWER_STATE state, bool force = false)
    {
        TransitionTo(state, force);
    }
    public IEnumerator C_Construction()
    {
        //var sprites = _tower.constructionIcon;
        var ui = _tower.ui;
        var icon = ui.icon;
        var stat = _tower.statHandler;


        float totalTime = 1.5f;
        //float interval = totalTime / sprites.Count;
        float elapsed = 0f;

        //int spriteIndex = 0;

        // 방어코드 기본 0으로 초기화 하긴함
        stat.CurrHp = 0f;
        icon.color = ColorExtensions.WithAlpha(icon.color, 0.5f);

        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / totalTime);
            stat.CurrHp = Mathf.Lerp(0, stat.MaxHp, t);

            ui.UpdateHpBar(stat.CurrHp, stat.MaxHp);
            /*
            // 일정 시간마다 스프라이트 전환
            if (spriteIndex < sprites.Count && elapsed >= interval * spriteIndex)
            {
                icon.sprite = sprites[spriteIndex];
                //AudioManager.Instance().PlaySfx(SFX_TYPE.Building_Place);
                spriteIndex++;
            }*/

            yield return null;
        }

        // 건설 완료 후 체력 100% 보장
        stat.CurrHp = stat.MaxHp;
        ui.hpBar_immediate.fillAmount = 1f;
        ui.hpBar_delay.fillAmount = 1f;
        icon.color = ColorExtensions.WithAlpha(icon.color, 1f);

        // 건설 직후, 공격 범위 내 적 검색
        _tower.attackSensor.ScanInitialEnemies();

        if (_tower.attackSensor.HasDetectedEnemy())
            CurState = TOWER_STATE.Attack;
        else
            CurState = TOWER_STATE.Idle;
    }


    protected virtual IEnumerator C_Idle()
    {
        if (_isNowBuilding)
        {
            _isNowBuilding = false;
            _tower.attackSensor.ScanInitialEnemies();

            if (_tower.attackSensor.HasDetectedEnemy())
                CurState = TOWER_STATE.Attack;
            else
                CurState = TOWER_STATE.Idle;
        }
        yield return null;
    }
    protected virtual IEnumerator C_Attack()
    {
        while (true)
        {
            if (IsInterrupted)
            {
                Debug.Log("[C_Attack] 상태 중단 감지");
                yield break;
            }

            var stat = _tower.statHandler;

            switch (_tower.statHandler.attackType)
            {
                // 투사체 발사 타워
                case TOWER_ATTACK_TYPE.Projectile:
                    _tower.attackSensor.CheckTargetValid();

                    GameObject target = _tower.attackSensor.CurrentTarget;
                    if (target == null)
                    {
                        Debug.Log("타겟 없음, Idle로 전환");
                        CurState = TOWER_STATE.Idle;
                        yield break;
                    }

                    // 화살 발사
                    //GameObject projObj = ObjectPoolM.anager.Instance().Get(building.projectile.gameObject.name);
                    GameObject projObj = Instantiate(_tower.projectile);
                    Projectile proj = Helper_Component.GetComponent<Projectile>(projObj);
                    // 발사체 속도 하드코딩 => 추후 proj 데이터를 따로 만들든, tower데이터의 추가하든..

                    // 추후에 필드 멤버로 만들어서 init에서 세팅해버리기


                    // 이쪽 추후에 리팩터링
                    if (_tower.towerType == TOWER_TYPE.CooperTower)
                    {
                        ProjectileAttackSettings projAttackSettings = new()
                        {
                            attacker = projObj,
                            target = target,
                            damage = stat.AttackPower,
                        };
                        ProjectileMovementSettings projMovementSettings = new()
                        {
                            firePosition = _tower.transform.position + new Vector3(0, 2, 0),
                            moveSpeed = 10f,
                        };
                        proj.Init(projAttackSettings, projMovementSettings, new ProjectileMovement_StraightTarget(), new ProjectileAttack_Single());
                    }
                    else if (_tower.towerType == TOWER_TYPE.IronTower)
                    {
                        ProjectileAttackSettings projAttackSettings = new()
                        {
                            attacker = projObj,
                            target = target,
                            damage = stat.AttackPower,
                            splashRadius = 1.5f,
                            enemyLayer = LayerMask.GetMask("Monster")
                        };
                        ProjectileMovementSettings projMovementSettings = new()
                        {
                            firePosition = _tower.transform.position + new Vector3(0, 2, 0),
                            duration = 1f,
                        };
                        proj.Init(projAttackSettings, projMovementSettings, new ProjectileMovement_Curved(), new ProjectileAttack_Splash());
                    }
                    else if (_tower.towerType == TOWER_TYPE.DiaprismTower)
                    {
                        ProjectileAttackSettings projAttackSettings = new()
                        {
                            attacker = projObj,
                            target = target,
                            damage = stat.AttackPower,
                            enemyLayer = LayerMask.GetMask("Monster"),
                            chainCount = 2,
                            chainingRadius = 2f,
                            previousTarget = null,
                        };
                        ProjectileMovementSettings projMovementSettings = new()
                        {
                            firePosition = _tower.transform.position + new Vector3(0, 2, 0),
                            duration = 1f,
                        };
                        proj.Init(projAttackSettings, projMovementSettings, new ProjectileMovement_CurvedTarget(), new ProjectileAttack_Chaining());
                    }
                    
                    break;
                    

                // 자신 중심 공격 타워
                case TOWER_ATTACK_TYPE.Areaofeffect:

                    if (_tower.towerType == TOWER_TYPE.HealTower)
                    {
                        float radius = _tower.statHandler.AttackRange;
                        int healAmount = Mathf.RoundToInt(_tower.statHandler.AttackPower);

                        Collider2D[] hits = Physics2D.OverlapCircleAll(
                            _tower.transform.position, radius, LayerMask.GetMask("Tower"));

                        Debug.Log($"검색 갯수 : {hits.Length}");

                        // 체력 비율 낮은 순으로 정렬
                        List<GameObject> healables = new();

                        foreach (var hit in hits)
                        {
                            if (hit.gameObject == _tower.gameObject) continue; // 자기 자신 제외

                            var hitStat = hit.GetComponent<TowerStatHandler>();
                            if (hitStat != null && hitStat.CurrHp < hitStat.MaxHp)
                            {
                                healables.Add(hit.gameObject);
                            }
                        }

                        healables.Sort((a, b) =>
                        {
                            var sa = a.GetComponent<TowerStatHandler>();
                            var sb = b.GetComponent<TowerStatHandler>();
                            float ra = sa == null ? float.MaxValue : sa.CurrHp;
                            float rb = sb == null ? float.MaxValue : sb.CurrHp;
                            return ra.CompareTo(rb);
                        });

                        int healCount = Mathf.Min(3, healables.Count);
                        for (int i = 0; i < healCount; i++)
                        {
                            var healbleStat = healables[i].GetComponent<TowerStatHandler>();
                            healbleStat.CurrHp += healAmount;

                            Debug.Log($"힐타워: {healables[i].name} 체력 {healAmount} 회복");
                        }
                    }
                    else if (_tower.towerType == TOWER_TYPE.MagnetTower)
                    {
                        float radius = _tower.statHandler.AttackRange;
                        //float pullSpeed = _tower.statHandler.AttackPower;
                        float pullSpeed = 6;
                        Vector3 centerPos = _tower.transform.position;


                        Collider2D[] hits = Physics2D.OverlapCircleAll(centerPos, radius, LayerMask.GetMask("Monster"));

                        List<GameObject> candidates = new();
                        foreach (var h in hits)
                        {
                            if (h == null || !h.gameObject.activeSelf) continue;
                            candidates.Add(h.gameObject);
                        }

                        candidates.Sort((a, b) => Vector2.Distance(centerPos, b.transform.position).CompareTo(Vector2.Distance(centerPos, a.transform.position)));

                        int pullCount = Mathf.Min(3, candidates.Count);
                        for (int i = 0; i < pullCount; i++)
                        {
                            GameObject pullTarget = candidates[i];
                            _tower.StartCoroutine(PullTargetCoroutine(pullTarget, centerPos, pullSpeed));
                        }
                    }
                    break;
            }
            yield return new WaitForSeconds(stat.AttackSpeed);
        }
    }

    private IEnumerator PullTargetCoroutine(GameObject target, Vector3 center, float speed)
    {
        Debug.Log(target);
        //var ai = target.GetComponent<MonsterAI>();
        //if (ai != null) ai.enabled = false;

        while (target != null && target.activeSelf)
        {
            Vector3 dir = (center - target.transform.position).normalized;
            target.transform.position += dir * speed * Time.deltaTime;

            if (Vector3.Distance(center, target.transform.position) < 2f)
                break;

            yield return null;
        }
        //if (ai != null) ai.enabled = true;
    }
    private IEnumerator C_Destroy()
    {
        Debug.Log("C_Destroy() 실행됨");

        yield return null;

        _isDestroy = true;
        DefenseManager.Instance.RemoveObstacle(transform);
        PoolManager.Instance.ReturnToPool(_tower.statHandler.ID, gameObject);

        yield return null;
    }
    public void ResetStateMachine()
    {
        StopAllCoroutines();
        _isDestroy = false;
        _isNowBuilding = true;
        IsInterrupted = false;
        CurState = InvalidState;
        RunDoingState();
    }
}
