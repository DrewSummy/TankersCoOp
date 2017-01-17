using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    private float m_Speed = 9f;                // How fast the tank drives.
    private float m_RotateSpeed = 8f;           // How fast the tank rotates.
    public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip m_idle;                    // Audio to play when the tank isn't moving.
    public AudioClip m_drive;                   // Audio to play when the tank is moving.
    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

    public Vector3 m_CurrentDirection;          // The current direction the tank points.
    public Vector3 m_TargetDirection;           // The direction the tank points toward for driving.
    private float m_VerticalDirectionValue;     // The vertical component of the drive direction
    private float m_HorizontalDirectionValue;   // The horizontal component of the drive direction
    private string m_VerticalDirectionName;     // The name of the input axis for moving forward and back.
    private string m_HorizontalDirectionName;   // The name of the input axis for turning.    

    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    // TODO fix engine sound then use original pitch
    //private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        //TODO maybe
    }


    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        // The names are based on the player number.
        m_HorizontalDirectionName = "Horizontal" + m_PlayerNumber;
        m_VerticalDirectionName = "Vertical" + m_PlayerNumber;

        // Store the original pitch of the audio source.
        //m_OriginalPitch = m_MovementAudio.pitch;


    }


    private void Update()
    {
        // Get magnitude of joystick inputs.
        float joystickMagnitude = Mathf.Pow(Input.GetAxis(m_VerticalDirectionName), 2) + Mathf.Pow(Input.GetAxis(m_HorizontalDirectionName), 2);
        // Create a deadzone so that small values are discarded.
        if ( joystickMagnitude < .1 )
        {
            // This is in the deadzone.
            m_HorizontalDirectionValue = 0;
            m_VerticalDirectionValue = 0;
        }
        else
        {
            // Get the horizontal and vertical components.
            m_HorizontalDirectionValue = Input.GetAxis(m_HorizontalDirectionName);
            m_VerticalDirectionValue = Input.GetAxis(m_VerticalDirectionName);
        }


        EngineAudio();
    }


    private void EngineAudio()
        //TODO
        
    {
        /*
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (m_MovementAudio.clip == m_drive)
            {
                // ... change the clip to idling and play it.
                m_MovementAudio.clip = m_idle;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (m_MovementAudio.clip == m_idle)
            {
                // ... change the clip to driving and play.
                m_MovementAudio.clip = m_drive;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        */
    }


    private void FixedUpdate()
    {

        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Turn();
        Move();

    
    }


    private void Move()
    {
        // Calculate the speed with CalculateSpeed()
        float speed = CalculateSpeed();

        // Put it together to get the movement vector.
        Vector3 movement = transform.forward * speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private float CalculateSpeed()
    {
        //Speed depends on 3 things: 
        //m_Speed
        //how hard the joystick is being pressed
        //and the angle between the joystick and the tank

        // Keep track of the direction the joystick is pointed toward.
        Vector3 m_TargetDirection = new Vector3(-m_HorizontalDirectionValue, 0, m_VerticalDirectionValue);

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -transform.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));
        float angleTargetToDirection = Mathf.Abs(Vector3.Angle(trueAngle, m_TargetDirection));
        
        // The tank's speed is 0 until the tank is directed to where the joystick faces (m_TargetDirection = m_CurrentDirection).
        // Otherwise the speed depends on how hard the joystick is being pressed and m_Speed.
        if (Mathf.Abs(angleTargetToDirection) != 0)
        {
            // Return 0 to make tank's speed 0.
            return 0;
        }
        else
        {
            return m_Speed * (Mathf.Abs(m_HorizontalDirectionValue) + Mathf.Abs(m_VerticalDirectionValue));
        }
        
    }


    private void Turn()
    {
        // Keep track of the direction the joystick is pointed toward.
        Vector3 m_TargetDirection = new Vector3(-m_HorizontalDirectionValue, 0, m_VerticalDirectionValue);

        // Get the input values to determine the direction for the tank to turn to.
        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, m_TargetDirection, step, 0.001F);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}