using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    //TODO: add smoke trails
    //TODO: add a gameobject infront of the tower so that if a wall is inside it, the tank won't shoot

    public GameObject projectileExplosion;         // The GameObject of the projectile's explosion.
    public AudioSource audioSource;                // Reference to the audio source.
    public AudioClip ricochetAudio;                // The GameObject of the audio for ricochetting.
    public GameObject smokeTrail;

    protected int maxCollisions = 1;               // The max number of collisions before the object is destroyed. 
    private int collisionCounter;                  // Used to keep track of the number of collisions.
    private float projectileSpeed = 16;            // The speed the projectile fires at.
    private Vector3 projectileSpeedVector;         // The vector the projectile moves at.
    private Vector3 normalCollision;               // The vector normal to the collision.
    private Vector3 newProjectileSpeedVector;      // The vector the projectile reflects at.
    private Rigidbody ProjectileRigidbody;         // Reference used to move the projectile.
    public GameObject parentTank;                  // Reference to the parent tank game object.
    private Tank tankScript;                       // Reference to Tank to be able to call DestroyTank().
    private bool disabled = false;                 // Boolean for whether the projectile is disabled.
    private GameObject currentTrail;
    public bool transfered = false;

    //TODO: make the bullet rotate around the center of the sphere collider, maybe make an empty parent gameObject as the parent


    protected void Awake()
    {
        // Use the rigidbody and calculate the projectileSpeedVector.
        ProjectileRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Use this for initialization
    protected void Start()
    {
        // Each spawned projectile has collided with 0 objects.
        collisionCounter = 0;

        // The speed the projectile moves at.
        projectileSpeed = 16;

        // Use the rigidbody and calculate the projectileSpeedVector.
        ProjectileRigidbody = gameObject.GetComponent<Rigidbody>();

        // Get rid of gravity.
        ProjectileRigidbody.useGravity = false;

        // Add velocity to the projectile.
        ProjectileRigidbody.velocity = -ProjectileRigidbody.transform.forward * projectileSpeed;
        projectileSpeedVector = ProjectileRigidbody.velocity;

        // Update the trail.
        currentTrail = Instantiate(smokeTrail) as GameObject;
        currentTrail.transform.SetParent(transform);
        currentTrail.transform.position = transform.position;
        currentTrail.transform.rotation = transform.rotation;
        currentTrail.GetComponent<Rigidbody>().velocity = projectileSpeedVector;

        // Load in the explosion being used from the Resources folder in assets.
        projectileExplosion = Resources.Load("TankResources/Effect_02") as GameObject;

        // Get the AudioSource component of the game object.
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        // Load in the sound being used from the Resources folder in assets.
        ricochetAudio = Resources.Load("TankResources/RicochetSound") as AudioClip;
        audioSource.clip = ricochetAudio;
    }


    protected void OnCollisionEnter(Collision collisionInfo)
    {
        // The object has collided with another projectile.
        if (collisionInfo.transform.tag == "Projectile")
        {
            if (!disabled)
            {
                // Drop the current smoke trail.
                currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
                KillProjectile();
            }
        }
        // The object has collided with a player tank.
        else if (collisionInfo.transform.tag == "Player" || collisionInfo.transform.tag == "Enemy")
        {
            if (!disabled)
            {
                // Drop the current smoke trail.
                tankScript = collisionInfo.gameObject.GetComponent<Tank>();
                tankScript.DestroyTank();
                currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
                KillProjectile();

            }
        }
        // The object has collided the max amount of times.
        else if (collisionCounter >= maxCollisions)
        {
            // Drop the current smoke trail.
            if (currentTrail) currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
            KillProjectile();
        }
        // The object has more collisions and needs to rotate and continue.
        else
        {
            // A collsion has occured.
            ++collisionCounter;

            if (!disabled)
            {
                // Drop the current smoke trail.
                //TODO: this is causing error when tanks die
                currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);

                //if (ProjectileRigidbody) ProjectileRigidbody.freezeRotation = true;
                ProjectileRigidbody.freezeRotation = true;


                // Calculate the new vector follow the equation V' = 2(V.N)*N-V.
                normalCollision = collisionInfo.contacts[0].normal;

                newProjectileSpeedVector =
                    Vector3.Reflect(projectileSpeedVector, normalCollision);

                ProjectileRigidbody.transform.forward = -newProjectileSpeedVector;
                ProjectileRigidbody.velocity = newProjectileSpeedVector;
                projectileSpeedVector = ProjectileRigidbody.velocity;




                // Update the trail.
                currentTrail = Instantiate(smokeTrail) as GameObject;
                currentTrail.transform.SetParent(transform);
                currentTrail.transform.position = transform.position;
                currentTrail.transform.rotation = transform.rotation;
                currentTrail.GetComponent<Rigidbody>().velocity = projectileSpeedVector;

            }

            // If the audioclip isn't the ricochet audio make it that.
            if (audioSource.clip != ricochetAudio)
            {
                audioSource.clip = ricochetAudio;
            }

            // Play the audio for the ricochet.
            audioSource.Play();
        }
    }
    

    public virtual void KillProjectile()
    {
        // The projectile has reached the max number of collisions.
        // Get rid of object.
        Destroy(gameObject);
        // Add the explosion and delete the object after 3 seconds.
        GameObject explosion = Instantiate(projectileExplosion, gameObject.transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, 1);
        // The audio will for exploding the object will play

        // Update the projectileCount of the parent tank.
        if (!transfered)
        {
            parentTank.GetComponent<Tank>().increaseProjCount();
        }
    }

    public virtual void RemoveProjectile()
    {
        // The same as killProjectile without audio or the explosion.
        currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
        GameObject.Destroy(this.gameObject);
    }


    public virtual void DisableProjectile()
    {
        //Debug.Log("disabling");
        // Makes projectiles fall at the completion of a room.
        ProjectileRigidbody.useGravity = true;
        disabled = true;

        // Add random velocity and rotation to projectile.
        //ProjectileRigidbody.velocity += new Vector3(Random.Range(1f, 5f), Random.Range(1f, 20f), Random.Range(1f, 5f));
        ProjectileRigidbody.velocity = new Vector3(Random.Range(1f, 5f), Random.Range(1f, 20f), Random.Range(1f, 5f));
        ProjectileRigidbody.rotation = Random.rotation;

        // Wait and destroy the projectile after a random amount of time between 4 and 5 seconds.
        // Or set to detonate on next collision.
        StartCoroutine(DestroyAfter(Random.Range(4f, 5f)));
        setToDetonate();
    }
    
    // Obsolete: use setToDetonate(). Destroys the projectile object after time time.
    IEnumerator DestroyAfter(float time)
    {
        currentTrail.GetComponent<SmokeTrailScript>().removeSmokeTrail(projectileSpeedVector);
        yield return new WaitForSeconds(time);
        //GameObject.Destroy(this.gameObject);
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

    public float SendProjectileVelocity()
    {
        return projectileSpeed;
    }

    // Helper function for DisableProjectile used by RoomManager.
    private void setToDetonate()
    {
        collisionCounter = maxCollisions;
    }
}