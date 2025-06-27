using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _damageEffect;
    [SerializeField] private SpriteRenderer _icon;
    public GameObject Target { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }
    public float LifeTime { get; set; } = 5f;

    private bool _isSentDamaged;

    private float _timer;

    public void Init(GameObject target, float speed, float damage, Vector3 spawnPosition)
    {
        Target = target;
        Speed = speed;
        Damage = damage;
        transform.position = spawnPosition;
        _timer = 0f;
        _isSentDamaged = false;
        RenderUtil.SetSortingOrderByY(_icon);
    }
    private void Update()
    {
        if (Target == null)
        {
            Destroy(gameObject);
            //Release();
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= LifeTime)
        {
            Destroy(gameObject);
            //Release();
            return;
        }

        Vector3 dir = (Target.transform.position - transform.position).normalized;
        transform.position += dir * Speed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance < 0.3f)
        {
            if (_isSentDamaged) return;

            // Target의 데미지 매니징 함수 호출
            // Debug.Log($"[Projectile] Target 명중: {Damage} 피해");
            DamagedSystem.Instance.Send(new Damaged
            {
                Attacker = gameObject,
                Victim = Target,
                Value = Damage,
                IgnoreDefense = false
            });

            _isSentDamaged = true;

            if (_damageEffect != null)
                _damageEffect.Play();

            StartCoroutine(C_ReleaseAfterDelay());
        }
    }
    private IEnumerator C_ReleaseAfterDelay()
    {
        //Debug.Log("지연시간"+_damageEffect.main.duration);
        _icon.gameObject.SetActive(false);

        if (_damageEffect != null)
            yield return new WaitForSeconds(_damageEffect.main.duration);

        _icon.gameObject.SetActive(true);

        Destroy(gameObject);
        //Release();
    }
}
