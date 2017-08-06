using UnityEngine;
using System.Collections;

public class TankEnemyGreen : TankEnemy
{
    new void Awake()
    {
        base.Awake();
        // This needs to be called in awake so that it is instantiated earlier than GUI_HUD.
        tankColor = tankColors[1];
        ColorizeTank();

        // Set the member variables for an easy TankEnemy.

        // Shooting Variables
        fireFreqFight = 5f;
        fireFreqChase = 10f;
        m_RotateSpeed = .5f;

        // Driving Variables
        m_CurrentSpeed = 3f;
    }

    
    new void Start()
    {
        base.Start();

        projectile = Resources.Load("TankResources/Projectile/ShellEnemyGreen") as GameObject;
    }
}
