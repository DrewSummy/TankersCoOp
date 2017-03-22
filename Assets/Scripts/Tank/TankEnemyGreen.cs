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
    }
}
