using UnityEngine;
using System.Collections;

public class TankEnemyBlue : TankEnemy
{
    new void Awake()
    {
        base.Awake();
        // This needs to be called in awake so that it is instantiated earlier than GUI_HUD.
        tankColor = tankColors[0];
        ColorizeTank();
    }
}
