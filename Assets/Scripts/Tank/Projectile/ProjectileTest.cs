﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTest : MonoBehaviour {


    protected int maxCollisions = 2;               // The max number of collisions before the object is destroyed. 
    protected Tank tankScript;                     // Reference to the parent TankEnemy.

    private string playerTag = "Player";           // The string for the player tag.
    public LayerMask raycastLayer;                 // The layerMask for the ProjectileTest to avoid the shell game objects.
    private float maxDist = 70;                    // The maximum distance of the raycast.

    private struct pt                              // A class that contains a position, direction, and counter to represent a projectile test.
    {
        public Vector3 pos;
        public Vector3 dir;
        public int collisionCounter;

        public pt(Vector3 position, Vector3 direction)
        {
            pos = position;
            dir = direction;
            collisionCounter = 0;
        }
    }


    protected void Start()
    {
        //TODO: test this
        raycastLayer = ~(1 << LayerMask.NameToLayer("Ignore Raycasat"));
    }

    public bool beginShoot(Vector3 position, Vector3 direction)
    {
        pt shot = new pt(position, direction);

        return shoot(shot);
    }

    private bool shoot(pt s)
    {

        Debug.DrawLine(s.pos, s.pos + s.dir * 30, Color.white, 0.5f);

        // Send out raycast
        RaycastHit hit;
        Physics.Raycast(s.pos, s.dir * maxDist, out hit, raycastLayer);

        // If the raycast doesn't hit anything at all.
        if (hit.collider == null)
        {
            return false;
        }
        // If it hit a tank.
        else if (hit.transform.tag == playerTag)
        {
            return true;
        }
        // Else it hit a wall or block.
        else
        {

            // No more collisions
            if (s.collisionCounter >= maxCollisions)
            {
                return false;
            }
            // More collisionselse
            else
            {
                // Increment counter.
                ++s.collisionCounter;

                // Record the new position.
                s.pos = hit.point;

                // Calculate the new direction with the normal of the collision.
                Vector3 normalCollision = hit.normal;
                s.dir = Vector3.Reflect(s.dir, normalCollision);
                
                return shoot(s);
            }
        }
    }
}
