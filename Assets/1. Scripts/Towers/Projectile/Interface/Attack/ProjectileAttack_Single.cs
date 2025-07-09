using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack_Single : IProjectileAttack
{
    public void Hit(ProjectileAttackSettings attackSettings)
    {
        if (attackSettings.target == null) return;

        DamagedSystem.Instance.Send(new Damaged
        {
            Attacker = attackSettings.attacker,
            Victim = attackSettings.target,
            Value = attackSettings.damage,
            IgnoreDefense = false
        });
    }
}

