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

public class Projectile : MonoBehaviour, IPoolable
{
    [Header("[ 에디터 할당 - 데이터 생기면 교체 ]")]
    [SerializeField] private int _ID;
    [SerializeField] private ParticleSystem _damageEffect;
    //[SerializeField] private SpriteRenderer _icon;
    [SerializeField] private GameObject _slashTestImage;
    [SerializeField] private Animator _animator;

    private ProjectileDatabase _data;

    private IProjectileMovement _movement;
    private IProjectileAttack _attack;

    private GameObject _target;
    private float _damage;
    private float _lifeTime = 10f;
    private float _timer;
    private bool _isInit = false;
    
    private bool _hasHit = false;

    private ProjectileAttackSettings _attackSettings;
    private ProjectileMovementSettings _movementSettings;

    public void Init(ProjectileAttackSettings attackSettings, ProjectileMovementSettings movementSettings, IProjectileMovement movement, IProjectileAttack attack)
    {
        _data = DataManager.Instance.ProjectileData.GetById(_ID);

        _attackSettings = attackSettings;
        _movementSettings = movementSettings;
        _movement = movement;
        _attack = attack;
   
        transform.position = _movementSettings.firePosition;
        _target = _attackSettings.target;
        _damage = _attackSettings.damage;
        _movement.Init(_attackSettings, _movementSettings);
        _timer = 0f;

        //테스트용
        _hasHit = false;
        if(_slashTestImage!=null)
            _slashTestImage.SetActive(false);
        _isInit = true;
    }
    private void Update()
    {
        if (_isInit && this.isActiveAndEnabled)
        {
            _timer += Time.deltaTime;
            if (_hasHit) return;

            bool hasArrived = _movement.Movement(); // 1회만 호출하여 캐싱

            if (_timer > _lifeTime)
            {
                _hasHit = true;
                _attack.Hit(_attackSettings);
                StartCoroutine(C_ReleaseAfterFx());
                return;
            }

            if (hasArrived)
            {
                _hasHit = true;
                _attack.Hit(_attackSettings);

                // 체인 공격은 재사용될 수 있으므로 Release는 마지막에만
                if (!(_attack is ProjectileAttack_Chaining))
                {
                    StartCoroutine(C_ReleaseAfterFx());
                }
            }
        }
    }
    
    private IEnumerator C_ReleaseAfterFx()
    {
        // 테스트용 이미지코드 
        if (_slashTestImage != null)
        {
            _slashTestImage.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        // 회전 초기화
        transform.rotation = Quaternion.identity;

        //Debug.Log("지연시간"+_damageEffect.main.duration);
        //_icon.gameObject.SetActive(false);
        _animator?.SetBool("IsMove", false);
        yield return new WaitForSeconds(0.4f);
        
        if (_damageEffect != null)
            yield return new WaitForSeconds(_damageEffect.main.duration);

        //_icon.gameObject.SetActive(true);

        //Destroy(gameObject);
        transform.localScale = Vector3.one;
        PoolManager.Instance.ReturnToPool(_ID, gameObject);

    }

    public IEnumerator ReleaseAfterChainEnd()
    {
        transform.localScale = Vector3.one * 2;
        _animator?.SetBool("IsMove", false);
        yield return C_ReleaseAfterFx();
    }
    public int GetId()
    {
        return _ID;
    }

    public void OnInstantiate()
    {
    }

    public void OnGetFromPool()
    {
        _animator?.SetBool("IsMove", true);
    }

    public void OnReturnToPool()
    {
        _isInit = false;
    }
}
