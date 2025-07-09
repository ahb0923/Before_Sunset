using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileTarget
{
    GameObject SelectTarget(BaseTower tower);
}
