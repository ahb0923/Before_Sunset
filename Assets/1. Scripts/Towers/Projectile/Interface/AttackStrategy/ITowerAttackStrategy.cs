using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
    IEnumerator Attack(BaseTower tower);
}
