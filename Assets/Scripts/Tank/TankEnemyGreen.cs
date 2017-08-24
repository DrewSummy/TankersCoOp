using UnityEngine;
using System.Collections;

public class TankEnemyGreen : TankEnemy
{
    // General variables

    // State variables

    // Shooting variables
    
    // Driving variables
    //private new float m_Speed = 3f;


    protected override void resetVariables()
    {
        // Driving variables
        m_Speed = 3f;
    }

    protected new void Start()
    {
        base.Start();

        // The FSM begins on Evade.
        setToFight();
    }
    

    private new IEnumerator FSM()
    {
        while (alive)
        {
            trackPlayer();
            switch (state)
            {
                case State.FIGHT:
                    Fight();
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
