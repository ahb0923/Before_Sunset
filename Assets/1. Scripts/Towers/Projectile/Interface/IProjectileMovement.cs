using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileMovement
{
    public void Init(ProjectileAttackSettings attackSettings, ProjectileMovementSettings moveSettings);
    public bool Movement();
}

