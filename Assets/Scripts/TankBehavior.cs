using UnityEngine;
public class TankBehavior : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    public AudioSource m_MovementAudio;         // Reference to the audio source used to play the moving tank sounds.
    public AudioSource m_FireAudioSource;       // Reference to the audio source used to play miscellaneous sounds.
    public AudioClip m_FireAudio;               // Reference to the audio clip used to play the shooting audio source.
    public AudioClip m_EmptyFireAudio;          // Reference to the audio clip used to play the shooting audio source.
    public AudioClip m_IdleAudio;               // Reference to the audio clip used to play the idle tank audio source.
    public int projectileAmount = 5;            // The maximum number of projectiles at a time.

    private float m_Speed = 12f;                // How fast the tank drives.
    private float m_RotateSpeed = 6f;           // How fast the tank rotates.
    private Vector3 m_CurrentDirection;         // The current direction the tank points.
    private Vector3 m_TargetDirection;          // The direction the tank points toward for driving.
    private float m_DriveVerticalValue;         // The vertical component of the drive direction
    private float m_DriveHorizontalValue;       // The horizontal component of the drive direction
    private string m_DriveVerticalName;         // The name of the input axis for moving forward and back.
    private string m_DriveHorizontalName;       // The name of the input axis for turning.    
    private string m_AimHorizontalName;         // The name of the vector for aiming.
    private string m_AimVerticalName;           // The name of the vector for aiming.
    private float m_AimHorizontalValue;         // The value of the vector for aiming.
    private float m_AimVerticalValue;           // The value of the vector for aiming.
    private string m_FireName;                  // The name of the vector for shooting.
    private float m_FireValue;                  // The value of the vector for the trigger.
    private Vector3 m_AimRotation;              // The target direction for the tower to point.
    private bool m_HasShot;                     // The boolean used to permit the tank to shoot once per trigger pull.
    private float joystickMagnitude1;           // The magnitude of the joystick for moving. 

    private Rigidbody m_RidgidbodyTank;         // Reference used to move the tank.
    private Rigidbody m_RigidbodyTower;         // Reference used to move the tank tower. 
    private Rigidbody m_RidgidbodyBody;         // Reference used to move the tank body.
    public GameObject projectile;               // The GameObject of the projectile.
    private Transform tower;                    // The transform of the tower; the child of tank.
    private Transform body;                     // The transform of the tower; the child of tank
    //private float m_OriginalPitch;            // TODO: The pitch of the audio source at the start of the scene.
    private Transform m_ProjectileSpawnPoint;   // The point the projectile spawns relative to the tower.
    private string projectileTag = "Projectile";// String to apply the tag on horizontal walls.
    GameObject projectileHolder;                // GameObject for holding projectiles.


    private void Awake()
    {
        m_RidgidbodyTank = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_RidgidbodyTank.isKinematic = false;

        // Also reset the input values.
        m_AimHorizontalValue = 0f;
        m_AimVerticalValue = 0f;
        m_DriveHorizontalValue = 0f;
        m_DriveVerticalValue = 0f;
        m_FireValue = 0f;


        // Also reset the input values.
        //TODO maybe
    }

    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_RidgidbodyTank.isKinematic = true;
    }

    private void Start()
    {
        // The axes names are based on player number.
        m_AimHorizontalName = "AimHorizontal" + m_PlayerNumber;
        m_AimVerticalName = "AimVertical" + m_PlayerNumber;
        m_DriveHorizontalName = "DriveHorizontal" + m_PlayerNumber;
        m_DriveVerticalName = "DriveVertical" + m_PlayerNumber;
        m_FireName = "Fire" + m_PlayerNumber;

        // Get the children of tank; body and tower.
        body = this.gameObject.transform.GetChild(0);
        tower = this.gameObject.transform.GetChild(1);
        m_ProjectileSpawnPoint = tower.GetChild(3);

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("Shell") as GameObject;

        // Tanks hasn't shot yet. This is used to allow semi-auto shooting.
        m_HasShot = false;

        // Store the original pitch of the audio source.
        //m_OriginalPitch = m_MovementAudio.pitch;

        // Load in the Audio files.
        m_FireAudioSource = gameObject.GetComponents<AudioSource>()[0];
        //m_MovementAudio = gameObject.GetComponents<AudioSource>()[1];
        m_FireAudio = Resources.Load("FireSound") as AudioClip;
        m_EmptyFireAudio = Resources.Load("EmptyFireSound") as AudioClip;

        // Empty game object for holding projectiles.
        projectileHolder = new GameObject();
        projectileHolder.name = "Projectile Holder";
        projectileHolder.transform.SetParent(gameObject.transform);
    }

    private void Update()
    {
        TakeControllerInputs();

        // Give audio to the movement of the car.
        //EngineAudio();
    }

    private void TakeControllerInputs()
    {
        // Store the value of the input axes while exculding deadzones.
        // Get magnitude of joysticks inputs.
        joystickMagnitude1 = Mathf.Pow(Input.GetAxis(m_DriveVerticalName), 2) + Mathf.Pow(Input.GetAxis(m_DriveHorizontalName), 2);
        float joystickMagnitude2 = Mathf.Pow(Input.GetAxis(m_AimVerticalName), 2) + Mathf.Pow(Input.GetAxis(m_AimHorizontalName), 2);

        // Create a deadzone so that small values are discarded.
        // Driving.
        if (joystickMagnitude1 < .1)
        {
            // This is in the deadzone.
            m_DriveHorizontalValue = 0;
            m_DriveVerticalValue = 0;
        }
        else
        {
            // Get the horizontal and vertical components.
            m_DriveHorizontalValue = Input.GetAxis(m_DriveHorizontalName);
            m_DriveVerticalValue = Input.GetAxis(m_DriveVerticalName);
        }
        // Aiming.
        if (joystickMagnitude2 < .1)
        {
            // This is in the deadzone.
            m_AimHorizontalValue = 0;
            m_AimVerticalValue = 0;
        }
        else
        {
            // Get the horizontal and vertical components.
            m_AimHorizontalValue = Input.GetAxis(m_AimHorizontalName);
            m_AimVerticalValue = Input.GetAxis(m_AimVerticalName);
        }

        // Store the value for firing.
        m_FireValue = Input.GetAxis(m_FireName);
    }


    private void FixedUpdate()
    {
        // Adjust the rigidbody's position and orientation in FixedUpdate.
        Move();
        Turn();

        // Adjust the rotation of tower in FixedUpdate.
        Aim();

        // Shoot bullets from the tower in FixedUpdate if m_FireValue exceeds .9.
        Shoot();
    }


    private void Move()
    {
        // Keep track of the direction the joystick is pointed toward.
        Vector3 m_TargetDirection = new Vector3(-m_DriveHorizontalValue, 0, m_DriveVerticalValue);

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (Vector3.Angle(trueAngle, m_TargetDirection) < 5)
        {
            speed = m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }


    private void Turn()
    {
        // moves very slightly and the body doesn't follow. This would cause the speed to be 0 from calculate speed.
        // Keep track of the direction the joystick is pointed toward.
        Vector3 m_TargetDirection = new Vector3(-m_DriveHorizontalValue, 0, m_DriveVerticalValue);

        // Get the input values to determine the direction for the tank to turn to.
        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(body.forward, m_TargetDirection, step, .01F);
        body.rotation = Quaternion.LookRotation(newDir);
    }

    private void Aim()
    {

        // Keep track of the m_AimRotation for aiming.
        Vector3 m_AimRotation = new Vector3(-m_AimVerticalValue, 0, m_AimHorizontalValue);
        tower.LookAt(tower.position + m_AimRotation);
    }


    private void Shoot()
    {
        if (Input.GetAxisRaw("Fire1") == 1)
        {
            if (m_HasShot == false)
            {
                Debug.Log(projectileHolder.transform.childCount);
                if (projectileHolder.transform.childCount < projectileAmount)
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

                    // Tank shot.
                    m_HasShot = true;

                    // Set the parent of the projectile to the projectileHolder.
                    Shell.transform.SetParent(projectileHolder.transform);
                }
                else
                {
                    // Call the empty fire audio.
                    if (m_FireAudioSource.clip != m_EmptyFireAudio)
                    {
                        m_FireAudioSource.clip = m_EmptyFireAudio;
                    }
                    m_FireAudioSource.Play();

                    // Tank shot.
                    m_HasShot = true;
                }
            }
        }
        // Input.GetAxisRaw("Fire1") != 1
        else
        {
            m_HasShot = false;
        }



        //TODO: https://www.youtube.com/watch?v=J9ErQDWR44k
    }


    private void PlayFireAudio()
    {
        // A shot has been fired, play the audio.
        m_FireAudioSource.clip = m_FireAudio;
        m_FireAudioSource.Play();
    }

    private void EmptyFireAudio()
    {

    }
    

    private void EngineAudio()
    //TODO

    {
        // If there is a small input for driving, play the idle audio clip.
        if (Mathf.Abs(joystickMagnitude1) < 0.1f)
        {
            m_MovementAudio.clip = m_IdleAudio;
            if (m_MovementAudio.clip == m_IdleAudio)
            {
                // TODO: Figure out another audio to play different from the driving one.
                //m_MovementAudio.Play();
            }
        }
        else
        {
            m_MovementAudio.clip = m_IdleAudio;
            // Otherwise play this soon to be different audio clip.
            m_MovementAudio.Play();

            //TODO: may need another if else case inside both these cases to switch m_MovementAudio.clip
        }
    }
}