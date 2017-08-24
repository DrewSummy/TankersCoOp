using UnityEngine;
using System.Collections;

public class ProjectileEnemyBlue : ProjectileEnemy
{
    protected override void resetVariables()
    {
        maxCollisions = 1;
    }
}