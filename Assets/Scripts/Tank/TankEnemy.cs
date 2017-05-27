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
    private float distancePlayer1;              // The float distance to player 1.
    private Vector3 player1Pos;                 // The vector for the current position of player 1.
    private Vector3 player1Velocity;            // The vector for the current velocity of player 1.
    private Vector3 player1Hit;                 // The vector toward player1 from tankEnemy.
    private bool drivingForward;                // The bool for if the tank is driving forward.
    private float projectileSpeed;              // The speed of the projectile being shot.
    private float fireFreq = 3.0f;
    private bool canFire = true; //TODO: make it not be able to shoot for like a second

    private int roomLength = 50;                // Integer for the length of the room.
    private string playerTag = "Player";        // String representing the tag of a player.
    private string wallTag = "Wall";            // String representing the tag of a wall.
    private string blockTag = "Block";          // String representing the tag of a block.
    private string removeBlockTag = "BlockRemovable"; // String representing the tag of a removable block.

    // Driving Variables
    private float m_CurrentSpeed;               // How fast the tank is driving.
    private float m_CurrentAcceleration = 1f;   // How fast the tank changes speed.
    private float accelerationTimeMax = 3.0f;   // The max amount of time the tank stays on one speed.
    private float accelerationTimeNext;         // The randomly selected amount of time the tank stays at this speed.
    private float accelerationCounter = 0;      // The current amount of time the tank has spent on the current speed.
    private int turning = 90;                   // The amount of degrees the tank can turn.
    private Vector2 targetDirection;                  // The current targetDirection the tank is facing.
    private float turningTimeMax = 2.0f;        // The max amount of time the tank goes before turning.
    private float turningTimeNext;              // The randomly selected amount of time the tank goes before turning.
    private float turningCounter = 0;           // The current amount of time the tank has spent not turning.
    private float padding = 1.5f;               // The float representing how close a tank can move toward a wall.

    // Hunting Variables
    private float fireRate = 4.5f;               // Float for how quickly shells can be fired in succession.
    private float fireCounter = 0f;               // Float for how quickly shells can be fired in succession.
    private float huntTime = 6.0f;              // Float for how long hunting lasts without seeing a player.
    private float huntingCounter = 0;           // Float for how long hunting has gone without seeing the player.

    // Waypoint Variables
    public GameObject[] waypoints;              // Array of GameObject waypoints established by the room.
    private float minWPDist = 3.0f;             // Float for the minimum distance required to reach a waypoint.
    private int currentWaypoint = -1;           // Integer representing the current waypoint to move to.

    // State Variables
    public enum State
    {
        PATROL,
        HUNT
    }

    public State state;

    //TODO: make an array of strings of possible actions ( hunt = ["chase", "holdGround", "run"]) and select one depending on tank type


    // Use this for initialization
    private new void Start()
    {
        base.Start();

        // The FSM begins on Patrol.
        state = TankEnemy.State.PATROL;

        // Initiate the driving variables.
        m_Speed = 6f;
        m_RotateSpeed = 1f;
        accelerationTimeNext = Random.Range(0, accelerationTimeMax);
        turningTimeNext = Random.Range(0, turningTimeMax);

        // Get the script of player 1.
        player1 = GameObject.FindGameObjectWithTag("Player");
        player1Script = player1.GetComponent<TankPlayer>();

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("TankResources/ShellEnemy") as GameObject;

        // Get the script of the projectile and record its speed.
        projectileScript = projectile.GetComponent<ProjectileEnemy>();
        projectileSpeed = projectileScript.SendProjectileVelocity();

        //TODO: need to find first appropriate currentWaypoint

        // Set the beginning speed.
        m_CurrentSpeed = m_Speed;
    }

    // Called by the room to start the TankEnemy.
    public void startTankEnemy()
    {
        StartCoroutine("FSM");
    }

    // Finite State Machine representing the actions TankEnemy goes through
    IEnumerator FSM()
    {
        while (alive)
        {
            switch (state)
            {
                case State.PATROL:
                    Patrol();
                    //PatrolMeander();
                    break;
                case State.HUNT:
                    Hunt();
                    break;
            }
            yield return null;
        }
    }

    /*
    States representing the tanks action.
    Patrol - Goes from waypoint to waypoint looking for playerTanks.
    PatrolMeander - Drives randomly looking for playerTanks.
    Hunt - Shoots toward playerTank until out of sight for too long.
    */
    private void Patrol()
    {
        if (playerInSight())
        {
            state = TankEnemy.State.HUNT;
            currentWaypoint = -1;
            Debug.Log("hunt");
        }
        else if (Vector3.Distance(body.position, waypoints[currentWaypoint].transform.position) >= minWPDist)
        {
            // Move toward the current waypoint.
            moveTo(waypoints[currentWaypoint]);
        }
        else
        {
            // Travel to a neighboring waypoint.
            selectWaypoint();
        }
    }
    private void PatrolMeander()
    {
        if (playerInSight())
        {
            state = TankEnemy.State.HUNT;
            currentWaypoint = -1;
            Debug.Log("hunt");
        }
        else
        {
            meanderTo();
        }
    }
    private void Hunt()
    {
        huntingCounter += Time.deltaTime;
        fireCounter += Time.deltaTime;

        if (huntingCounter > huntTime)
        {
            huntingCounter = 0;
            selectWaypoint();
            state = TankEnemy.State.PATROL;
        }
        else if (playerInSight())
        {
            //TODO: incorporate fire rate
            if (fireCounter > fireRate)
            {
                Fire();
                fireCounter = 0;
            }
            huntingCounter = 0;
        }
    }

    /*
    Functions for the Patrol state:
    moveTo(wp) - Drives toward wp.
    rotateTo(wp) - Rotates toward wp.
    selectWaypoint() - Randomly selects a waypoint reachable by the current position.
    playerInSight() - Returns true if a player is within sight.
    */
    // Selects the first currentWayPoint randomly based on closeness.
    private void moveTo(GameObject wp)
    {
        // Only drives toward at a less than 5 degree angle.

        rotateTo(wp);
        vectorTowardPlayer1 = body.position - wp.transform.position;

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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
    public void selectWaypoint()
    {
        // Sum up the distances to each waypoint. Then, randomly select a waypoint by choosing a random
        // float below that sum and find which waypoint this corresponds to by repeatedly subtracting 
        // each distance until negative. This means closer waypoints are more likely to be selected. 
        float distanceSum = 0;
        bool[] wpOptions = new bool[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(body.position, body.position - waypoints[i].transform.position, out hit, roomLength)
                && hit.transform.tag != blockTag
                && i != currentWaypoint)
            {
                distanceSum += Vector3.Distance(body.position, waypoints[i].transform.position);
                wpOptions[i] = true;
            }
        }
        float weightedRand = Random.Range(0, distanceSum);
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (wpOptions[i])
            {
                weightedRand -= Vector3.Distance(body.position, waypoints[i].transform.position);
                if (weightedRand <= 0)
                {
                    currentWaypoint = i;
                    return;
                }
            }
        }
    }
    //TODO: make these better
    private bool playerInSight()
    {
        //TODO: tower isn't ready
        RaycastHit hit;
        return (Physics.Raycast(tower.position, -tower.forward, out hit, roomLength) && hit.transform.tag == playerTag);
    }
    private void rotateTo(GameObject wp)
    {
        // Keep track of the targetDirection toward the player.
        Vector3 m_TargetDirection = body.position - wp.transform.position;

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

    /*
    Functions for the PatrolMeander state:
    meaderTo() - Randomly changes speed and direction of the tank.
    rotateDirection() - Helper for meanderTo which changes the tanks target direction.
    selectDirection() - Helper for rotateDirection to select a random target direction.
    */
    private void meanderTo()
    {
        // Select a targetDirection when turningTimeMax is exceeded.
        turningCounter += Time.deltaTime;
        if (turningCounter > turningTimeNext)
        {
            turningCounter = 0;
            turningTimeNext = Random.Range(0, turningTimeMax);

            // Alter targetDirection.
            selectDirection();
        }
        rotateDirection();
        
        // Change the speed when accelerationTimeMax is exceeded.
        accelerationCounter += Time.deltaTime;
        if (accelerationCounter > accelerationTimeNext)
        {
            m_CurrentSpeed += Random.Range(-m_CurrentAcceleration / 2, m_CurrentAcceleration);
            accelerationCounter = 0;
            if (m_CurrentSpeed < 0)
            {
                m_CurrentSpeed = 0;
            }
            else if (m_CurrentSpeed > m_Speed)
            {
                m_CurrentSpeed = m_Speed;
            }

            // Set time until next change of speed.
            accelerationTimeNext = Random.Range(0, accelerationTimeMax);
        }

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));
        
        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
        float speed = 0;
        Vector3 m_TargetDirection = Vector3.Normalize(body.position) + new Vector3(targetDirection[0], 0, targetDirection[1]);
        if (Vector3.Angle(trueAngle, m_TargetDirection) < 5)
        {
            speed = m_CurrentSpeed;
        }
        else if (175 < Vector3.Angle(trueAngle, m_TargetDirection))
        {
            speed = -m_CurrentSpeed;
        }
        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }
    private void rotateDirection()
    {
        // This is the targetDirection to rotate to.
        Debug.Log(new Vector3(targetDirection[0], 0, targetDirection[1]));
        Vector3 target = Vector3.Normalize(body.position) + new Vector3(targetDirection[0], 0, targetDirection[1]);

        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(body.forward, target, step, .01F);

        if (newDir == Vector3.zero)
        {
            // Don't rotate at all
        }
        // Turn forward or backward depending on which is closer.
        else if (Vector3.Angle(body.forward, target) < 90)
        {
            // Rotate towards forwards.
            body.rotation = Quaternion.LookRotation(newDir);
        }
        else
        {
            // Rotate towards backwards.
            newDir = Vector3.RotateTowards(body.forward, -target, step, .01F);
            body.rotation = Quaternion.LookRotation(newDir);
        }
    }
    private void selectDirection()
    {
        RaycastHit hit;
        targetDirection = Random.insideUnitCircle;

        // Alter targetDirection.
        //TODO: this isn't a good raycast check which is SUPPOSED to make sure the tank doesn't run into walls
        while (Physics.Raycast(body.position, new Vector3(targetDirection[0], 0, targetDirection[1]), out hit, padding) &&
            (hit.transform.tag != wallTag || hit.transform.tag != blockTag || hit.transform.tag != removeBlockTag))
        {
            targetDirection = Random.insideUnitCircle;
            Debug.Log(hit.transform.tag);
        }
    }

    /*
    Functions for the Hunt state:
    Fire() - Fires a shell from the tower.
    */
    // Fires a shell.
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
        // Keep track of the targetDirection toward the player.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));


        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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
        // Keep track of the targetDirection toward the player.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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

        // Keep track of the targetDirection the tank is pointed toward.
        m_CurrentDirection = -body.eulerAngles;

        // trueAngle is used because CurrentDirection has a value like (0,-270,0) where it measures the angle from rotating around the y axis.
        // angleTargetToDirection gets a float value to later check if the tank is facing the targetDirection the joystick is pressed.
        Vector3 trueAngle = new Vector3(-Mathf.Sin(m_CurrentDirection.y * Mathf.PI / 180), 0, Mathf.Cos(m_CurrentDirection.y * Mathf.PI / 180));

        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
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
