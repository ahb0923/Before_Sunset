using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack_Chaining : IProjectileAttack
{
    public void Hit(ProjectileAttackSettings attackSettings)
    {
        if (attackSettings.target == null)
        {
            TryReleaseProjectile(attackSettings.attacker);
            return;
        }

        DamagedSystem.Instance.Send(new Damaged
        {
            Attacker = attackSettings.attacker,
            Victim = attackSettings.target,
            Value = attackSettings.damage,
            IgnoreDefense = false
        });

        // 체인 종료 조건 1: 남은 체인 횟수 없음
        if (attackSettings.chainCount <= 0)
        {
            TryReleaseProjectile(attackSettings.attacker);
            return;
        }

        // 체인 종료 조건 2: 다음 대상 없음
        GameObject nextTarget = FindNextTarget(attackSettings);
        if (nextTarget == null)
        {
            TryReleaseProjectile(attackSettings.attacker);
            return;
        }

        // 체인 진행
        var proj = Helper_Component.GetComponent<Projectile>(attackSettings.attacker);

        ProjectileAttackSettings nextAtk = attackSettings;
        nextAtk.previousTarget = attackSettings.target;
        nextAtk.target = nextTarget;
        nextAtk.chainCount -= 1;
        nextAtk.damage = attackSettings.damage / 2;

        ProjectileMovementSettings nextMove = new ProjectileMovementSettings
        {
            firePosition = proj.transform.position,
            duration = 1f,
            maxHeight = 2f
        };

        proj.transform.localScale *= 0.8f;
        proj.Init(nextAtk, nextMove, new ProjectileMovement_Curved(), this);
    }

    private void TryReleaseProjectile(GameObject projectileGO)
    {
        var proj = Helper_Component.GetComponent<Projectile>(projectileGO);
        if (proj != null)
        {
            proj.StartCoroutine(proj.ReleaseAfterChainEnd());
        }
        else
        {
            GameObject.Destroy(projectileGO); // 예외 케이스 방지
        }
    }

    private GameObject FindNextTarget(ProjectileAttackSettings attackSettings)
    {
        Vector2 center = attackSettings.target.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackSettings.chainingRadius, attackSettings.enemyLayer);

        GameObject best = null;
        float highestHp = float.MinValue;

        foreach (var h in hits)
        {
            if (h == null) continue;

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
