using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PROJECTILE_TYPE
{
    Defalut,
    Splash,
    Chaining
}

public abstract class BaseProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _damageEffect;
    [SerializeField] private SpriteRenderer _icon;
    public GameObject Target { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }
    public float LifeTime { get; set; } = 5f;


    // << 곡선 타워만 사용하는 정보>>
    protected LayerMask enemyLayer;
    protected CircleCollider2D attackCollider;
    protected float damageRadius => attackCollider.radius * transform.localScale.x; // 월드 스케일로 변환시 transfrom.localScale.x를 곱해줄 필요 있음, x=y 라서 x값만 곱해줘도 계산 가능
    protected Vector3 start;
    protected Vector3 end;
    protected float elapsed;
    protected float duration = 0.5f;
    protected float maxHeight = 2f;

    // << 연쇄 타워만 사용하는 정보>>
    [SerializeField] protected int maxChains;
    [SerializeField] protected float chainRange;
    protected int currentChainCount = 0;
    protected GameObject previousTarget;


    public bool isSentDamaged = false;
    private float _timer;

    public PROJECTILE_TYPE attackType;

    public virtual void Init(GameObject target, float speed, float damage, Vector3 spawnPosition, int chainCount = 0, GameObject fromTarget = null)
    {
        Target = target;
        Speed = speed;
        Damage = damage;
        transform.position = spawnPosition;
        _timer = 0f;
        isSentDamaged = false;
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

        UpdateMovement();
    }

    protected abstract void UpdateMovement();

    protected virtual void OnHit()
    {
        if (isSentDamaged) return;

        switch (attackType)
        {
            case PROJECTILE_TYPE.Defalut:
                DefaultAttack();
                break;
            case PROJECTILE_TYPE.Splash:
                SplashAttack(); 
                break;
            case PROJECTILE_TYPE.Chaining:
                ChaingAttack();
                break;
        }

        isSentDamaged = true;
        StartCoroutine(C_ReleaseAfterDelay());
    }

    public void DefaultAttack()
    {
        DamagedSystem.Instance.Send(new Damaged
        {
            Attacker = gameObject,
            Victim = Target,
            Value = Damage,
            IgnoreDefense = false
        });
    }

    public void SplashAttack()
    {
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
    }

    public void ChaingAttack()
    {
        if (currentChainCount < maxChains)
        {
            GameObject nextTarget = FindNextTarget();
            if (nextTarget != null)
            {
                var newProjectile = Instantiate(gameObject).GetComponent<CurvedChainProjectile>();
                newProjectile.Init(nextTarget, Speed, Damage, transform.position, currentChainCount + 1, Target);
            }
        }
    }
    public IEnumerator C_ReleaseAfterDelay()
    {
        //Debug.Log("지연시간"+_damageEffect.main.duration);
        _icon.gameObject.SetActive(false);

        if (_damageEffect != null)
            yield return new WaitForSeconds(_damageEffect.main.duration);

        _icon.gameObject.SetActive(true);

        Destroy(gameObject);
        //Release();
    }

    public GameObject FindNextTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chainRange, enemyLayer);

        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.gameObject == Target || hit.gameObject == previousTarget)
                continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.gameObject;
            }
        }

        return closest;
    }
}
