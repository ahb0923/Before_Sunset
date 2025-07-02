using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Utilities;
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
    private bool _isDestroy = false;
    private bool _isNowBuilding = true;
    public BaseTower tower;

    protected override TOWER_STATE InvalidState => TOWER_STATE.None;

    protected override void OnAwake()
    {
        tower = GetComponent<BaseTower>();
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
            Exited = () => {
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
        var sprites = tower.constructionIcon;
        var icon = tower.icon;
        var stat = tower.statHandler;
        var ui = tower.ui;

        float totalTime = 1.5f;
        float interval = totalTime / sprites.Count;
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
        tower.attackSensor.ScanInitialEnemies();

        if (tower.attackSensor.HasDetectedEnemy())
            CurState = TOWER_STATE.Attack;
        else
            CurState = TOWER_STATE.Idle;
    }


    protected virtual IEnumerator C_Idle()
    {
        if (_isNowBuilding)
        {
            _isNowBuilding = false;
            tower.attackSensor.ScanInitialEnemies();

            if (tower.attackSensor.HasDetectedEnemy())
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

            var stat = tower.statHandler;
            // 발사 위치 변경시 이곳
            Vector3 firePosition = tower.transform.position;

            switch (tower.statHandler.attackType)
            {
                // 투사체 발사 타워
                case TOWER_ATTACK_TYPE.Projectile:
                    tower.attackSensor.CheckTargetValid();

                    GameObject target = tower.attackSensor.CurrentTarget;
                    if (target == null)
                    {
                        Debug.Log("타겟 없음, Idle로 전환");
                        CurState = TOWER_STATE.Idle;
                        yield break;
                    }

                    // 화살 발사
                    //GameObject projObj = ObjectPoolM.anager.Instance().Get(building.projectile.gameObject.name);
                    GameObject projObj = Instantiate(tower.projectile);
                    Projectile proj = projObj.GetComponent<Projectile>();
                    // 발사체 속도 하드코딩 => 추후 proj 데이터를 따로 만들든, tower데이터의 추가하든..
                    proj.Init(target, 10, stat.AttackPower, firePosition);
                    break;

                // 자신 중심 공격 타워
                case TOWER_ATTACK_TYPE.Areaofeffect:

                    break;

            }

            yield return new WaitForSeconds(stat.AttackSpeed);
        }
    }
    private IEnumerator C_Destroy()
    {
        Debug.Log("C_Destroy() 실행됨");

        yield return null;

        _isDestroy = true;
        Destroy(tower.gameObject);

        yield return null;
    }
}
