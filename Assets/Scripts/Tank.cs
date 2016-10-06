using UnityEngine;
using System.Collections;

abstract public class Tank : MonoBehaviour {

    public AudioSource m_MovementAudio;         // Reference to the audio source used to play the moving tank sounds.
    public AudioSource m_FireAudioSource;       // Reference to the audio source used to play miscellaneous sounds.
    public AudioClip m_FireAudio;               // Reference to the audio clip used to play the shooting audio source.
    public AudioClip m_EmptyFireAudio;          // Reference to the audio clip used to play the shooting audio source.
    public AudioClip m_IdleAudio;               // Reference to the audio clip used to play the idle tank audio source.
    public int projectileAmount = 5;            // The maximum number of projectiles at a time.
    public int projectileCount = 0;

    protected float m_Speed = 12f;              // How fast the tank drives.
    protected float m_RotateSpeed = 6f;         // How fast the tank rotates.
    protected Vector3 m_CurrentDirection;       // The current direction the tank points.
    protected Rigidbody m_RidgidbodyTank;       // Reference used to move the tank.
    protected Rigidbody m_RigidbodyTower;       // Reference used to move the tank tower. 
    protected Rigidbody m_RidgidbodyBody;       // Reference used to move the tank body.
    protected GameObject projectile;            // The GameObject of the projectile.
    protected Transform tower;                  // The transform of the tower; the child of tank.
    public Transform body;                      // The transform of the tower; the child of tank.
    //private float m_OriginalPitch;            // TODO: The pitch of the audio source at the start of the scene.
    protected Transform m_ProjectileSpawnPoint; // The point the projectile spawns relative to the tower.
    protected string projectileTag = "Projectile";// String to apply the tag on horizontal walls.
    public Transform projectileHolder;          // GameObject for holding projectiles.


    protected Transform leftoverProjectileHolder;
    public Material[] tankColors;



    protected void Awake()
    {
        m_RidgidbodyTank = GetComponent<Rigidbody>();

        // Load in the tank colors being used from the Resources folder in assets.
        // This needs to be called early so that it is instantiated before GUI_HUD.
        tankColors = Resources.LoadAll<Material>("TankColors");
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

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("Shell") as GameObject;
        
        // Store the original pitch of the audio source.
        //m_OriginalPitch = m_MovementAudio.pitch;

        // Load in the Audio files.
        m_FireAudioSource = gameObject.GetComponents<AudioSource>()[0];
        //m_MovementAudio = gameObject.GetComponents<AudioSource>()[1];
        m_FireAudio = Resources.Load("FireSound") as AudioClip;
        m_EmptyFireAudio = Resources.Load("EmptyFireSound") as AudioClip;

        // Empty game object for holding projectiles.
        projectileHolder = new GameObject("Projectile Holder").transform;
        projectileHolder.transform.SetParent(gameObject.transform);
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

        projectileCount++;
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
        Destroy(gameObject);
    }
    
    public void TransferProjectiles()
    {
        int projAmount = projectileHolder.GetComponentsInChildren<Projectile>().Length;
        for (int proj = 0; proj < projAmount; proj++)
        {
            //Projectile currentProj = projectileHolder.GetComponentsInChildren<Projectile>()[proj];
            //currentProj.transform.SetParent(leftoverProjectileHolder);
            projectileHolder.GetComponentInChildren<Projectile>().transform.SetParent(leftoverProjectileHolder);
        }
    }

    public void SetLeftoverProjectileHolder(Transform holder)
    {
        leftoverProjectileHolder = holder;
    }
}
