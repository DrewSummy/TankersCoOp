using UnityEngine;
using System.Collections;

public class TankEnemyBlue : TankEnemy
{
    protected override void resetVariables()
    {
        // General variables

        // State variables

        // Shooting variables
        fireFreq = 2f;

        // Driving variables
        m_RotateSpeed = 3f;
    }
}
