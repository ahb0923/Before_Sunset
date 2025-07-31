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
    Trap
}


public class TowerAI : StateBasedAI<TOWER_STATE>
{
    private BaseTower _tower;
    private bool _isDestroy = false;
    private bool _isFirstAttack = false;
    private Animator _animator;
    protected override TOWER_STATE InvalidState => TOWER_STATE.None;


    public void Init(BaseTower baseTower)
    {
        _tower = baseTower;
        _animator = _tower.ui.animator;
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
            Entered = () => _tower._attackCollider.enabled = false,
            Doing = C_Construction,
            Exited = () => _tower._attackCollider.enabled = true
        });
        AddState(TOWER_STATE.Attack, new StateElem
        {
            //Entered = () => Debug.Log("공격 시작"),
            Doing = C_Attack,
        });

        AddState(TOWER_STATE.Idle, new StateElem
        {
            //Entered = () => Debug.Log("건물 대기 중"),
            Doing = C_Idle,
        });

        AddState(TOWER_STATE.Destroy, new StateElem
        {
            //Entered = () => Debug.Log("건물 파괴"),
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
        if (state == TOWER_STATE.Attack)
            _isFirstAttack = true;
        TransitionTo(state, force);
        /*
        if (state == TOWER_STATE.Construction)
            _tower._attackCollider.enabled = false;
        else if (state == TOWER_STATE.Idle || state == TOWER_STATE.Attack)
            _tower._attackCollider.enabled = true;*/
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
        //icon.color = ColorExtensions.WithAlpha(icon.color, 0.5f);
        

        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / totalTime);
            stat.CurrHp = Mathf.Lerp(0, stat.MaxHp, t);

            //_tower.ui.SetConstructionProgress(t);
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
        //_tower.ui.SetConstructionProgress(1f);
        stat.CurrHp = stat.MaxHp;

        /*
        // 건설 직후, 공격 범위 내 적 검색
        _tower.attackSensor.ScanInitialEnemies();

        if (_tower.attackSensor.HasDetectedEnemy())
            CurState = TOWER_STATE.Attack;
        else
            CurState = TOWER_STATE.Idle;*/
        CurState = TOWER_STATE.Idle;
    }


    protected virtual IEnumerator C_Idle()
    {
        _tower.attackSensor.ScanInitialEnemies();

        if (_tower.attackSensor.HasDetectedEnemy())
        {
            CurState = TOWER_STATE.Attack;
            yield break;
        }

        yield return null;
    }
    protected virtual IEnumerator C_Attack()
    {
        if (CurState == TOWER_STATE.Destroy)
            yield break;

        if (_isFirstAttack)
        {
            //Debug.Log("[C_Attack] 공격 진입 직후 1회 스킵");
            _isFirstAttack = false;
            //yield return Helper_Coroutine.WaitSeconds(_tower.statHandler.AttackSpeed);
            yield return Helper_Coroutine.WaitSeconds(1.0f);
        }

        if (CurState == TOWER_STATE.Destroy)
            yield break;

        if (_tower.attackSensor.CurrentTarget == null || !_tower.attackSensor.CurrentTarget.activeSelf)
        {
            _tower.attackSensor.CheckTargetValid();
            yield break; // 상태 Idle로 전환될 것
        }
 
        AudioManager.Instance.PlaySFX(_tower.statHandler.TowerName);

        while (true)
        {
            if (IsInterrupted || CurState == TOWER_STATE.Destroy)
            {
               // Debug.Log("[C_Attack] 상태 중단 감지");
                yield break;
            }

            switch (_tower.statHandler.AttackType)
            {
                case TOWER_ATTACK_TYPE.Projectile:
                    _tower.attackSensor.CheckTargetValid();
                    GameObject target = _tower.attackSensor.CurrentTarget;
                    if (target == null)
                    {
                        //Debug.Log("타겟 없음, Idle로 전환");
                        CurState = TOWER_STATE.Idle;
                        yield break;
                    }
                    break;
                case TOWER_ATTACK_TYPE.Areaofeffect:
                    break;
            }

            yield return _tower.attackStrategy.Attack(_tower);

            if (CurState == TOWER_STATE.Destroy)
                yield break;

            yield return Helper_Coroutine.WaitWithInterrupt(_tower.statHandler.AttackSpeed, () => CurState == TOWER_STATE.Destroy || IsInterrupted);
        }
    }
    private IEnumerator C_Destroy()
    {
        Debug.Log("C_Destroy() 실행됨");
        if (_animator != null)
        {
            _animator.SetTrigger("IsDestroy");
        }
        AudioManager.Instance.PlaySFX(_tower.statHandler.name);
        yield return Helper_Coroutine.WaitSeconds(0.4f);

        _isDestroy = true;
        DefenseManager.Instance.RemoveObstacle(transform);
        if (_tower.towerType == TOWER_TYPE.Electricline && TryGetComponent(out ElectriclineTower wireTower))
        {
            wireTower.Disconnect();
        }
        PoolManager.Instance.ReturnToPool(_tower.statHandler.ID, gameObject);
       

        yield return null;
    }
    public void ResetStateMachine()
    {
        StopAllCoroutines();
        _isDestroy = false;
        IsInterrupted = false;
        CurState = InvalidState;
        RunDoingState();
    }
}
