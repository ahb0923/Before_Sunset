using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    private const float HPBAR_LERP_SPEED = 0.5f;

    private BaseTower _tower;
    private Coroutine _hpDelayCoroutine;

    public Canvas canvas;

    public SpriteRenderer icon;
    public List<Sprite> constructionIcon;   // 추후에 건설이미지 추가가 된다면 사용, 아닐경우 삭제o

    public Image hpBar_delay;
    public Image hpBar_immediate;
 


    private void Reset()
    {
        canvas = Helper_Component.GetComponentInChildren<Canvas>(gameObject, "UI");
        icon = Helper_Component.GetComponentInChildren<SpriteRenderer>(gameObject);
        hpBar_delay = Helper_Component.FindChildComponent<Image>(transform, "HpBar_Delay"); 
        hpBar_immediate = Helper_Component.FindChildComponent<Image>(transform, "HpBar_Immediate");
    }

    public void Init(BaseTower baseTower)
    {
        _tower = baseTower;
        _tower.statHandler.OnHpChanged += UpdateHpBar;
    }

    /// <summary>
    /// 체력바 업데이트
    /// </summary>
    /// <param name="curr">현재 체력</param>
    /// <param name="max">최대 체력</param>
    public void UpdateHpBar(float curr, float max)
    {
        float ratio = Mathf.Clamp01(curr / max);

        hpBar_immediate.fillAmount = ratio;
        if (_hpDelayCoroutine != null)
            StopCoroutine(_hpDelayCoroutine);

        _hpDelayCoroutine = StartCoroutine(C_UpdateHpBar(ratio));
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
    }

}
