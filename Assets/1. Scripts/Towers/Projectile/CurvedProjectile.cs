using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedProjectile : BaseProjectile
{
    public float Duration = 0.5f;
    public float MaxHeight = 2f;

    private Vector3 _start;
    private Vector3 _end;
    private float _elapsed;

    public override void Init(GameObject target, float speed, float damage, Vector3 spawnPosition)
    {
        base.Init(target, speed, damage, spawnPosition);
        _start = spawnPosition;
        _end = target.transform.position;
        _elapsed = 0f;
    }

    protected override void UpdateMovement()
    {
        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / Duration);

        Vector3 pos = Vector3.Lerp(_start, _end, t);
        float height = Mathf.Sin(t * Mathf.PI) * MaxHeight;
        pos.y += height;

        transform.position = pos;

        Vector3 dir = _end - _start;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        if (t >= 1f)
            OnHit();
    }
}
