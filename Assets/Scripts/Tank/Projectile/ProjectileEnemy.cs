using UnityEngine;
using System.Collections;

public class ProjectileEnemy : Projectile
{
    protected override void setVariables()
    {
        maxCollisions = 0;
        Debug.Log(maxCollisions);
    }
}
