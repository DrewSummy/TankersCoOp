﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    //TODO: add a gameobject infront of the tower so that if a wall is inside it, the tank won't shoot

    public GameObject projectileExplosion;         // The GameObject of the projectile's explosion.
    public AudioSource audioSource;                // Reference to the audio source.
    public AudioClip ricochetAudio;                // The GameObject of the audio for ricochetting.
    public GameObject smokeTrail;
    public GameObject sender;

    protected int maxCollisions = 1;               // The max number of collisions before the object is destroyed. 
    protected int collisionCounter = 0;            // Used to keep track of the number of collisions.
    protected float projectileSpeed = 6.6f;           // The speed the projectile fires at.
    protected Vector3 projectileSpeedVector;       // The vector the projectile moves at.
    protected Rigidbody ProjectileRigidbody;       // Reference used to move the projectile.
    public GameObject parentTank;                  // Reference to the parent tank game object.
    protected Tank tankScript;                     // Reference to Tank to be able to call DestroyTank().
    private bool disabled = false;                 // Boolean for whether the projectile is disabled.
    private GameObject currentTrail;
    public bool transfered = false;
    private string projTag = "Projectile";
    private string playerTag = "Player";
    private string enemyTag = "Enemy";
    private float offset = .25f;
    private float maxDestroyTime = 20;

    // Remove projectile variables
    private float horProjExplVel = 2;
    private float vertProjExplVel = 4.5f;
    private float horProjExplTor = 2;




    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Rigidbody myRigidbody;
    private Collider myCollider;


    protected void Awake()
    {
        // Use the rigidbody and calculate the projectileSpeedVector.
        ProjectileRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Use this for initialization
    protected void Start()
    {
        resetVariables();

        // Add velocity to the projectile.
        ProjectileRigidbody.velocity = ProjectileRigidbody.transform.forward * projectileSpeed;
        projectileSpeedVector = ProjectileRigidbody.velocity;

        // Use the rigidbody and calculate the projectileSpeedVector.
        ProjectileRigidbody = gameObject.GetComponent<Rigidbody>();

        // Get rid of gravity.
        ProjectileRigidbody.useGravity = false;

        // Set the trail.
        setTrail();

        // Load in the explosion and trail being used from the Resources folder in assets.
        projectileExplosion = Resources.Load("Prefab/GameObjectPrefab/TankPrefab/Projectile/ShellExplosion") as GameObject;
        smokeTrail = Resources.Load("Prefab/GameObjectPrefab/TankPrefab/Projectile/SmokeTrail") as GameObject;

        // Get the AudioSource component of the game object.
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        // Load in the sound being used from the Resources folder in assets.
        ricochetAudio = Resources.Load("Prefab/Audio/ricochet") as AudioClip;
        audioSource.clip = ricochetAudio;

        // Collision variables.
        myRigidbody = ProjectileRigidbody;
        myCollider = GetComponentInChildren<Collider>();
        previousPosition = myRigidbody.position;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }
    
    // Used in inheritance to change projectile variables.
    protected virtual void resetVariables()
    {
        // Null
    }

    // Adds a smokeTrail object behind the projectile at start and every ricochet.
    protected virtual void setTrail()
    {
        currentTrail = Instantiate(smokeTrail) as GameObject;
        currentTrail.transform.SetParent(transform);
        currentTrail.transform.rotation = transform.rotation;
        currentTrail.transform.Rotate(new Vector3(0, 180, 0));
        currentTrail.transform.position = transform.position + offset * currentTrail.transform.forward;
        currentTrail.GetComponent<Rigidbody>().velocity = projectileSpeedVector;
        ParticleSystem.MainModule ps = currentTrail.GetComponent<ParticleSystem>().main;
        ps.simulationSpeed = projectileSpeed / ps.simulationSpeed;
    }


    protected void OnCollision(RaycastHit collisionInfo)
    {
        // The object has collided with another projectile.
        if (collisionInfo.transform.tag == projTag)
        {
            projectileCollision();
        }
        // The object has collided with a tank.
        else if (collisionInfo.transform.tag == playerTag || collisionInfo.transform.tag == enemyTag)
        {
            tankCollision(collisionInfo);
        }
        // The object has collided the max amount of times.
        else if (collisionCounter >= maxCollisions)
        {
            lastCollision();
        }
        // The object has more collisions and needs to rotate and continue.
        else
        {
            wallCollision(collisionInfo);
        }
    }


    /*
    Functions for collisions:
    projectileCollision() - Kill the projectile.
    tankCollision() - Kill the projectile and tank that was hit.
    lastCollision() - Kill the projectile.
    wallCollision() - Calculates a new direction based on angle of incidence, substitutes projectile trails, and plays audio.
    */
    public void projectileCollision()
    {
        if (!disabled)
        {
            // Drop the current smoke trail.
            KillProjectile();
        }
    }
    protected void tankCollision(RaycastHit ci)
    {
        if (!disabled)
        {
            // Drop the current smoke trail.
            ci.collider.gameObject.GetComponentInParent<Tank>().DestroyTank();
            //ci.gameObject.GetComponent<Tank>().DestroyTank();
            KillProjectile();
        }
    }
    protected void lastCollision()
    {
        // Drop the current smoke trail and destroy the object.
        KillProjectile();
    }
    protected void wallCollision(RaycastHit ci)
    {
        // A collsion has occured.
        collisionCounter++;
        myCollider.enabled = false;

        if (!disabled)
        {

            // If the audioclip isn't the ricochet audio make it that.
            if (audioSource.clip != ricochetAudio)
            {
                audioSource.clip = ricochetAudio;
            }
            // Play the audio for the ricochet.
            audioSource.Play();

            // Drop the current smoke trail.
            if (currentTrail)
            {
                currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
            }

            ProjectileRigidbody.freezeRotation = true;


            // Calculate the new vector follow the equation V' = 2(V.N)*N-V.

            // The vector normal to the collision.
            Vector3 normalCollision = ci.normal;

            // The vector the projectile reflects at.
            Vector3 newProjectileSpeedVector =
                Vector3.Reflect(projectileSpeedVector, normalCollision);

            ProjectileRigidbody.transform.forward = newProjectileSpeedVector;
            ProjectileRigidbody.velocity = newProjectileSpeedVector;
            projectileSpeedVector = ProjectileRigidbody.velocity;

            // Update the trail.
            setTrail();
        }
    }

    

    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        Vector3 movementThisStep = myRigidbody.position - previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed 
            if (Physics.Raycast(transform.position, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
            {
                if (!hitInfo.collider)
                    return;
                if (hitInfo.collider != myCollider)
                {
                    OnCollision(hitInfo);
                }
            }
        }
        previousPosition = myRigidbody.position;
    }

    public virtual void KillProjectile()
    {
        // Get rid of object and its trail.
        if (currentTrail)
        {
            currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
        }
        Destroy(gameObject);
        // Add the explosion and delete the object after 1 seconds.
        GameObject explosion = Instantiate(projectileExplosion, gameObject.transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, 1);
        // TODO: The audio will for exploding the object will play

        // Update the projectileCount of the parent tank.
        if (!transfered)
        {
            parentTank.GetComponent<Tank>().increaseProjCount();
        }
    }

    public virtual void RemoveProjectile()
    {
        // The same as killProjectile without audio or the explosion.
        if (currentTrail)
        {
            currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
        }
        GameObject.Destroy(this.gameObject);
    }


    public virtual void DisableProjectile()
    {
        // Makes projectiles fall at the completion of a room.
        ProjectileRigidbody.useGravity = true;
        disabled = true;

        // Get rid of projectile sender.
        sender.SetActive(false);

        // Add random force and torque to projectile.
        ProjectileRigidbody.velocity = new Vector3(Random.Range(-horProjExplVel, horProjExplVel), vertProjExplVel, Random.Range(-horProjExplVel, horProjExplVel));
        ProjectileRigidbody.AddTorque(new Vector3(Random.Range(-horProjExplTor, horProjExplTor), Random.Range(-horProjExplTor, horProjExplTor), Random.Range(-horProjExplTor, horProjExplTor)));

        // Wait and destroy the projectile after a random amount of time between 4 and 5 seconds.
        // Or set to detonate on next collision.
        StartCoroutine(DestroyAfter(maxDestroyTime));
        setToDetonate();
    }
    
    // Obsolete: use setToDetonate(). Destroys the projectile object after time time.
    IEnumerator DestroyAfter(float time)
    {
        if (currentTrail)
        {
            currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
        }
        yield return new WaitForSeconds(time);
        GameObject.Destroy(this.gameObject);
        KillProjectile();
    }

    // Helper function to see which way the projectile is moving.
    protected void PrintDirection()
    {
        Debug.Log("The projectile's vector is:");
        Debug.Log(projectileSpeedVector);
        Debug.Log("The projectile's forward is:");
        Debug.Log(gameObject.transform.forward);
    }
    
    // Helper function for DisableProjectile used by RoomManager.
    private void setToDetonate()
    {
        if (currentTrail)
        {
            currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
        }

        collisionCounter = maxCollisions;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            projectileCollision();
        }
        else if (collisionCounter >= maxCollisions & disabled)
        {
            lastCollision();
        }
    }
}