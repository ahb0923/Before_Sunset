using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StraightProjectile : BaseProjectile
{
    public override void Init(GameObject target, float speed, float damage, Vector3 spawnPosition, int chainCount = 0, GameObject fromTarget = null)
    {
        base.Init(target, speed, damage, spawnPosition);
        attackType = PROJECTILE_TYPE.Defalut;
    }

    protected override void UpdateMovement()
    {
        if (Target == null) return;

        Vector3 dir = (Target.transform.position - transform.position).normalized;
        transform.position += dir * Speed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance < 0.3f)
            OnHit();
    }
}
