using UnityEngine;
using System.Collections;

public class TankEnemyGreen : TankEnemy
{
    // State variables
    private float coolDownEnd = 4f;             // The length of time the tank stays aggressive without seeing the player.
    private float coolDownTimer;                // The length of time the tank has gone without seeing the player.

    // Shooting variables
    private float burstFireFreq = 2.5f;         // The amount of time between burst fires.
    protected float fireBurstFreq = .15f;       // The amount of time between each individual shot during burst fire.
    private float burstAccuracy = 15f;          // The max amount of degrees a burst shot can miss the player by.

    // Driving variables
    private float stopDistance = 20f;           // The distance the tank gets to the player tank before stopping.
    private float speedAggressive = 12f;        // The speed the tank drives at.


    new void Awake()
    {
        base.Awake();
        // This needs to be called in awake so that it is instantiated earlier than GUI_HUD.
        tankColor = tankColors[1];
        ColorizeTank();
    }


    new void Start()
    {
        base.Start();

        // Shooting Variables
        fireFreqFight = 5f;
        fireFreqChase = 10f;
        m_RotateSpeed = .5f;

        // Driving Variables
        m_CurrentSpeed = 3f;

        // Set projectile.
        projectile = Resources.Load("TankResources/Projectile/ShellEnemyGreen") as GameObject;
    }
}
