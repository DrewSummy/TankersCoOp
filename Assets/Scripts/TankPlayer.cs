using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists.

public class TankPlayer : Tank {

    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.

    private Vector3 m_TargetDirection;          // The direction the tank points toward for driving.
    private float m_DriveVerticalValue;         // The vertical component of the drive direction
    private float m_DriveHorizontalValue;       // The horizontal component of the drive direction
    private string m_DriveVerticalName;         // The name of the input axis for moving forward and back.
    private string m_DriveHorizontalName;       // The name of the input axis for turning.    
    private string m_AimHorizontalName;         // The name of the vector for aiming.
    private string m_AimVerticalName;           // The name of the vector for aiming.
    private float m_AimHorizontalValue;         // The value of the vector for aiming.
    private float m_AimVerticalValue;           // The value of the vector for aiming.
    private string m_FireName;                  // The name of the float for shooting.
    private float m_FireValue;                  // The value of the float for the trigger.
    private string m_PauseName;                 // The name of the bool for pausing.
    private bool m_PauseValue;                  // The value of the bool for the pause.
    private Vector3 m_AimRotation;              // The target direction for the tower to point.
    private bool m_HasShot;                     // The boolean used to permit the tank to shoot once per trigger pull.
    private bool m_HasPaused;                   // The boolean used to permit the player from pausing repeatedly.
    private bool paused;
    private float joystickMagnitude1;           // The magnitude of the joystick for moving.

    private Vector3 velocity;                   // The velocity of the tank, kept track of for ai.
    public int killAmount = 0;                  // The number of kills by this tank, kept track of for GUI_Pause.
    public int[] killCounter;
    public bool disabled = true;

    //TODO: figure out how to make moving go in both directions

    protected new void OnEnable()
    {
        base.OnEnable();

        // Also reset the input values.
        m_AimHorizontalValue = 0f;
        m_AimVerticalValue = 0f;
        m_DriveHorizontalValue = 0f;
        m_DriveVerticalValue = 0f;
        m_FireValue = 0f;
        m_PauseValue = false;

        

        // Also reset the input values.
        //TODO maybe
    }

    private new void Start()
    {
        base.Start();

        // The axes names are based on player number.
        m_AimHorizontalName = "AimHorizontal" + m_PlayerNumber;
        m_AimVerticalName = "AimVertical" + m_PlayerNumber;
        m_DriveHorizontalName = "DriveHorizontal" + m_PlayerNumber;
        m_DriveVerticalName = "DriveVertical" + m_PlayerNumber;
        m_FireName = "Fire" + m_PlayerNumber;
        m_PauseName = "Pause" + m_PlayerNumber;

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("Shell") as GameObject;

        // Tanks hasn't shot yet. This is used to allow semi-auto shooting.
        m_HasShot = false;

        m_HasPaused = false;
        paused = false;

        // Make the size of the killCounter the amount of colors.
        killCounter = new int[tankColors.Length];

        // Store the original pitch of the audio source.
        //m_OriginalPitch = m_MovementAudio.pitch;

        // Load in the Audio files.
        m_FireAudioSource = gameObject.GetComponents<AudioSource>()[0];
        //m_MovementAudio = gameObject.GetComponents<AudioSource>()[1];
        m_FireAudio = Resources.Load("FireSound") as AudioClip;
        m_EmptyFireAudio = Resources.Load("EmptyFireSound") as AudioClip;
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
        
        // Store the value for pauseing.
        m_PauseValue = Input.GetButtonDown(m_PauseName);

    }

    private void FixedUpdate()
    {
        // Adjust the rigidbody's position and orientation in FixedUpdate.
        if (!disabled)
        {
            Move();
            Turn();
        }

        // Adjust the rotation of tower in FixedUpdate.
        Aim();

        // Shoot bullets from the tower in FixedUpdate if m_FireValue exceeds .9.
        if (!disabled)
        {
            Shoot();
        }
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
        else if (175 < Vector3.Angle(trueAngle, m_TargetDirection))
        {
            speed = -m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);

        // Update the velocity.
        velocity = -body.forward * speed;
    }
    

    private void Turn()
    {
        // moves very slightly and the body doesn't follow. This would cause the speed to be 0 from calculate speed.
        // Keep track of the direction the joystick is pointed toward.
        Vector3 m_TargetDirection = new Vector3(-m_DriveHorizontalValue, 0, m_DriveVerticalValue);
        
        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(body.forward, m_TargetDirection, step, .01F);
        
        if (newDir == Vector3.zero)
        {
            // Don't rotate at all
        }
        // Turn forward or backward depending on which is closer.
        else if (Vector3.Angle(body.forward, m_TargetDirection) < 90)
        {
            // Rotate towards forwards.
            body.rotation = Quaternion.LookRotation(newDir);
        }
        else
        {
            // Rotate towards backwards.
            newDir = Vector3.RotateTowards(body.forward, -m_TargetDirection, step, .01F);
            body.rotation = Quaternion.LookRotation(newDir);
        }
    }


    private void Aim()
    {

        // Keep track of the m_AimRotation for aiming.
        Vector3 m_AimRotation = new Vector3(-m_AimVerticalValue, 0, m_AimHorizontalValue);
        tower.LookAt(tower.position + m_AimRotation);
    }


    protected new void Fire()
    {
        base.Fire();

    }


    private void Shoot()
    {
        if (m_FireValue == 1)
        {
            if (m_HasShot == false)
            {
                // If there are less than the max projectile amount, call Fire.
                if (projectileCount < projectileAmount)
                {
                    Fire();

                    GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateP1Projectiles();

                    // Tank shot.
                    m_HasShot = true;
                }
                else
                {
                    EmptyFire();

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

    public Vector3 SendVelocity()
    {
        //TODO: figure out why this is always 0
        return velocity;
    }

    public Vector3 SendPosition()
    {
        return this.transform.position;
    }

    public void updateKillTracker(Material material)
    {
        // Find which material is being updated from enemyTankColors and increment the respective killTracker.
        for (int m = 0; m < tankColors.Length; m++)
        {
            if (tankColors[m] == material)
            {
                killCounter[m]++;
            }
        }
    }
}
