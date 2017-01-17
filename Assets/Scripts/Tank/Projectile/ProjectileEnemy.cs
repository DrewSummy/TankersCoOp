using UnityEngine;
using System.Collections;

public class ProjectileEnemy : Projectile {


    protected new void Start()
    {
        base.Start();
        maxCollisions = 2;
    }
}
