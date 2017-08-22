using UnityEngine;
using System.Collections;

public class TankEnemyGreen : TankEnemy
{
    // General variables

    // State variables

    // Shooting variables
    
    // Driving variables
    private new float m_Speed = 3f;//TODO: is this inherited

    protected new void Start()
    {
        base.Start();

        // The FSM begins on Evade.
        setToExplore();
    }
    

    private new IEnumerator FSM()
    {
        while (alive)
        {
            trackPlayer();
            switch (state)
            {
                case State.EXPLORE:
                    Explore();
                    break;
            }
            yield return null;
        }
    }

    /*
    Functions for the EXPLORE state:
    exploreCS() - Overrides the change state funciton to never change states from EXPLORE.
    */
    protected override void exploreCS()
    {
        // Never change states
    }
}
