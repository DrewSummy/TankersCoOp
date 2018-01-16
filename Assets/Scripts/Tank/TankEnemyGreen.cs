using UnityEngine;
using System.Collections;

public class TankEnemyGreen : TankEnemy
{
    protected override void resetVariables()
    {
        // General variables

        // State variables

        // Shooting variables
        fireFreq = 3f;

        // Driving variables
        m_Speed = 3f;
    }
    
    protected new void Start()
    {
        base.Start();

        // The FSM begins on Evade.
        setToFight();
    }
    

    protected override IEnumerator FSM()
    {
        while (alive)
        {
            trackPlayer();
            switch (state)
            {
                case State.FIGHT:
                    Fight();
                    break;
                case State.IDLE:
                    Idle();
                    break;
            }
            yield return null;
        }
    }

    /*
    Functions for the FIGHT state:
    fightCS() - Overrides the change state funciton to never change states from FIGHT.
    */
    protected override void fightCS()
    {
        // Never change states
    }
}
