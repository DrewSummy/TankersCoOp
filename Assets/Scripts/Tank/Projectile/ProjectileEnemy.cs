using UnityEngine;
using System.Collections;

public class ProjectileEnemy : Projectile
{
    protected new void Start()
    {
        Debug.Log("projenemy start");
        base.Start();
        Debug.Log("projenemy start");
        maxCollisions = 2;
    }
}
