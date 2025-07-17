using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStrategy_DiaprismTower : IAttackStrategy
{
    public IEnumerator Attack(BaseTower tower)
    {
        var stat = tower.statHandler;
        var target = tower.attackSensor.CurrentTarget;

        if (target == null)
        {
            tower.ai.SetState(TOWER_STATE.Idle);
            yield break;
        }
        GameObject projObj = PoolManager.Instance.GetFromPool(stat.ProjectileID, tower.transform.position + Vector3.up * 2f, tower.transform);
        Projectile proj = Helper_Component.GetComponent<Projectile>(projObj);

        var attackSettings = new ProjectileAttackSettings
        {
            attacker = projObj,
            target = target,
            damage = stat.AttackPower,
            enemyLayer = LayerMask.GetMask("Monster"),
            chainCount = 2,
            chainingRadius = 2f,
            previousTarget = null,
        };

        var movementSettings = new ProjectileMovementSettings
        {
            firePosition = tower.transform.position + Vector3.up * 2f,
            duration = DataManager.Instance.ProjectileData.GetById(proj.GetId()).moveSpeed
        };

        proj.Init(attackSettings, movementSettings, new ProjectileMovement_CurvedTarget(), new ProjectileAttack_Chaining());

        yield return new WaitForSeconds(stat.AttackSpeed);
    }
}
