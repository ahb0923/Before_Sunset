using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _damageEffect;
    [SerializeField] private SpriteRenderer _icon;
    public GameObject Target { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }
    public float LifeTime { get; set; } = 5f;

    public bool isSentDamaged = false;

    private float _timer;

    public virtual void Init(GameObject target, float speed, float damage, Vector3 spawnPosition)
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

        DamagedSystem.Instance.Send(new Damaged
        {
            Attacker = gameObject,
            Victim = Target,
            Value = Damage,
            IgnoreDefense = false
        });

        isSentDamaged = true;
        StartCoroutine(C_ReleaseAfterDelay());
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
}
