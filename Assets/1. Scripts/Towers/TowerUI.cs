using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    private const float HPBAR_LERP_SPEED = 0.5f;
    private const float HPBAR_VISIBLE_DURATION = 3f;
    private const float HPBAR_FADE_DURATION = 1f;

    private BaseTower _tower;
    private Coroutine _hpDelayCoroutine;
    private Coroutine _fadeOutCoroutine;

    public Canvas canvas;

    public SpriteRenderer icon;
    public List<Sprite> constructionIcon;   // 추후에 건설이미지 추가가 된다면 사용, 아닐경우 삭제o

    public GameObject hpBar;
    public Image hpBar_delay;
    public Image hpBar_immediate;
    private CanvasGroup _hpBarCanvasGroup;
    public SpriteRenderer effectArea;
    public Animator animator;


    private void Reset()
    {
        canvas = Helper_Component.GetComponentInChildren<Canvas>(gameObject, "UI");
        icon = Helper_Component.GetComponentInChildren<SpriteRenderer>(gameObject);
        animator = Helper_Component.GetComponentInChildren<Animator>(gameObject);
        hpBar = Helper_Component.FindChildByName(transform, "HpBar").gameObject;
        hpBar_delay = Helper_Component.FindChildComponent<Image>(transform, "HpBar_Delay"); 
        hpBar_immediate = Helper_Component.FindChildComponent<Image>(transform, "HpBar_Immediate");
    }
    private void Awake()
    {
        _hpBarCanvasGroup = hpBar.GetComponent<CanvasGroup>();
        if (_hpBarCanvasGroup == null)
            _hpBarCanvasGroup = hpBar.AddComponent<CanvasGroup>();

        _hpBarCanvasGroup.alpha = 0f;
    }
    public void Init(BaseTower baseTower)
    {
        _tower = baseTower;
        _tower.statHandler.OnHpChanged += UpdateHpBar;
    }

    public void SetHpBarAlpha(float alpha)
    {
        _hpBarCanvasGroup.alpha = Mathf.Clamp01(alpha);
    }

    public void SetConstructionProgress(float progress) 
    {
        float hp = Mathf.Lerp(0, _tower.statHandler.MaxHp, progress);
        UpdateHpBar(hp, _tower.statHandler.MaxHp);
        //icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, Mathf.Lerp(0.5f, 1f, progress));
    }

    /// <summary>
    /// 체력바 업데이트
    /// </summary>
    /// <param name="curr">현재 체력</param>
    /// <param name="max">최대 체력</param>
    public void UpdateHpBar(float curr, float max)
    {
        if (!gameObject.activeInHierarchy) return;

        ShowHpBar();

        float ratio = Mathf.Clamp01(curr / max);

        hpBar_immediate.fillAmount = ratio;
        if (_hpDelayCoroutine != null)
            StopCoroutine(_hpDelayCoroutine);

        _hpDelayCoroutine = StartCoroutine(C_UpdateHpBar(ratio));
    }

    private void ShowHpBar()
    {
        _hpBarCanvasGroup.alpha = 1f;

        if (_fadeOutCoroutine != null)
            StopCoroutine(_fadeOutCoroutine);

        _fadeOutCoroutine = StartCoroutine(C_FadeHpBar());
    }
    private IEnumerator C_FadeHpBar()
    {
        yield return new WaitForSeconds(HPBAR_VISIBLE_DURATION);

        float time = 0f;
        while (time < HPBAR_FADE_DURATION)
        {
            time += Time.deltaTime;
            float t = 1f - (time / HPBAR_FADE_DURATION);
            _hpBarCanvasGroup.alpha = t;
            yield return null;
        }

        _hpBarCanvasGroup.alpha = 0f;
        _fadeOutCoroutine = null;
    }

    /// <summary>
    /// 체력바 러프 업데이트
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    private IEnumerator C_UpdateHpBar(float ratio)
    {
        yield return Helper_Coroutine.WaitSeconds(0.5f);

        float current = hpBar_delay.fillAmount;

        while (current > ratio)
        {
            current = Mathf.MoveTowards(current, ratio, Time.deltaTime * HPBAR_LERP_SPEED);
            hpBar_delay.fillAmount = current;
            yield return null;
        }

        hpBar_delay.fillAmount = ratio;
        _hpDelayCoroutine = null;
    }

    public void ResetHpBar()
    {
        float ratio = Mathf.Clamp01(_tower.statHandler.CurrHp / _tower.statHandler.MaxHp);
        hpBar_immediate.fillAmount = ratio;
        hpBar_delay.fillAmount = ratio;

        if (_hpDelayCoroutine != null)
        {
            StopCoroutine(_hpDelayCoroutine);
            _hpDelayCoroutine = null;
        }

        if (_fadeOutCoroutine != null)
        {
            StopCoroutine(_fadeOutCoroutine);
            _fadeOutCoroutine = null;
        }

        _hpBarCanvasGroup.alpha = 0f;
    }
    public void ResetAnimation()
    {
        animator.Rebind();      // 트랜스폼 포함 초기화
        animator.Update(0f);    // 즉시 적용
    }

    public void OffAttackArea()
    {
        effectArea.gameObject.SetActive(false);
    }
    public void OnAttackArea()
    {
        effectArea.gameObject.SetActive(true);
    }

    public void SetEffectSize(float radius)
    {
        radius += 0.5f;
        effectArea.size = new Vector2 (radius, radius);
    }
}
