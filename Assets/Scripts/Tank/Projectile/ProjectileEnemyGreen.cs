using UnityEngine;
using System.Collections;

public class ProjectileEnemyGreen : ProjectileEnemy
{
    protected new void Start()
    {
        base.Start();
        projectileSpeed = 3;
        maxCollisions = 0;
    }
}
