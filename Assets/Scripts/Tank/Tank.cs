using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class Tank : MonoBehaviour {

    public AudioSource m_MovementAudio;         // Reference to the audio source used to play the moving tank sounds.
    public AudioSource m_FireAudioSource;       // Reference to the audio source used to play miscellaneous sounds.
    public AudioClip m_FireAudio;               // Reference to the audio clip used to play the shooting audio source.
    public AudioClip m_EmptyFireAudio;          // Reference to the audio clip used to play the shooting audio source.
    public AudioClip m_IdleAudio;               // Reference to the audio clip used to play the idle tank audio source.
    protected int projectileAmount = 5;         // The maximum number of projectiles at a time.
    protected int projectileCount = 5;          // The current amount of projectiles available to fire. Starts at 5.
    public Material tankColor;                  // The material for the tank.
    public bool alive = true;
    public List<GameObject> targets;
    public List<GameObject> teammates;
    public bool testEnvironment = false;        // Bool to represent if the tank is in a test environment.
    public string teamName;
    public string enemyTeamName;
    protected BoxCollider hitbox;           // Reference to the box collider in the body of the tank.


    protected Vector3 velocity;                 // The velocity of the tank, kept track of for ai.
    protected float m_Speed = 6.5f;              // How fast the tank drives.
    protected float m_RotateSpeed = 6f;         // How fast the tank body rotates.
    protected Vector3 m_CurrentDirection;       // The current direction the tank points.
    protected Rigidbody m_RidgidbodyTank;       // Reference used to move the tank.
    protected Rigidbody m_RidgidbodyTower;      // Reference used to move the tank tower. 
    protected Rigidbody m_RidgidbodyBody;       // Reference used to move the tank body.
    public GameObject projectile;               // The GameObject of the projectile.
    public Transform tower;                     // The transform of the tower; the child of tank.
    public Transform body;                      // The transform of the body; the child of tank.
    //private float m_OriginalPitch;            // TODO: The pitch of the audio source at the start of the scene.
    protected Transform m_ProjectileSpawnPoint; // The point the projectile spawns relative to the tower.
    protected string projectileTag = "Projectile";// String to apply the tag on horizontal walls.
    public Transform projectileHolder;          // Transform for holding projectiles.


    protected Transform leftoverProjectileHolder;// The transform for holding leftover projectiles.
    public Material[] tankColors;               // Array of materials for each tank.



    private Object projCounterLock = new Object();
    

    protected void Awake()
    {
        m_RidgidbodyTank = GetComponent<Rigidbody>();

        // Load in the tank colors being used from the Resources folder in assets.
        // This needs to be called early so that it is instantiated before GUI_HUD.
        tankColors = Resources.LoadAll<Material>("Prefab/GameObjectPrefab/TankPrefab/TankColors");
        
        // Empty game object for holding projectiles. This needs to be in awake so that RoomManager can use it.
        projectileHolder = new GameObject("Projectile Holder").transform;
        projectileHolder.transform.SetParent(gameObject.transform);
        
        ColorizeTank();

        Start();
    }

    protected void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_RidgidbodyTank.isKinematic = false;

        // Also reset the input values.
        //TODO maybe
    }

    protected void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_RidgidbodyTank.isKinematic = true;
    }

    protected void Start()
    {
        // Get the children of tank; body and tower.
        body = this.gameObject.transform.GetChild(0);
        tower = this.gameObject.transform.GetChild(1);
        m_ProjectileSpawnPoint = tower.GetChild(3);

        // Get the reference to the hitbox.
        //hitbox = collider.GetComponent<CapsuleCollider>();
        hitbox = body.GetComponent<BoxCollider>();

        // Store the original pitch of the audio source.
        //m_OriginalPitch = m_MovementAudio.pitch;

        // Load in the Audio files.
        m_FireAudioSource = gameObject.GetComponents<AudioSource>()[0];
        //m_MovementAudio = gameObject.GetComponents<AudioSource>()[1];
        m_FireAudio = Resources.Load("FireSound") as AudioClip;
        m_EmptyFireAudio = Resources.Load("EmptyFireSound") as AudioClip;
    }

    private void ColorizeTank()
    {
        foreach (MeshRenderer mr in this.GetComponentsInChildren(typeof(MeshRenderer)))
        {
            mr.material = tankColor;
        }
    }

    // Send velocity so AI tanks can predict.
    public Vector3 SendVelocity()
    {
        return velocity;
    }

    protected void Fire()
    {
        // Create a projectile with position rotation and velocity.
        GameObject Shell = Instantiate(projectile) as GameObject;
        Shell.transform.position = m_ProjectileSpawnPoint.position;
        Shell.transform.rotation = tower.rotation;

        // Create tag for collisions.
        Shell.tag = projectileTag;

        // Call the fire audio.
        if (m_FireAudioSource.clip != m_FireAudio)
        {
            m_FireAudioSource.clip = m_FireAudio;
        }
        m_FireAudioSource.Play();

        // Set the parent of the projectile to the projectileHolder.
        Shell.transform.SetParent(projectileHolder.transform);
        Shell.GetComponent<Projectile>().parentTank = this.gameObject;

        decreaseProjCount();
    }

    // Called by Tank.
    public void decreaseProjCount()
    {
        lock (projCounterLock)
        {
            if (projectileCount >= 0)
            {
                projectileCount--;
            }
        }
    }

    // Called by Projectile.
    public void increaseProjCount()
    {
        lock (projCounterLock)
        {
            if (projectileCount < projectileAmount && alive)
            {
                projectileCount++;
            }
        }
    }

    protected void EmptyFire()
    {
        // Call the empty fire audio.
        if (m_FireAudioSource.clip != m_EmptyFireAudio)
        {
            m_FireAudioSource.clip = m_EmptyFireAudio;
        }
        m_FireAudioSource.Play();
    }

    public virtual void DestroyTank()
    {
        // Give projectiles to the room's projectileHolder.
        TransferProjectiles();

        // Set the gameObject in active.
        this.gameObject.SetActive(false);

        // Destroy tank.
        Destroy(this.gameObject);

        hitbox.enabled = false;
    }
    
    public void TransferProjectiles()
    {
        int projAmount = projectileHolder.GetComponentsInChildren<Projectile>().Length;
        for (int proj = 0; proj < projAmount; proj++)
        {
            projectileHolder.GetComponentInChildren<Projectile>().transfered = true;
            projectileHolder.GetComponentInChildren<Projectile>().transform.SetParent(leftoverProjectileHolder);
        }
    }

    public void SetLeftoverProjectileHolder(Transform holder)
    {
        leftoverProjectileHolder = holder;
    }

    // Sets the tower to a random compass direction.
    public void InitializeRandomAim()
    {
        List<Quaternion> possibleStartAims = new List<Quaternion>()
        {
            Quaternion.LookRotation(Vector3.forward),
            Quaternion.LookRotation(Vector3.left),
            Quaternion.LookRotation(Vector3.back),
            Quaternion.LookRotation(Vector3.right)
        };
        tower.rotation = possibleStartAims[Random.Range(0, possibleStartAims.Count)];
    }

    // Sets the tower to a compass direction closest towards the input vector.
    public void InitializeAim(Vector3 toEnemy)
    {
        if (Mathf.Abs(toEnemy.x) > Mathf.Abs(toEnemy.z))
        {
            toEnemy.z = 0;
        }
        else
        {
            toEnemy.x = 0;
        }
        tower.rotation = Quaternion.LookRotation(toEnemy);
    }
}