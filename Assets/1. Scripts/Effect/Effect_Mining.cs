using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Effect_Mining : MonoBehaviour,IPoolable
{
    private const int ID = 10003;
    [SerializeField] private ParticleSystem _particle;

    private Coroutine _coroutine;
    private bool _returning;

    private void Awake()
    {
        // 파티클 시스템 초기화
        if (_particle == null)
        {
            _particle = Helper_Component.GetComponentInChildren<ParticleSystem>(gameObject);
        }
    }


    public void Play()
    {
        // 파티클 시스템을 초기화하고 재생
        _particle.Clear(true);
        _particle.Play(true);
    }

    public int GetId()
    {
        return ID;
    }

    public void OnGetFromPool()
    {
        _returning = false;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        if (_particle != null)
        {
            var main = _particle.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.None;

            _particle.Clear(true);  
            _particle.Play(true); 
        }

        _coroutine= StartCoroutine(C_PlayAndReturn(0.5f));
    }

    public void OnInstantiate()
    {
    }

    public void OnReturnToPool()
    {
        if (_particle != null)
        {
            _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    private IEnumerator C_PlayAndReturn(float lifetime)
    {
        // 타임스케일 무시: 실시간으로 0.5초
        yield return new WaitForSecondsRealtime(lifetime);
        SafeReturn();
        _coroutine = null;
    }

    private void SafeReturn()
    {
        if (_returning) return; // 중복 방지
        _returning = true;
        PoolManager.Instance.ReturnToPool(ID, gameObject);
    }
}
