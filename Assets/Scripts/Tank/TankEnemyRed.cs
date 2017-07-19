using UnityEngine;
using System.Collections;

public class TankEnemyRed : TankEnemy
{
    new void Awake()
    {
        base.Awake();
        // This needs to be called in awake so that it is instantiated earlier than GUI_HUD.
        tankColor = tankColors[2];
        ColorizeTank();
    }

    private new void Start()
    {
        base.Start();

        // The FSM begins on Evade.
        setToEvade();

        // Initiate the driving variables.
        m_Speed = 6f;
        m_RotateSpeed = 1f;
        turningTimeNext = Random.Range(turningTimeMin, turningTimeMax);
        m_CurrentSpeed = m_Speed;

        // Initiate the shooting variables.
        fireFreq = fireFreqChase;
        //recentShots = player1.transform.position - tower.position;

        // Get the script of player 1.
        player1 = GameObject.FindGameObjectWithTag("Player");
        //player1Script = player1.GetComponent<TankPlayer>();

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("TankResources/ShellEnemy") as GameObject;

        // Get the script of the projectile and record its speed.
        projTestScript = GetComponent<ProjectileTest>();
        projTest = Resources.Load("TankResources/ProjTest") as GameObject;


    }

    private new IEnumerator FSM()
    {
        while (alive)
        {
            trackPlayer();

            switch (state)
            {
                case State.EVADE:
                    Evade();
                    break;
            }
            yield return null;
        }
    }
}
