using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileMovement_Curved : IProjectileMovement
{
    private Transform _self;
    private GameObject _target;

    private Vector3 _start;
    private Vector3 _end;
    private float _elapsed;
    private float _duration;
    private float _maxHeight;
    public void Init(ProjectileAttackSettings attackSettings, ProjectileMovementSettings moveSettings)
    {
        _self = attackSettings.attacker.transform;
        _target = attackSettings.target;

        _start = _self.position;
        _end = _target.transform.position;
        _elapsed = 0f;
        _duration = moveSettings.duration;
        //_duration = 2f;
        //_maxHeight = moveSettings.maxHeight;
        _maxHeight = 2f;
    }

    public bool Movement()
    {
        _elapsed += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(_elapsed / _duration);

        Vector3 projectilePosition = Vector3.Lerp(_start, _end, normalizedTime);
        projectilePosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * _maxHeight;
        _self.position = projectilePosition;

        Vector3 direction = _end - _start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _self.rotation = Quaternion.Euler(0, 0, angle - 90f);

        return normalizedTime >= 1f;
    }
}
