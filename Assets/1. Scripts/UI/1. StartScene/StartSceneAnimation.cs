using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartSceneAnimation : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private GameObject _mainCamera;
    
    [Header("캐릭터")]
    [SerializeField] private GameObject _movingCharacter;
    [SerializeField] private GameObject _idleCharacter;
    [SerializeField] private GameObject _miningCharacter;
    [SerializeField] private GameObject _runningCharacter;
    
    [Header("이펙트")]
    [SerializeField] private GameObject _dust1;
    [SerializeField] private GameObject _dust2;
    [SerializeField] private GameObject _dustParticle;
    [SerializeField] private ParticleSystem _dustParticleSystem;
    [SerializeField] private GameObject _explosionContainer1;
    [SerializeField] private GameObject _explosionContainer2;
    [SerializeField] private SpriteRenderer _explosion1;
    [SerializeField] private SpriteRenderer _explosion2;
    [SerializeField] private SpriteRenderer _explosion3;
    [SerializeField] private SpriteRenderer _explosion4;
    [SerializeField] private SpriteRenderer _explosion5;
    [SerializeField] private GameObject _circleExplosion;
    [SerializeField] private GameObject _circleFadeIn;
    
    private Vector2 _bottomLeft = new Vector2(31.5f, -4.5f);
    private Vector2 _topRight = new Vector2(84.5f, 17.5f);
    
    private Coroutine _cameraCoroutine;

    private void Reset()
    {
        _mainCamera = GameObject.Find("Main Camera");
        _movingCharacter = Helper_Component.FindChildGameObjectByName(this.gameObject, "MovingCharacter");
        _idleCharacter = Helper_Component.FindChildGameObjectByName(this.gameObject, "IdleCharacter");
        _miningCharacter = Helper_Component.FindChildGameObjectByName(this.gameObject, "MiningCharacter");
        _runningCharacter = Helper_Component.FindChildGameObjectByName(this.gameObject, "RunningCharacter");
        _dust1 = Helper_Component.FindChildGameObjectByName(this.gameObject, "Dust1");
        _dust2 = Helper_Component.FindChildGameObjectByName(this.gameObject, "Dust2");
        _dustParticle = Helper_Component.FindChildGameObjectByName(this.gameObject, "DustParticle");
        _explosionContainer1 = Helper_Component.FindChildGameObjectByName(this.gameObject, "ExplosionContainer1");
        _explosionContainer2 = Helper_Component.FindChildGameObjectByName(this.gameObject, "ExplosionContainer2");
        _circleExplosion = Helper_Component.FindChildGameObjectByName(this.gameObject, "CircleExplosion");
        _circleFadeIn = Helper_Component.FindChildGameObjectByName(this.gameObject, "CircleFadeIn");
        
        _dustParticleSystem = Helper_Component.FindChildComponent<ParticleSystem>(this.transform, "DustParticle");
        _explosion1 = Helper_Component.FindChildComponent<SpriteRenderer>(this.transform, "Explosion1");
        _explosion2 = Helper_Component.FindChildComponent<SpriteRenderer>(this.transform, "Explosion2");
        _explosion3 = Helper_Component.FindChildComponent<SpriteRenderer>(this.transform, "Explosion3");
        _explosion4 = Helper_Component.FindChildComponent<SpriteRenderer>(this.transform, "Explosion4");
        _explosion5 = Helper_Component.FindChildComponent<SpriteRenderer>(this.transform, "Explosion5");
    }

    private void Start()
    {
        AudioManager.Instance.SetSFXVolume(1f);
        AudioManager.Instance.SetBGMVolume(1f);
        
        if (!GlobalState.HasPlayedIntro)
        {
            StartMove();
        }
        else
        {
            CameraAction();
            AudioManager.Instance.PlayBGM("Main");
        }
    }

    [Button]
    private void StartMove()
    {
        _movingCharacter.SetActive(true);
        Vector2 target = new Vector2(-2f, -1.2f);
        
        Sequence seq = DOTween.Sequence();
        seq.Append(_movingCharacter.transform.DOMove(target, 5f).SetEase(Ease.Linear).OnComplete(StartIdle));
        for (int i = 0; i < 8; i++)
        {
            seq.InsertCallback(0.6f * (i + 1), WalkSound);
        }
    }

    private void WalkSound()
    {
        AudioManager.Instance.PlaySFX("UIClick");
    }

    private void StartIdle()
    {
        _movingCharacter.SetActive(false);
        _idleCharacter.SetActive(true);
        StartCoroutine(C_Idle());
    }

    private IEnumerator C_Idle()
    {
        yield return new WaitForSeconds(3f);
        _idleCharacter.SetActive(false);
        StartMining();
    }

    [Button]
    private void StartMining()
    {
        _miningCharacter.SetActive(true);
        _dust1.SetActive(true);
        StartCoroutine(C_Mining());
    }

    private IEnumerator C_Mining()
    {
        _dustParticle.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _dustParticleSystem.Play();
        ShakeCamera(0.2f, 0.1f, 0.2f);
        yield return new WaitForSeconds(1f);
        _dustParticleSystem.Play();
        ShakeCamera(0.2f, 0.1f, 0.2f);
        ShakeSound();
        yield return new WaitForSeconds(1f);
        _dustParticleSystem.Play();
        ShakeCamera(0.2f, 0.1f, 0.2f);
        yield return new WaitForSeconds(1f);
        _dustParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        yield return new WaitWhile(() => _dustParticleSystem.IsAlive());
        var main = _dustParticleSystem.main;
        main.duration = 9f;
        _dustParticleSystem.Play();
        ShakeCamera(9f, 0.1f);
        yield return new WaitForSeconds(2f);
        _miningCharacter.SetActive(false);
        _dust1.SetActive(false);
        StartIdle2();
    }

    private void ShakeSound()
    {
        AudioManager.Instance.PlaySFX("EarthRumble");
    }

    private void StartIdle2()
    {
        _idleCharacter.SetActive(true);
        _dust2.SetActive(true);
        StartCoroutine(C_Idle2());
    }

    private IEnumerator C_Idle2()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(C_StartExplode());
        yield return new WaitForSeconds(1.5f);
        _idleCharacter.SetActive(false);
        StartRunning();
        yield return new WaitForSeconds(4.5f);
        _dust2.SetActive(false);
    }

    private void StartRunning()
    {
        _runningCharacter.SetActive(true);
        Vector2 target = new Vector2(-10f, -1.2f);
        _runningCharacter.transform.DOMove(target, 2f).SetEase(Ease.Linear);
    }

    private IEnumerator C_StartExplode()
    {
        _explosionContainer1.SetActive(true);
        StartCoroutine(C_Explode(_explosion1, _explosion2));
        yield return new WaitForSeconds(1.5f);
        _explosionContainer2.SetActive(true);
        StartCoroutine(C_Explode(_explosion3, _explosion4, _explosion5));
        AudioManager.Instance.PlaySFX("Hit");
        yield return new WaitForSeconds(1f);
        _dust2.SetActive(false);
        StartCircleExplode();
    }

    private IEnumerator C_Explode(SpriteRenderer sprite1, SpriteRenderer sprite2, SpriteRenderer sprite3 = null)
    {
        float elapsed = 0f;
        bool isPlayed = false;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float amount = Mathf.Clamp01(elapsed/0.7f);
            SetFill(sprite1, amount);
            SetFill(sprite2, amount);
            if (sprite3 != null)
            {
                SetFill(sprite3, amount);
            }

            if (elapsed >= 0.1f && !isPlayed)
            {
                ExplosionSound();
                isPlayed = true;
            }
            
            yield return null;
        }
        SetFill(sprite1, 1f);
        SetFill(sprite2, 1f);
        if (sprite3 != null)
        {
            SetFill(sprite3, 1f);
        }
    }

    private void ExplosionSound()
    {
        AudioManager.Instance.PlaySFX("토파즈 필드");
    }
    
    private void SetFill(SpriteRenderer sprite, float amount)
    {
        sprite.material.SetFloat("_Cutoff", amount);
    }

    private void StartCircleExplode()
    {
        _circleExplosion.SetActive(true);
        _circleExplosion.transform.localScale = Vector3.zero;
        LoudExplosionSound();
        _circleExplosion.transform.DOScale(Vector3.one * 32f, 0.5f).OnComplete(StartCircleFadeIn);
    }

    private void LoudExplosionSound()
    {
        AudioManager.Instance.PlaySFX("Explosion");
    }

    [Button]
    private void ShakeCamera(float duration = 0.2f, float intensity = 0.1f, float delay = 0f)
    {
        StartCoroutine(C_Shake(duration, intensity, delay));
    }

    private IEnumerator C_Shake(float duration, float intensity, float delay)
    {
        Vector3 original = _mainCamera.transform.position;
        yield return new WaitForSeconds(delay);
        while (duration > 0f)
        {
            _mainCamera.transform.position = original + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0f);
            duration -= Time.deltaTime;
            yield return null;
        }
        _mainCamera.transform.position = original;
    }
    
    private void StartCircleFadeIn()
    {
        _circleFadeIn.SetActive(true);
        _circleFadeIn.transform.localScale = Vector3.zero;
        _circleFadeIn.transform.DOScale(Vector3.one * 32f, 0.5f).OnComplete(AnimationOff);
    }

    private void AnimationOff()
    {
        CameraAction();
        StartSceneManager.Instance.StartSceneUI.TitleAnimation();
    }

        
    public void CameraAction()
    {
        
        Vector2 startPos = new Vector2(
            Random.Range(_bottomLeft.x, _topRight.x),
            Random.Range(_bottomLeft.y, _topRight.y));
        _mainCamera.transform.position = new Vector3(startPos.x, startPos.y, -10f);

        Vector2[] dirs =
        {
            new Vector2(1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, 1f), new Vector2(-1f, -1f)
        };
        Vector2 dir = dirs[Random.Range(0, dirs.Length)].normalized;

        _cameraCoroutine = StartCoroutine(C_BounceCamera(dir));
    }

    private IEnumerator C_BounceCamera(Vector2 dir)
    {
        while (true)
        {
            Vector3 pos = _mainCamera.transform.position;
            
            Vector3 nextPos = pos + (Vector3)(dir * 2f * Time.deltaTime);

            if (nextPos.x < _bottomLeft.x || nextPos.x > _topRight.x)
            {
                dir.x *= -1;
                nextPos.x = Mathf.Clamp(nextPos.x, _bottomLeft.x, _topRight.x);
            }

            if (nextPos.y < _bottomLeft.y || nextPos.y > _topRight.y)
            {
                dir.y *= -1;
                nextPos.y = Mathf.Clamp(nextPos.y, _bottomLeft.y, _topRight.y);
            }
            _mainCamera.transform.position = nextPos;
            yield return null;
        }
    }
    
    public void StopCamera()
    {
        if (_cameraCoroutine == null)
        {
            return;
        }

        StopCoroutine(_cameraCoroutine);
        _cameraCoroutine = null;
    }
}
