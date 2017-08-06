using UnityEngine;
using System.Collections;

public class ProjectileEnemyRed : ProjectileEnemy
{
    protected new void Start()
    {
        base.Start();
        projectileSpeed = 1;
        maxCollisions = 0;
    }
}
