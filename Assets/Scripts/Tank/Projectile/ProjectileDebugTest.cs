using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDebugTest : Projectile
{
    protected override void resetVariables()
    {
        maxCollisions = 4;
    }

    public override void KillProjectile()
    {
        Destroy(gameObject);
    }

    protected override void setTrail()
    {
        return;
    }
}
