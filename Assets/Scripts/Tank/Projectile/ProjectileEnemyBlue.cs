using UnityEngine;
using System.Collections;

public class ProjectileEnemyBlue : ProjectileEnemy
{
    protected new void setVariables()
    {
        maxCollisions = 1;
    }
}