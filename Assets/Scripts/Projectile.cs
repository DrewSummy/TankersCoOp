using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    //TODO: figure out how to assign dynamically

    public GameObject projectileExplosion;         // The GameObject of the projectile's explosion.
    public AudioSource audioSource;                // Reference to the audio source.
    public AudioClip ricochetAudio;                // The GameObject of the audio for ricochetting.

    protected int maxCollisions = 1;               // The max number of collisions before the object is destroyed. 
    private int collisionCounter;                  // Used to keep track of the number of collisions.
    private float projectileSpeed = 16;            // The speed the projectile fires at.
    private Vector3 projectileSpeedVector;         // The vector the projectile moves at.
    private Vector3 normalCollision;               // The vector normal to the collision.
    private Vector3 newProjectileSpeedVector;      // The vector the projectile reflects at.
    private Rigidbody ProjectileRigidbody;         // Reference used to move the projectile.
    public GameObject parentTank;
    private Tank tankScript;
    private bool disabled = false;

    
    // Use this for initialization
    protected void Start()
    {
        // Each spawned projectile has collided with 0 objects.
        collisionCounter = 0;
        //maxCollisions = 1;

        // The speed the projectile moves at.
        projectileSpeed = 16;

        // Use the rigidbody and calculate the projectileSpeedVector.
        ProjectileRigidbody = gameObject.GetComponent<Rigidbody>();

        // Get rid of gravity.
        ProjectileRigidbody.useGravity = false;

        // Add velocity to the projectile.
        ProjectileRigidbody.velocity = -ProjectileRigidbody.transform.forward * projectileSpeed;
        projectileSpeedVector = ProjectileRigidbody.velocity;

        // Load in the explosion being used from the Resources folder in assets.
        projectileExplosion = Resources.Load("Effect_02") as GameObject;

        // Get the AudioSource component of the game object.
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        // Load in the sound being used from the Resources folder in assets.
        ricochetAudio = Resources.Load("RicochetSound") as AudioClip;
        audioSource.clip = ricochetAudio;

        // Get the tank component from the parent game object.
        //TODO: how do i do this?
        //GameObject parentTank = GameObject.FindGameObjectWithTag("Player");
        //parentTank.GetComponent<TankBehavior>.SubtractProjectile();
    }
    
    
    protected void OnCollisionEnter(Collision collisionInfo)
    {
        // The object has collided with another projectile.
        if (collisionInfo.transform.tag == "Projectile")
        {
            if (!disabled)
            {
                KillProjectile();
            }
        }
        // The object has collided with a player tank.
        else if (collisionInfo.transform.tag == "Player" || collisionInfo.transform.tag == "Enemy")
        {
            if (!disabled)
            {
                tankScript = collisionInfo.gameObject.GetComponent<Tank>();
                tankScript.DestroyTank();
                KillProjectile();                
            }
        }
        // The object has collided the max amount of times.
        else if (collisionCounter >= maxCollisions)
        {
            if (!disabled)
            {
                KillProjectile();
            }
        }
        // The object has more collisions and needs to rotate and continue.
        else
        {
            // A collsion has occured.
            ++collisionCounter;

            if (!disabled)
            {
                // Keep the object from spinning on collision.
                if (ProjectileRigidbody) ProjectileRigidbody.freezeRotation = true;


                // Calculate the new vector follow the equation V' = 2(V.N)*N-V.
                normalCollision = collisionInfo.contacts[0].normal;

                newProjectileSpeedVector =
                    Vector3.Reflect(projectileSpeedVector, normalCollision);

                if (ProjectileRigidbody) ProjectileRigidbody.transform.forward = -newProjectileSpeedVector;
                if (ProjectileRigidbody) ProjectileRigidbody.velocity = newProjectileSpeedVector;
                if (ProjectileRigidbody) projectileSpeedVector = ProjectileRigidbody.velocity;
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
    }

    public virtual void RemoveProjectile()
    {
        // The same as killProjectile without audio or the explosion.
        GameObject.Destroy(this.gameObject);
    }


    public virtual void DisableProjectile()
    {
        // Makes projectiles fall at the completion of a room.
        ProjectileRigidbody.useGravity = true;
        disabled = true;

        // Wait and destroy the projectile after a random amount of time between 1 and 5 seconds.
        StartCoroutine(DestroyAfter(Random.Range(1f, 5f)));
    }

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject.Destroy(this.gameObject);
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
}
