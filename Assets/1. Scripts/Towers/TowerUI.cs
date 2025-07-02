using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    public const float hpBarLerpSpeed = 0.5f;

    [SerializeField] private BaseTower _tower;

    public Canvas canvas;
    public Image hpBar_delay;
    public Image hpBar_immediate;

    private Coroutine _hpDelayCoroutine;

    private void Reset()
    {
        _tower = Helper_Component.GetComponentInParent<BaseTower>(gameObject,"UI");
        canvas = Helper_Component.GetComponentInChildren<Canvas>(gameObject, "UI");
        hpBar_delay = Helper_Component.FindChildByName(transform,"HpBar_Delay").GetComponent<Image>();
        hpBar_immediate = Helper_Component.FindChildByName(transform, "HpBar_Immediate").GetComponent<Image>();
    }

    public void Init()
    {
        _tower.statHandler.OnHpChanged += UpdateHpBar;
    }

    private void UpdateHpBar(float curr, float max)
    {
        float ratio = Mathf.Clamp01(curr / max);

        hpBar_immediate.fillAmount = ratio;
        if (_hpDelayCoroutine != null)
            StopCoroutine(_hpDelayCoroutine);

        _hpDelayCoroutine = StartCoroutine(C_UpdateHpBar(ratio));
    }
    private IEnumerator C_UpdateHpBar(float ratio)
    {
        yield return Helper_Coroutine.WaitSeconds(1f);

        float current = hpBar_delay.fillAmount;

        while (current > ratio)
        {
            current = Mathf.MoveTowards(current, ratio, Time.deltaTime * hpBarLerpSpeed);
            hpBar_delay.fillAmount = current;
            yield return null;
        }

        hpBar_delay.fillAmount = ratio;
        _hpDelayCoroutine = null;
    }

}
