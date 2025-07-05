using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileMovementSettings
{
    public Vector3 firePosition;

    public float moveSpeed; //데이터로 필요함
     
    //<< 곡사에서 사용 >>
    public float duration;  // 데이터로 필요함
    public float maxHeight; // 데이터로 필요함
}

public struct ProjectileAttackSettings
{
    public GameObject attacker;
    public GameObject target; 
    public float damage;
    public LayerMask enemyLayer;

    //<< 범위 공격에서 사용 >>
    public float splashRadius;  //데이터로 필요함  스플래쉬 데미지 반경

    //<< 연쇄 공격에서 사용 >>
    public int chainCount;     //데이터로 필요함
    public float chainingRadius; // 데이터로 필요함
    public GameObject previousTarget;
}

public class Projectile : MonoBehaviour
{
    [Header("[ 에디터 할당 - 데이터 생기면 교체 ]")]
    [SerializeField] private ParticleSystem _damageEffect;
    [SerializeField] private SpriteRenderer _icon;

    private IProjectileMovement _movement;
    private IProjectileAttack _attack;

    private GameObject _target;
    private float _damage;
    private float _lifeTime = 5f;
    private float _timer;

    private ProjectileAttackSettings _attackSettings;
    private ProjectileMovementSettings _movementSettings;

    public void Init(ProjectileAttackSettings attackSettings, ProjectileMovementSettings movementSettings, IProjectileMovement movement, IProjectileAttack attack)
    {
        _attackSettings = attackSettings;
        _movementSettings = movementSettings;
        _movement = movement;
        _attack = attack;

        _target = _attackSettings.target;
        _damage = _attackSettings.damage;
        transform.position = _movementSettings.firePosition;
        _movement.Init(_attackSettings, _movementSettings);
        _timer = 0f;
    }
    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _lifeTime || _movement.Movement())
        {
            _attack.Hit(_attackSettings);
            StartCoroutine(C_ReleaseAfterFx());
        }

        if (_movement.Movement())
        {
            // 체인 공격일 경우 Hit() 내부에서 다시 Init 될 수 있으므로
            // ReleaseAfterFx는 "체인 종료 시에만" 실행되어야 함
            _attack.Hit(_attackSettings);

            if (!(_attack is ProjectileAttack_Chaining))
                StartCoroutine(C_ReleaseAfterFx());
        }
    }
    private IEnumerator C_ReleaseAfterFx()
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
