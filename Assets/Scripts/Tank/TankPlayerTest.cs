using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayerTest : TankPlayer
{


    protected new void Start()
    {
        base.Start();

        rotateOnly(false);
        disableShoot(false);
    }


    public override void DestroyTank()
    {
        // Freeze the tank from moving.
        m_RidgidbodyTank.velocity = Vector3.zero;
        m_RidgidbodyTank.freezeRotation = true;

        // Give projectiles to the room's projectileHolder.
        TransferProjectiles();

        // Immaterialize the tank.
        //TODO: this is immaterializing the projectiles

        for (int i = 0; i < GetComponentsInChildren<MeshRenderer>().Length; i++)
        {
            GetComponentsInChildren<MeshRenderer>()[i].enabled = false;
        }

        hitbox.enabled = false;

        //TODO: place explosion or some animation

        // Set alive to be false. Other scripts depend on this.
        alive = false;

        // Set projectile count to 0.
        projectileCount = 0;
        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateProjectiles();
        
        // Destroy tank.
        //Destroy(this.gameObject);
        //Debug.Log("here");
    }
}
