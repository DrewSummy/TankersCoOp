using UnityEngine;
using System.Collections;
using Completed; // {}?

public class ProjectilePlayer : Projectile
{

    // TODO: set variables dependent on player

    private new void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.transform.tag == "Enemy")
        {
            // Add the kill to the parent tank.
            parentTank.GetComponent<TankPlayer>().killAmount++;
            parentTank.GetComponent<TankPlayer>().updateKillTracker(collisionInfo.transform.GetComponent<TankEnemy>().tankColor);


            //collisionInfor.GetComponent<TankEnemy>().typeOfEnemyTank
        }

        base.OnCollisionEnter(collisionInfo);
    }

    // Occurs on regular instances; tanks, max collisions, other projectiles.
    public override void KillProjectile()
    {
        base.KillProjectile();

        parentTank.GetComponent<Tank>().increaseProjCount();
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateP1Projectiles();
    }

    // Obsolete: use DisableProjectile(). Used by RoomManager to remove extra projectiles.
    public override void RemoveProjectile()
    {
        base.RemoveProjectile();

        parentTank.GetComponent<Tank>().increaseProjCount();
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateP1Projectiles();
    }

    // Used by RoomManager to remove extra projectiles. Launches projectiles randomly at the end of a level.
    public override void DisableProjectile()
    {
        base.DisableProjectile();

        parentTank.GetComponent<Tank>().increaseProjCount();
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateP1Projectiles();
    }
}
