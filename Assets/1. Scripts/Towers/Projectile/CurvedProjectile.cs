using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CurvedProjectile : BaseProjectile
{
    public override void Init(GameObject target, float speed, float damage, Vector3 spawnPosition, int chainCount = 0, GameObject fromTarget = null)
    {
        base.Init(target, speed, damage, spawnPosition);
        start = spawnPosition;
        end = target.transform.position;
        elapsed = 0f;

        duration = 0.5f;
        maxHeight = 2f;

        attackType = PROJECTILE_ATTACK_TYPE.Defalut;
    }

    protected override void UpdateMovement()
    {
        elapsed += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(elapsed / duration);

        Vector3 projectilePosition = Vector3.Lerp(start, end, normalizedTime);
        projectilePosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * maxHeight;
        transform.position = projectilePosition;

        Vector3 direction = end - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        if (normalizedTime >= 1f)
            OnHit();
    }
}
