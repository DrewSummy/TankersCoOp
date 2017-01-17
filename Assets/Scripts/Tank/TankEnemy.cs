using UnityEngine;
using System.Collections;
using Completed; // does this need {}

public class TankEnemy : Tank
{
    public RoomManager parentRoom;

    private GameObject player1;                 // Reference to the player 1 game object.
    private float joystickMagnitude1;           // The magnitude of the joystick for moving. 
    private Vector3 vectorTowardPlayer1;        // The vector toward player 1.
    private bool hasBegun = false;              // Used for shooting.
    private int drivingRange = 30;              // Angle used for driving toward/away from a player.
    private TankPlayer player1Script;           // Store a reference to the TankPlayer of player 1.
    private TankPlayer player2Script;           // Store a reference to the TankPlayer of player 2.
    private ProjectileEnemy projectileScript;   // Store a reference to the projectile script.
    private Vector3 randomDirection;            // The random direction vector.
    private float distancePlayer1;              // The float distance to player 1.
    private Vector3 player1Pos;                 // The vector for the current position of player 1.
    private Vector3 player1Velocity;            // The vector for the current velocity of player 1.
    private Vector3 player1Hit;                 // The vector toward player1 from tankEnemy.
    private bool drivingForward;                // The bool for if the tank is driving forward.
    private float projectileSpeed;              // The speed of the projectile being shot.
    private float fireFreq = 3.0f;
    private bool canFire = true; //TODO: make it not be able to shoot for like a second





    // Use this for initialization
    protected new void Start()
    {
        base.Start();

        // Change the speed and rotate speed of the tank.
        m_Speed = 6f;
        m_RotateSpeed = 1f;

        // Get the script of player 1.
        player1 = GameObject.FindGameObjectWithTag("Player");
        player1Script = player1.GetComponent<TankPlayer>();

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("TankResources/ShellEnemy") as GameObject;

        // Get the script of the projectile and record its speed.
        projectileScript = projectile.GetComponent<ProjectileEnemy>();
        projectileSpeed = projectileScript.SendProjectileVelocity();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (player1Script.alive)
        {
            // Keep track of the vector towards player 1.
            vectorTowardPlayer1 = -(player1.transform.position - tower.position);
            vectorTowardPlayer1.y = 0;
            distancePlayer1 = Vector3.Distance(player1.transform.position, tower.position);

            player1Pos = player1.transform.position;

            // Keep track of player 1's veloctiy.
            player1Velocity = player1Script.SendVelocity();

            // Update the vector that hits player 1.
            player1Hit = distancePlayer1 * player1Velocity / projectileSpeed;
        }
    }

    protected void FixedUpdate()
    {
        if (!hasBegun)
        {
            //InvokeRepeating("Fire", 1, 1);
            //InvokeRepeating("ChangeRandomDirection", 0, 1);
            hasBegun = true;
        }
        //AimPredict();
        FirePredict();
    }

    protected new void Fire()
    {
        base.Fire();
    }

    protected void FireDirect()
    {
        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, vectorTowardPlayer1, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);

        if (Vector3.Normalize(newDir) == Vector3.Normalize(vectorTowardPlayer1))
        {
            if (canFire)
            {
                Fire();
                canFire = false;
                StartCoroutine(DelayFire());
            }
        }
    }

    protected void FirePredict()
    {

        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, player1Hit + vectorTowardPlayer1, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);
        

        if (Vector3.Normalize(newDir) == Vector3.Normalize(player1Hit + vectorTowardPlayer1))
        {
            if (canFire)
            {
                Fire();
                canFire = false;
                StartCoroutine(DelayFire());
            }
        }
    }

    IEnumerator DelayFire()
    {
        yield return new WaitForSeconds(fireFreq);
        canFire = true;
    }

    protected void AimDirect()
    {
        // AI to look at player.
        tower.LookAt(tower.position + vectorTowardPlayer1);
    }

    protected void AimPredict()
    {
        // AI to look infront of player.
        tower.LookAt(player1Hit + tower.position + vectorTowardPlayer1);
    }

    protected void RotateToward()
    {
        // Keep track of the direction toward the player.
        Vector3 m_TargetDirection = vectorTowardPlayer1;

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

    protected void DriveTowardStrict()
    {
        // Only drives toward at a less than 5 degree angle.

        RotateToward();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (Vector3.Angle(trueAngle, -vectorTowardPlayer1) < 5)
        {
            speed = m_Speed;
        }
        else if (175 < Vector3.Angle(trueAngle, -vectorTowardPlayer1))
        {
            speed = -m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    protected void DriveAwayStrict()
    {
        // Only drives away at a more than 175 degree angle.

        RotateToward();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (Vector3.Angle(trueAngle, -vectorTowardPlayer1) < 5)
        {
            speed = -m_Speed;
        }
        else if (175 < Vector3.Angle(trueAngle, -vectorTowardPlayer1))
        {
            speed = m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    protected void DriveTowardLoose()
    {
        // Drives toward at an angle between drivingRange.
        RotateToward();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (Vector3.Angle(trueAngle, -vectorTowardPlayer1) < drivingRange)
        {
            speed = m_Speed;
        }
        else if (180 - drivingRange < Vector3.Angle(trueAngle, -vectorTowardPlayer1))
        {
            speed = -m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    protected void DriveAwayLoose()
    {
        // Drives away at an angle between drivingRange.
        // Drives toward at an angle between drivingRange.
        RotateToward();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (Vector3.Angle(trueAngle, -vectorTowardPlayer1) < drivingRange)
        {
            speed = -m_Speed;
        }
        else if (180 - drivingRange < Vector3.Angle(trueAngle, -vectorTowardPlayer1))
        {
            speed = m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    protected void RotatePerpendicular()
    {
        // Keep track of the direction toward the player.
        Vector3 m_TargetDirection = Vector3.Cross(vectorTowardPlayer1, Vector3.up);

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

    protected void DriveCCWStrict()
    {
        // Only perpendicular counter clockwise.
        RotatePerpendicular();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (85 < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 95)
        {
            speed = -m_Speed;
        }
        else if (265 < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 275)
        {
            speed = -m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardPlayer1) < 1)
        {
            speed = -m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    protected void DriveCCWLoose()
    {
        // Only perpendicular counter clockwise.
        RotatePerpendicular();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (90 - drivingRange < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 90 + drivingRange)
        {
            speed = -m_Speed;
        }
        else if (270 - drivingRange < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 270 + drivingRange)
        {
            speed = -m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardPlayer1) < 1)
        {
            speed = -m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    protected void DriveCWStrict()
    {
        // Only perpendicular counter clockwise.
        RotatePerpendicular();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (85 < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 95)
        {
            speed = m_Speed;
        }
        else if (265 < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 275)
        {
            speed = m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardPlayer1) < 1)
        {
            speed = m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    protected void DriveCWLoose()
    {
        // Only perpendicular counter clockwise.
        RotatePerpendicular();

        // Keep track of the direction the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the direction the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the direction the joystick is pointing, the speed equals 0.
        float speed = 0;
        if (90 - drivingRange < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 90 + drivingRange)
        {
            speed = m_Speed;
        }
        else if (270 - drivingRange < CalculateAngle(trueAngle, vectorTowardPlayer1) && CalculateAngle(trueAngle, vectorTowardPlayer1) < 270 + drivingRange)
        {
            speed = m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardPlayer1) < 1)
        {
            speed = m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }

    private static float CalculateAngle(Vector3 from, Vector3 to)
    {
        // Helper function for calculating an angle between 0 and 360.

        float angle = Mathf.Acos((Vector3.Dot(from, to) / (from.magnitude + to.magnitude))) * 180 / Mathf.PI;
        return angle;
    }

    private void ChangeRandomDirection()
    {
        randomDirection = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
    }

    protected void RotateRandomDirection()
    {
        //TODO: only hovers around certain direction, fix this
        Vector3 m_TargetDirection = randomDirection;

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

            drivingForward = true;
        }
        else
        {
            // Rotate towards backwards.
            newDir = Vector3.RotateTowards(body.forward, -m_TargetDirection, step, .01F);
            body.rotation = Quaternion.LookRotation(newDir);

            drivingForward = false;
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (drivingForward)
        {
            drivingForward = false;
        }
        else
        {
            drivingForward = true;
        }
    }

    protected void DriveRandomDirection()
    {
        RotateRandomDirection();

        bool hasBeenCalled = false;
        if (!hasBeenCalled)
        {
            InvokeRepeating("ChangeRandomDirection", 0, 3);
            hasBeenCalled = true;
        }
    }

    private void DriveMeander()
    {
        //TODO: figure out how to cycle through actions
        bool hasBeenCalled = false;
        if (!hasBeenCalled)
        {
            InvokeRepeating("DriveRandomDirection", 0, 3);
            hasBeenCalled = true;
        }
    }

    protected void ColorizeTank()
    {
        foreach (MeshRenderer mr in this.GetComponentsInChildren(typeof(MeshRenderer)))
        {
            mr.material = tankColor;
        }
    }

    public override void DestroyTank()
    {
        base.DestroyTank();

        //GetComponent<Rigidbody>().AddExplosionForce(5, transform.position, 4, 3.0F);

        // Update the enemy count in the parent room.
        parentRoom.GetComponent<RoomManager>().enemyDecrement();

        GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateEnemies(this.transform.parent);
    }
}
