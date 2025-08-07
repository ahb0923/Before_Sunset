using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WARNING_DIRECTION
{
    Up,
    Right,
    Down, 
    Left, 
}
public class WarningArrow : MonoBehaviour
{
    [SerializeField] private Transform UpSide;
    private SpriteRenderer[] _upSpriters;
    [SerializeField] private Transform DownSide;
    private SpriteRenderer[] _downSpriters;
    [SerializeField] private Transform LeftSide;
    private SpriteRenderer[] _leftSpriters;
    [SerializeField] private Transform RightSide;
    private SpriteRenderer[] _rightSpriters;

    [SerializeField] private Color _originColor;
    [SerializeField] private Color _displayColor;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _changingTime = 0.5f;

    private Coroutine[] _displayCoroutines = new Coroutine[4];

    private void Reset()
    {
        UpSide = Helper_Component.FindChildByName(transform, "UpSide");
        DownSide = Helper_Component.FindChildByName(transform, "DownSide");
        LeftSide = Helper_Component.FindChildByName(transform, "LeftSide");
        RightSide = Helper_Component.FindChildByName(transform, "RightSide");
    }

    private void Awake()
    {
        _upSpriters = UpSide.GetComponentsInChildren<SpriteRenderer>();
        _downSpriters = DownSide.GetComponentsInChildren<SpriteRenderer>();
        _leftSpriters = LeftSide.GetComponentsInChildren<SpriteRenderer>();
        _rightSpriters = RightSide.GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        OffAllWarning();
    }

    /// <summary>
    /// 다음 스테이지의 몬스터 스폰 방향 표시
    /// </summary>
    public void DisplayMonsterSpawnDirection(WARNING_DIRECTION dir, bool isWarning)
    {
        Transform arrow = null;
        SpriteRenderer[] renderers = null;

        switch (dir)
        {
            case WARNING_DIRECTION.Up:
                arrow = UpSide;
                renderers = _upSpriters;
                break;

            case WARNING_DIRECTION.Down:
                arrow = DownSide;
                renderers = _downSpriters;
                break;

            case WARNING_DIRECTION.Left:
                arrow = LeftSide;
                renderers = _leftSpriters;
                break;

            case WARNING_DIRECTION.Right:
                arrow = RightSide;
                renderers = _rightSpriters;
                break;
        }

        if (_displayCoroutines[(int)dir] != null)
            StopCoroutine(_displayCoroutines[(int)dir]);
        _displayCoroutines[(int)dir] = StartCoroutine(C_DisplayWithDurationi(arrow, renderers, isWarning , dir));
    }

    /// <summary>
    /// 지속 시간 동안 화살표 활성화 후 비활성화
    /// </summary>
    private IEnumerator C_DisplayWithDurationi(Transform arrow, SpriteRenderer[] renderers, bool isWarning, WARNING_DIRECTION dir)
    {
        if(arrow == null || renderers == null)
            yield break;

        arrow.gameObject.SetActive(true);
        int repeat = 2;
        while(repeat > 0)
        {
            // 서서히 보이기
            float timer = 0f;
            while(timer <= _changingTime)
            {
                foreach (SpriteRenderer renderer in renderers)
                {
                    float alpha = isWarning ? _originColor.a : _displayColor.a;
                    renderer.color = isWarning ? _originColor.WithAlpha(timer / _changingTime * alpha) : _displayColor.WithAlpha(timer / _changingTime * alpha);
                }
                timer += Time.deltaTime;
                yield return null;
            }

            yield return Helper_Coroutine.WaitSeconds(_duration);

            // 서서히 안 보이기
            timer = _changingTime;
            while (timer >= 0)
            {
                foreach (SpriteRenderer renderer in renderers)
                {
                    float alpha = isWarning ? _originColor.a : _displayColor.a;
                    renderer.color = isWarning ? _originColor.WithAlpha(timer / _changingTime * alpha) : _displayColor.WithAlpha(timer / _changingTime * alpha);
                }
                timer -= Time.deltaTime;
                yield return null;
            }

            if(isWarning) repeat--;
        }
        arrow.gameObject.SetActive(false);

        _displayCoroutines[(int)dir] = null;
    }

    /// <summary> 모든 워닝사인 OFF </summary>
    public void OffAllWarning()
    {
        UpSide.gameObject.SetActive(false);
        DownSide.gameObject.SetActive(false);
        LeftSide.gameObject.SetActive(false);
        RightSide.gameObject.SetActive(false);
    }

    #region For Editor
    [ContextMenu("up")]
    public void Upside()
    {
        UpSide.gameObject.SetActive(true);
    }
    [ContextMenu("down")]
    public void Downside()
    {
        DownSide.gameObject.SetActive(true);
    }
    [ContextMenu("left")]
    public void Leftside()
    {
        LeftSide.gameObject.SetActive(true);
        
    }
    [ContextMenu("right")]
    public void Rightside()
    {
        RightSide.gameObject.SetActive(true);
    }
    [ContextMenu("Off")]
    public void Off()
    {
        OffAllWarning();
    }
    #endregion
}
