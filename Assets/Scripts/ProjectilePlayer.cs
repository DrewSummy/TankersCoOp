using UnityEngine;
using System.Collections;

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

    public override void KillProjectile()
    {
        base.KillProjectile();

        parentTank.GetComponent<Tank>().projectileCount--;
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateP1Projectiles();
    }

    public override void RemoveProjectile()
    {
        base.RemoveProjectile();
        
        parentTank.GetComponent<Tank>().projectileCount--;
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateP1Projectiles();
    }

    public override void DisableProjectile()
    {
        base.DisableProjectile();

        parentTank.GetComponent<Tank>().projectileCount--;
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateP1Projectiles();
    }
}
