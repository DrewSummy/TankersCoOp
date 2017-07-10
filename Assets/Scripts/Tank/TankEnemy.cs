using UnityEngine;
using System.Collections;
using Completed; // does this need {}

public class TankEnemy : Tank
{
    // General Variables
    public RoomManager parentRoom;              // Reference to the room TankEnemy spawns in.
    private GameObject player1;                 // Reference to the player 1 game object.
    //private TankPlayer player1Script;           // Store a reference to the TankPlayer of player 1.
    //private TankPlayer player2Script;           // Store a reference to the TankPlayer of player 2.
    private int roomLength = 50;                // Integer for the length of the room.
    private string playerTag = "Player";        // String representing the tag of a player.
    private string blockTag = "Block";          // String representing the tag of a block.

    // Shooting Variables
    private Vector3 player1Pos;                 // The vector for the current position of player 1.
    private Vector3 player1Velocity;            // The vector for the current velocity of player 1.
    private Vector3 vectorTowardPlayer1;        // The vector toward player 1.
    private ProjectileTest projTestScript;
    private float distancePlayer1;              // The float distance to player 1.
    private float fireFreq;                     // The float for how frequent the tank shoots at.
    private float fireFreqFight = 1.5f;         // The float for the fireFreq during FIGHT.
    private float fireFreqChase = .75f;         // The float for the fireFreq during CHASE.
    private bool canFire = true;                // The bool for if the tank can turn.
    private float m_towerSpeed = 90f;           // The float for how fast the tower rotates.
    protected GameObject projTest;

    // Driving Variables
    private int drivingRange = 30;              // Angle used for driving toward/away from a player.
    private float m_CurrentSpeed;               // How fast the tank is driving.
    private Vector3 targetDirection;            // The current targetDirection for the tank to go.
    private float turningTimeMax = 5.0f;        // The max amount of time the tank goes before turning.
    private float turningTimeMin = 1.0f;        // The min amount of time the tank goes before turning.
    private float turningTimeNext;              // The randomly selected amount of time the tank goes before turning.
    private bool canTurn = true;                // The bool for if the tank can turn.
    private float padding = 5f;                 // The float representing how close a tank can move toward a wall.
    private float minExploreDist = 30f;         // The distance the tank remains in the EXPLORE state.
    private float minChaseDist = 15f;           // The distance the tank remains in the CHASE state.
    private float awayDegreesMin = 135;         // The leniency on how away the tank can drive.

    // Waypoint Variables
    public GameObject[] waypoints;              // Array of GameObject waypoints established by the room.
    private int currentWaypoint = -1;           // Integer representing the current waypoint to move to.

    // State Variables
    public enum State
    {
        EXPLORE,
        CHASE,
        FIGHT,
        EVADE
    }

    public State state;

    //TODO: make an array of strings of possible actions ( hunt = ["chase", "holdGround", "run"]) and select one depending on tank type


    // Use this for initialization
    private new void Start()
    {
        base.Start();

        // The FSM begins on Explore.
        //setToExplore();
        setToEvade();

        // Initiate the driving variables.
        m_Speed = 6f;
        m_RotateSpeed = 1f;
        turningTimeNext = Random.Range(turningTimeMin, turningTimeMax);
        m_CurrentSpeed = m_Speed;

        // Initiate the shooting variables.
        fireFreq = fireFreqChase;

        // Get the script of player 1.
        player1 = GameObject.FindGameObjectWithTag("Player");
        //player1Script = player1.GetComponent<TankPlayer>();

        // Load in the projectile being used from the Resources folder in assets.
        projectile = Resources.Load("TankResources/ShellEnemy") as GameObject;

        // Get the script of the projectile and record its speed.
        projTestScript = GetComponent<ProjectileTest>();
        projTest = Resources.Load("TankResources/ProjTest") as GameObject;
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
            trackPlayer();

            switch (state)
            {
                case State.EXPLORE:
                    Explore();
                    break;
                case State.CHASE:
                    Chase();
                    break;
                case State.FIGHT:
                    Fight();
                    break;
                case State.EVADE:
                    Evade();
                    break;
            }
            yield return null;
        }
    }

    private void trackPlayer()
    {
        //TODO: incorporate player2
        // Update vectorTowardPlayer1
        //vectorTowardPlayer1 = body.position - player1.transform.position;
        vectorTowardPlayer1 = player1.transform.position - body.position;
    }

    /*
    States representing the tanks action.
    Patrol - Goes from waypoint to waypoint looking for playerTanks.
    PatrolMeander - Drives randomly looking for playerTanks.
    Hunt - Shoots toward playerTank until out of sight for too long.
    */
    private void Explore()
    {
        if (isExploreDistance())
        {
            // Aim directly and drive randomly
            aimDirect();
            driveRandom();
        }
        else
        {
            setToChase();
        }
    }
    private void Chase()
    {
        if (isChaseDistance())
        {
            //TODO
            fireDirect();
            driveRandom();
        }
        else if (isExploreDistance())
        {
            setToExplore();
        }
        else
        {
            setToFight();
        }

    }
    private void Fight()
    {
        if (isFightDistance())
        {
            //TODO
            fireDirect();
            driveRandom();
        }
        else
        {
            setToChase();
        }
    }
    private void Evade()
    {
        aimScan();
        //driveAway();
    }


    /*
    Functions for the Explore state:
    setToExplore() - Set the state to EXPLORE and change variables accordingly.
    isExploreDistance() - Returns true if EnemyTank is in EXPLORE range.
    rotateDirection() - Rotates the EnemyTank to a random direction on random intervals.
    selectDirectionRandom() - Selects a random, unobstructed direction.
    delayTurn() - Helper function for selectDirection by keeping the canTurn bool false until the tank can turn.
    driveDirection() - Moves EnemyTank toward targetDirection when forward or backward is within 5 degrees.
    driveRandom() - Drives around the map and avoids walls.
    aimDirect() - Aims directly at player1.
    obstructed() - Returns true if there is something in the way.
    */
    //TODO: drives in wrong direction
    private void setToExplore()
    {
        Debug.Log("explore");
        state = TankEnemy.State.EXPLORE;
    }
    private bool isExploreDistance()
    {
        //TODO: needs to include 2nd player tanks
        float distance = Vector3.Distance(body.position, player1.transform.position);
        if (distance > minExploreDist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void rotateDirection()
    {
        // This is the targetDirection to rotate to.
        Vector3 target = targetDirection;

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
    private void selectDirectionRandom()
    {
        // If the turning counter exceeds the max count, look for a new direction.
        if (canTurn)
        {
            // Set targetDirection along the x-z plane and do so until a valid direction is selected.
            Vector2 unitPlane = Random.insideUnitCircle.normalized;
            targetDirection.Set(unitPlane[0], 0, unitPlane[1]);
            while (obstructed(targetDirection))
            {
                //Debug.DrawLine(body.position, Vector3.up + body.position + padding * targetDirection, Color.red, 10);
                // Set targetDirection along the x-z plane.
                unitPlane = Random.insideUnitCircle.normalized;
                targetDirection.Set(unitPlane[0], 0, unitPlane[1]);
            }
            //Debug.DrawLine(body.position, Vector3.up + body.position + padding * targetDirection, Color.white, 10);

            StartCoroutine(delayTurn());
        }
    }
    private IEnumerator delayTurn()
    {
        canTurn = false;
        yield return new WaitForSeconds(turningTimeNext);
        turningTimeNext = Random.Range(turningTimeMin, turningTimeMax);
        canTurn = true;
    }
    private void driveDirection()
    {
        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
        float speed = 0;
        

        // Set the speed to m_CurrentSpeed if the tank is pointed to within 5 degrees of the target direction from either end.

        if (Vector3.Angle(body.forward, targetDirection) < 5 || 360 < Vector3.Angle(body.forward, targetDirection))
        {
            speed = m_CurrentSpeed;
            //Debug.DrawLine(body.position, Vector3.up + body.position + padding * speed * body.forward, Color.green, 1);
        }
        else if (175 < Vector3.Angle(body.forward, targetDirection) && Vector3.Angle(body.forward, targetDirection) < 185)
        {
            speed = -m_CurrentSpeed;
            //Debug.DrawLine(body.position, Vector3.up + body.position + padding * speed * body.forward, Color.green, 1);
        }

        // Move the rigidbody.
        Vector3 movement = body.forward * speed * Time.deltaTime;
        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }
    private void driveRandom()
    {
        // Rotate the car toward targetDirection.
        selectDirectionRandom();
        rotateDirection();

        // Drive toward targetDirection.
        driveDirection();
    }
    private void aimDirect()
    {
        // TankEnemy looks directly at the player.
        // TODO: incorporate player2.
        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, -vectorTowardPlayer1, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);
    }
    private bool obstructed(Vector3 V)
    {
        RaycastHit hit;
        return Physics.Raycast(body.position, V, out hit, padding);
    }

    /*
    Functions for the Chase state:
    setToChase() - Set the state to CHASE and change variables accordingly.
    isChaseDistance() - Returns true if EnemyTank is inside of CHASE range.
    fireDirect() - Aims directly at the the player and shoots.
    Fire() - Fires a projectile if canFire and TankEnemy has projectiles.
    delayFire() - Sets canFire to false, waits fireFreq, the sets canFire to true.
    */
    private void setToChase()
    {
        Debug.Log("chase");
        fireFreq = fireFreqChase;
        state = TankEnemy.State.CHASE;
    }
    private bool isChaseDistance()
    {
        //TODO: needs to include 2nd player tanks
        float distance = Vector3.Distance(body.position, player1.transform.position);
        if (minChaseDist < distance && distance <= minExploreDist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void fireDirect()
    {
        // Aim directly at the player.
        aimDirect();

        // Fire if there are bullets and fire frequency has elapsed.
        Fire();
    }
    protected new void Fire()
    {
        if (canFire && projectileCount > 0)
        {
            StartCoroutine(delayFire());
            base.Fire();
        }
    }
    private IEnumerator delayFire()
    {
        canFire = false;
        yield return new WaitForSeconds(fireFreq);
        canFire = true;
    }

    /*
    Functions for the Fight state:
    setToFight() - Set the state to FIGHT and change variables accordingly.
    isFightDistance() - Returns true if EnemyTank is inside of FIGHT range.
    */
    private void setToFight()
    {
        Debug.Log("fight");
        fireFreq = fireFreqFight;
        state = TankEnemy.State.FIGHT;
    }
    private bool isFightDistance()
    {
        //TODO: needs to include 2nd player tanks
        float distance = Vector3.Distance(body.position, player1.transform.position);
        if (minChaseDist >= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /*
    Functions for the Evade state:
    setToEvade() - Set the state to EVADE and change variables accordingly.
    aimRicochet() - Aims at a wall to hit the tank.
    driveAway() - Drives away from the player tank.
    selectDirectionAway() - Selects an away, unobstructed direction.
    isAway(V) - Returns true if the direction is away from the tank by at least awayDegreesMin.
    aimScan() - Scans around and looks for a tank player.
    isHit(V) - Returns true if a player tank can be hit from shooting.
    */
    private void setToEvade()
    {
        Debug.Log("evade");
        state = TankEnemy.State.EVADE;
    }
    private void driveAway()
    {
        // Rotate the car toward targetDirection.
        selectDirectionAway();
        rotateDirection();

        // Drive toward targetDirection.
        driveDirection();
    }
    private void selectDirectionAway()
    {
        // If the turning counter exceeds the max count, look for a new direction.
        if (canTurn)
        {

            // Try at most 10 times to find an away direction.
            int trys = 10;
            
            // Set targetDirection along the x-z plane and do so until a valid direction is selected.
            Vector2 unitPlane = Random.insideUnitCircle.normalized;
            targetDirection.Set(unitPlane[0], 0, unitPlane[1]);

            Debug.Log(Mathf.Abs(Vector3.Angle(targetDirection, vectorTowardPlayer1)));
            Debug.Log(isAway(targetDirection));
            while ((obstructed(targetDirection) || !isAway(targetDirection)) && (trys > 0))
            {
                // Set targetDirection along the x-z plane.
                unitPlane = Random.insideUnitCircle.normalized;
                targetDirection.Set(unitPlane[0], 0, unitPlane[1]);

                --trys;
                Debug.Log(Mathf.Abs(Vector3.Angle(targetDirection, vectorTowardPlayer1)));
                Debug.Log(isAway(targetDirection));
            }
            Debug.Log(trys);
            StartCoroutine(delayTurn());
        }
    }
    private bool isAway(Vector3 V)
    {
        // Return true if V is far enough away from vectorTowardPlayer1.
        if (Mathf.Abs(Vector3.Angle(V, vectorTowardPlayer1)) > awayDegreesMin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void aimScan()
    {

        float step = m_towerSpeed * Time.deltaTime;
        tower.transform.Rotate(Vector3.up * step);
        
        if (isHit(tower.forward))
        {
            //Debug.Log("fire");
            //Fire();
        }
    }
    private bool isHit(Vector3 V)
    {
        //TODO:
        //projTestScript.shoot(tower.position, -tower.forward);

        
        return projTestScript.beginShoot(m_ProjectileSpawnPoint.position, -tower.forward);
    }

    // Temp: for turning
    protected void OnCollisionEnter(Collision collisionInfo)
    {
        // The TankEnemy collided with a wall so set the turn counter to max.
        if (true)//collisionInfo.transform.tag == "Wall")
        {
            canTurn = true;
        }
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
            //TODO: use 2^.5 * roomLength
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

    private bool playerInSight()
    {
        //TODO: tower isn't ready
        RaycastHit hit;
        return (Physics.Raycast(tower.position, -tower.forward, out hit, roomLength) && hit.transform.tag == playerTag);
    }

    protected void FirePredict()
    {
        /*
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
        }*/
    }

    protected void AimPredict()
    {
        // AI to look infront of player.
        //tower.LookAt(player1Hit + tower.position + vectorTowardPlayer1);
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
