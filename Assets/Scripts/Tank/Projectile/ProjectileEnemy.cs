using UnityEngine;
using System.Collections;

public class ProjectileEnemy : Projectile
{
    protected override void resetVariables()
    {
        maxCollisions = 0;
    }
}
