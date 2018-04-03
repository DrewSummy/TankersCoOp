using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyBlack : ProjectileEnemy
{
    protected override void resetVariables()
    {
        maxCollisions = 1;
        projectileSpeed = 3.3f;
    }
}
