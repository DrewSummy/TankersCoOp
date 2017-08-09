using UnityEngine;
using System.Collections;

public class ProjectileEnemy : Projectile
{
    override protected void setProjectileAttributes()
    {
        maxCollisions = 2;
    }
}
