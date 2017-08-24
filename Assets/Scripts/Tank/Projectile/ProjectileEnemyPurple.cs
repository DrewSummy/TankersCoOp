using UnityEngine;
using System.Collections;

public class ProjectileEnemyPurple : ProjectileEnemy
{
    //new float projectileSpeed = 20f;
    
    protected override void resetVariables()
    {
        maxCollisions = 2;
        projectileSpeed = 24f;
    }
}
