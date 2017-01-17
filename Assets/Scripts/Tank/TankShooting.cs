using System;
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    private string m_AimHorizontalName;         // The name of the vector for aiming.
    private string m_AimVerticalName;           // The name of the vector for aiming.
    private float m_AimHorizontalValue;         // The value of the vector for aiming.
    private float m_AimVerticalValue;           // The value of the vector for aiming.
    private string m_FireName;                  // The name of the vector for shooting.
    private float m_FireValue;                  // The value of the vector for the trigger.
    public float m_RotateSpeed;                 // The speed the tower rotates at.
    public float m_ShellSpeed;                  // The speed the shell fires at.
    private Vector3 m_AimRotation;              // The target direction for the tower to point.
    private bool m_HasShot;                     // The boolean used to permit the tank to shoot once per trigger pull.

    public int m_MaxNumberOfShells = 5;         // The max number of shells the tank can hold at a time.
    private int m_NumberOfShells = 5;           // The current number of shells the tank holds.

    public GameObject projectile;               // The GameObject of the projectile.
    private Transform tower;                    // The transform of the tower; the child of tank.

    private Rigidbody m_Rigidbody;              // Reference used to move the tank tower. 
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    private Vector3 m_TankPosition;             // The position of the tank.

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_AimHorizontalValue = 0f;
        m_AimVerticalValue = 0f;
        m_FireValue = 0f;
    }


    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        // The axes names are based on player number.
        m_AimHorizontalName = "AimHorizontal" + m_PlayerNumber;
        m_AimVerticalName = "AimVertical" + m_PlayerNumber;
        m_FireName = "Fire" + m_PlayerNumber;

        // The speed the tower rotates at.
        m_RotateSpeed = 500;

        // The speed the shell moves at.
        m_ShellSpeed = 50;

        // Get the child of tank; tower.
        tower = this.gameObject.transform.GetChild(1);

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("Shell") as GameObject;

        // Tanks hasn't shot yet.
        m_HasShot = false;
        }


    private void Update()
    {
        // Store the value of the input axes.
        m_AimHorizontalValue = Input.GetAxis(m_AimHorizontalName);
        m_AimVerticalValue = Input.GetAxis(m_AimVerticalName);
        m_FireValue = Input.GetAxis(m_FireName);
    }


    private void FixedUpdate()
    {
        // Keep track of the m_AimRotation for aiming and shooting.
        //Vector3 m_AimRotation = new Vector3(-m_AimVerticalValue, 0, m_AimHorizontalValue);

        // Adjust the rotation of tower in FixedUpdate.
        Aim();

        // Shoot bullets from the tower in FixedUpdate if m_FireValue exceeds .9.
        Shoot();
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
                // Keep track of the m_AimRotation for the position of the projectile.
                Vector3 m_AimRotation = new Vector3(-m_AimVerticalValue, 0, m_AimHorizontalValue);
                Debug.Log(m_AimRotation);

                // Create a projectile with position rotation and velocity.
                GameObject Shell = Instantiate(projectile) as GameObject;
                Shell.transform.position = tower.position; // + new Vector3(tower.rotation.x, tower.rotation.y, 0 * tower.rotation.z);
                Shell.transform.rotation = tower.rotation;
                Rigidbody rb = Shell.GetComponent<Rigidbody>();
                rb.velocity = -m_AimRotation * m_ShellSpeed;

                // Tank shot.
                m_HasShot = true;
            }
        }
        // Input.GetAxisRaw("Fire1") != 1
        else
        {
            m_HasShot = false;
        }



        //TODO: https://www.youtube.com/watch?v=J9ErQDWR44k
    }
}