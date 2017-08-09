using UnityEngine;
using System.Collections;

public class ProjectileEnemyGreen : ProjectileEnemy
{
    override protected void setProjectileAttributes()
    {
        projectileSpeed = 8f;
        maxCollisions = 0;
    }
}
