using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack_Chaining : IProjectileAttack
{
    public void Hit(ProjectileAttackSettings attackSettings)
    {
        if (attackSettings.target == null)
            return;

        DamagedSystem.Instance.Send(new Damaged
        {
            Attacker = attackSettings.attacker,
            Victim = attackSettings.target,
            Value = attackSettings.damage,
            IgnoreDefense = false
        });

        // 쿠션 횟수 없으면 종료
        if (attackSettings.chainCount <= 0) return;

        GameObject nextTarget = FindNextTarget(attackSettings);
        if (nextTarget == null)
        {
            Debug.Log("다음 타겟 물색 실패");
            return;
        }


        var proj = Helper_Component.GetComponent<Projectile>(attackSettings.attacker);

        ProjectileAttackSettings nextAtk = attackSettings;
        nextAtk.previousTarget = attackSettings.target;
        nextAtk.target = nextTarget;
        nextAtk.chainCount -= 1;
        nextAtk.damage = attackSettings.damage / 2;

        ProjectileMovementSettings nextMove = new ProjectileMovementSettings
        {
            firePosition = proj.transform.position,
            duration = 3f,
            maxHeight = 2f
        };

        proj.transform.localScale *= 0.8f;
        proj.Init(nextAtk, nextMove, new ProjectileMovement_Curved(), this);
    }

    private GameObject FindNextTarget(ProjectileAttackSettings attackSettings)
    {
        Vector2 center = attackSettings.target.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackSettings.chainingRaduis, attackSettings.enemyLayer);

        GameObject best = null;
        float highestHp = float.MinValue;

        foreach (var h in hits)
        {
            if (h == null)
                continue;

            GameObject cand = h.gameObject;
            if (cand == attackSettings.target || cand == attackSettings.previousTarget)
                continue;

            var enemy = cand.GetComponent<MonsterStatHandler>();
            if (enemy == null) continue;

            float hp = enemy.CurHp;
            if (hp > highestHp)
            {
                highestHp = hp;
                best = cand;
            }
        }

        return best;
    }

}
