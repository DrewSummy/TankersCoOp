using UnityEngine;
using Completed;

public class ProjectilePlayer : Projectile
{


    // Occurs on regular instances; tanks, max collisions, other projectiles.
    public override void KillProjectile()
    {
        base.KillProjectile();
        
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateProjectiles();
    }

    //TODO: figure out which one of these is obsolete vv: Used by RoomManager to remove extra projectiles.
    public override void RemoveProjectile()
    {
        base.RemoveProjectile();


        parentTank.GetComponent<Tank>().increaseProjCount();

        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateProjectiles();
    }

    // Used by RoomManager to remove extra projectiles. Launches projectiles randomly at the end of a level.
    public override void DisableProjectile()
    {
        base.DisableProjectile();


        parentTank.GetComponent<Tank>().increaseProjCount();

        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateProjectiles();
    }
}
