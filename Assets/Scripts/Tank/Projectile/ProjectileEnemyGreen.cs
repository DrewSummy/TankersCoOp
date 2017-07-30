using UnityEngine;
using System.Collections;

public class ProjectileEnemyGreen : ProjectileEnemy
{
    protected new void Start()
    {
        Debug.Log("projenemygrn start");
        base.Start();
        Debug.Log("projenemygrn start");
        projectileSpeed = 3;
        maxCollisions = 0;

        Debug.Log("//");
        Debug.Log(smokeTrail);
        Debug.Log("//");
    }
}
