using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _currentGoal;

    private const float HIDE_DISTANCE = 3f; // 목표와의 거리
    private bool _isArrowVisible = false;

    private void Update()
    {
        if (_currentGoal == null)
        {
            SetArrowVisible(false);
            return;
        }

        float distance = Vector3.Distance(_currentGoal.position, transform.position);

        if (distance >= HIDE_DISTANCE)
        {
            SetArrowVisible(true);
            RotationArrow();
        }
        else
        {
            SetArrowVisible(false);
        }
    }

    /// <summary>목표 지점 세팅</summary>
    public void SettingTarget(Transform goal)
    {
        _currentGoal = goal;
    }

    /// <summary>화살표 회전 처리</summary>
    private void RotationArrow()
    {
        Vector3 dir = (_currentGoal.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);
        transform.rotation = rot;
    }

    /// <summary>화살표 켜기/끄기</summary>
    public void SetArrowVisible(bool visible)
    {
        if (_isArrowVisible == visible) return; // 상태 변화 없으면 패스
        _isArrowVisible = visible;

        if (_anim != null)
        {
            _anim.SetBool("IsHide", !visible); // 🔹 visible 반대로 전달
        }
    }
}
