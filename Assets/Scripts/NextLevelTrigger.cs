using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;

public class NextLevelTrigger : MonoBehaviour
{
    public GameMaster GM;
    

    protected void OnCollisionEnter(Collision collisionInfo)
    {
        // The object has collided with another projectile.
        if (collisionInfo.transform.tag == "Player")
        {
            //TODO: call next room
            GM.nextLevel();
        }
    }
}
