using UnityEngine;
using System.Collections;

public class ProjectileEnemyRed : ProjectileEnemy
{
    override protected void setProjectileAttributes()
    {
        projectileSpeed = 12f;
        maxCollisions = 0;
    }
}
