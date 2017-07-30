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