using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement_Straight : IProjectileMovement
{
    private Transform _self;
    private GameObject _target;
    private float _moveSpeed;

    public void Init(ProjectileAttackSettings attackSettings, ProjectileMovementSettings moveSettings)
    {
        _self = attackSettings.attacker.transform;
        _target = attackSettings.target;
        _moveSpeed = moveSettings.moveSpeed;
    }

    public bool Movement()
    {
        if (_target == null) return true;

        Vector3 dir = (_target.transform.position - _self.position).normalized;
        _self.position += dir * _moveSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _self.rotation = Quaternion.Euler(0, 0, angle - 90f);

        return Vector3.Distance(_self.position, _target.transform.position) < 0.3f;
    }
}
