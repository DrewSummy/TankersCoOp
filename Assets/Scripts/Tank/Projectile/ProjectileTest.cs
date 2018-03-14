using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Completed;

public class ProjectileTest : MonoBehaviour {
    
    public int maxCollisions = 2;                  // The max number of collisions before the object is destroyed.
    public string enemyTeamName;                   // String to check if the object hit was an enemy.
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

    public struct ShotReport                       // A struct to report if the shot hit, how many collisions occurred, and the distance it travelled.
    {
        public bool isHit;
        public int collisionCount;
        public float distanceTravelled;

        public ShotReport(bool ih, int cc, float dt)
        {
            isHit = ih;
            collisionCount = cc;
            distanceTravelled = dt;
        }
    }


    protected void Start()
    {
        raycastLayer = ~(1 << LayerMask.NameToLayer("Ignore Raycasat"));
    }

    public ShotReport beginShoot(Vector3 position, Vector3 direction, bool debug = false)
    {
        pt shot = new pt(position, direction);

        return shoot(shot, 0, debug);
    }

    // Returns the distance traveled to hit an enemy. -1 is reserved for never hitting and -2 is reserved for hitting a teammate.


    // Returns a struct with a boolean for hitting, the number of walls hit, and the float for the distance travelled.
    private ShotReport shoot(pt s, float distWeight, bool debug = false)
    {
        if (debug)
        {
            Debug.DrawLine(s.pos, s.pos + s.dir * 30, Color.white, 1.5f);
        }

        // Send out raycast
        RaycastHit hit;
        Physics.Raycast(s.pos, s.dir * maxDist, out hit, raycastLayer);
        distWeight += hit.distance;

        // If the raycast doesn't hit anything at all.
        if (hit.collider == null)
        {
            return new ShotReport(false, s.collisionCounter, distWeight);
        }
        // If it hit a tank.
        else if (hit.transform.GetComponent<Tank>())
        {
            if (hit.transform.GetComponent<Tank>().teamName == enemyTeamName)
            {
                return new ShotReport(true, s.collisionCounter, distWeight);
            }
            else
            {
                return new ShotReport(false, s.collisionCounter, distWeight);
            }
        }
        // Else it hit a wall or block.
        else
        {
            // No more collisions
            if (s.collisionCounter >= maxCollisions)
            {
                return new ShotReport(false, s.collisionCounter, distWeight);
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
                
                return shoot(s, distWeight, debug);
            }
        }
    }
}
