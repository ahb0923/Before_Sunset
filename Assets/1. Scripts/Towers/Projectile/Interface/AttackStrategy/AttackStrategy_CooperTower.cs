using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStrategy_CooperTower : IAttackStrategy
{
    public IEnumerator Attack(BaseTower tower)
    {
        var stat = tower.statHandler;
        var target = tower.attackSensor.CurrentTarget;

        if (tower.ai.CurState == TOWER_STATE.Destroy)
            yield break;

        tower.ui.animator.SetTrigger("IsAttack");

        if (target == null)
        {
            tower.ai.SetState(TOWER_STATE.Idle);
            yield break;
        }
        GameObject projObj = PoolManager.Instance.GetFromPool((int)stat.ProjectileID, tower.transform.position + Vector3.up * 2f);
        Projectile proj = Helper_Component.GetComponent<Projectile>(projObj);

        var attackSettings = new ProjectileAttackSettings
        {
            attacker = projObj,
            target = target,
            damage = stat.AttackPower
        };

        var movementSettings = new ProjectileMovementSettings
        {
            firePosition = tower.transform.position + Vector3.up * 2f,
            moveSpeed = DataManager.Instance.ProjectileData.GetById(proj.GetId()).moveSpeed
        };

        proj.Init(attackSettings, movementSettings, new ProjectileMovement_StraightTarget(), new ProjectileAttack_Single());

        yield return null;
    }
}
