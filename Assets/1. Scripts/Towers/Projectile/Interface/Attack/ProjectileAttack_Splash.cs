using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ProjectileAttack_Splash : IProjectileAttack
{
    public void Hit(ProjectileAttackSettings attackSettings)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackSettings.attacker.transform.position, attackSettings.splashRadius, attackSettings.enemyLayer);

        foreach (var hit in hits)
        {
            DamagedSystem.Instance.Send(new Damaged
            {
                Attacker = attackSettings.attacker,
                Victim = hit.gameObject,
                Value = attackSettings.damage,
                IgnoreDefense = false
            });
        }
    }
}
