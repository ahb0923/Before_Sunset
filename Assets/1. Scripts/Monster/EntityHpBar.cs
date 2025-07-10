using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityHpBar : MonoBehaviour
{
    private const float LERP_SPEED = 0.5f;

    [SerializeField] private Image _delay;
    [SerializeField] private Image _immediate;

    private int _maxHp;
    private Coroutine _hpDelayCoroutine;

    public void Init(int maxHp)
    {
        _maxHp = maxHp;
    }

    public void SetFullHpBar()
    {
        _delay.fillAmount = 1;
        _immediate.fillAmount = 1;
    }

    public void UpdateHpBar(int curHp)
    {
        float ratio = (float)curHp / _maxHp;

        _immediate.fillAmount = ratio;
        if (_hpDelayCoroutine != null)
            StopCoroutine(_hpDelayCoroutine);

        _hpDelayCoroutine = StartCoroutine(C_UpdateHpBar(ratio));
    }

    private IEnumerator C_UpdateHpBar(float ratio)
    {
        yield return Helper_Coroutine.WaitSeconds(0.5f);

        float current = _delay.fillAmount;

        while (current > ratio)
        {
            current = Mathf.MoveTowards(current, ratio, Time.deltaTime * LERP_SPEED);
            _delay.fillAmount = current;
            yield return null;
        }

        _delay.fillAmount = ratio;
        _hpDelayCoroutine = null;
    }
}
