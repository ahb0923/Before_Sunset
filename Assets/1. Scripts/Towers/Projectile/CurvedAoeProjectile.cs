using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedAoeProjectile : BaseProjectile
{
    [Header("곡선 궤적 설정")]
    public float duration = 0.6f;   // 전체 비행 시간
    public float maxHeight = 2f;     // 포물선 최고점
    [Header("범위 데미지 설정")]
    public LayerMask enemyLayer;

    // 월드 스케일로 변환시 transfrom.localScale.x를 곱해줄 필요 있음, x=y 라서 x값만 곱해줘도 계산 가능
    public float damageRadius => _collider.radius * transform.localScale.x;

    private Vector3 _start;
    private Vector3 _end; 
    private float _elapsed;
    private CircleCollider2D _collider;

    public override void Init(GameObject target, float speed, float damage, Vector3 spawnPos)
    {
        base.Init(target, speed, damage, spawnPos);

        _start = spawnPos;
        _end = target != null ? target.transform.position : spawnPos;
        _elapsed = 0f;

        _collider = GetComponent<CircleCollider2D>();
        _collider.isTrigger = true;
    }

    protected override void UpdateMovement()
    {
        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / duration);

        Vector3 pos = Vector3.Lerp(_start, _end, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * maxHeight;
        transform.position = pos;

        // 회전(선택)
        Vector3 dir = _end - _start;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        if (t >= 1f)
            OnHit();
    }

    protected override void OnHit()
    {
        if (isSentDamaged) return;

        // 범위 내 Monster Layer적 모두 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius, enemyLayer);

        foreach (var hit in hits)
        {
            DamagedSystem.Instance.Send(new Damaged
            {
                Attacker = gameObject,
                Victim = hit.gameObject,
                Value = Damage,
                IgnoreDefense = false
            });
        }

        isSentDamaged = true;
        StartCoroutine(C_ReleaseAfterDelay());
    }

}
