using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Completed;
using Random = UnityEngine.Random;      // Tells Random to use the Unity Engine random number generator.

public class TankEnemy : Tank
{
    // General Variables
    public RoomManager parentRoom;              // Reference to the room TankEnemy spawns in.
    protected int roomLength = 50;              // Integer for the length of the room.
    protected string playerTag = "Player";      // String representing the tag of a player.
    protected string blockTag = "Block";        // String representing the tag of a block.

    // Shooting Variables
    protected GameObject targetTank;
    protected Vector3 vectorTowardTarget;       // The vector toward the target tank.
    protected Vector3 vectorVelocityTarget;     // The vector the target tank travels at.
    public ProjectileTest projTestScript;       // The script for shooting a test projectile.
    private float distancetargetTank;           // The float distance to player 1.
    protected float towerRotateSpeed = 2f;      // The speed the tank tower rotates at.
    protected float fireFreq;                   // The float for how frequent the tank shoots at.
    protected float fireFreqFight = 2.5f;       // The float for the fireFreq during FIGHT.
    protected float fireFreqChase = 5f;         // The float for the fireFreq during CHASE.
    protected float fireFreqExplore = 5f;       // The float for the fireFreq during EXPLORE.
    protected bool canFire = false;             // The bool for if the tank can fire.
    protected Vector3 targetDirectionAim;       // The current direction for the tank to shoot.
    private Queue<Vector3> recentShots = new Queue<Vector3>();                  // The queue for the last recentShotCount shots.
    protected int recentShotMax = 10;           // The maximum number of recent shots recorded at a time.
    protected float aimRandomChance = .5f;      // The odds of selecting a random direction to aim.
    protected LayerMask raycastLayer;                 // The layerMask for the ProjectileTest to avoid the shell game objects.

    // Driving Variables
    protected new float m_Speed = 6f; //the new keyword creates an error: https://www.youtube.com/watch?v=PFKkkLFdUtc
    protected new float m_RotateSpeed = 1f;
    private int drivingRange = 30;              // Angle used for driving toward/away from a player.
    protected float speedCurrent;               // The speed the tank drives at.
    protected Vector3 targetDirectionDrive;     // The current direction for the tank to go.
    protected float turningTimeMax = 5.0f;      // The max amount of time the tank goes before turning.
    protected float turningTimeMin = 1.0f;      // The min amount of time the tank goes before turning.
    protected float turningTimeNext;            // The randomly selected amount of time the tank goes before turning.
    private bool canTurn = true;                // The bool for if the tank can turn.
    private float padding = 5f;                 // The float representing how close a tank can move toward a wall.
    private float minExploreDist = 30f;         // The distance the tank remains in the EXPLORE state.
    private float minChaseDist = 15f;           // The distance the tank remains in the CHASE state.
    private float towardDegreesMax = 45;        // The leniency on how toward the tank can drive.
    private float awayDegreesMin = 60;          // The leniency on how away the tank can drive.
    private bool aggressive = false;

    // Waypoint Variables
    public GameObject[] waypoints;              // Array of GameObject waypoints established by the room.
    private int currentWaypoint = -1;           // Integer representing the current waypoint to move to.
    
    // State Variables
    protected enum State
    {
        //TODO: add an out function for each state that switches states or not depending on conditions
        EXPLORE,
        CHASE,
        FIGHT,
        EVADE,
        AGGRESSIVERELOAD,
        AGGRESSIVE,
        SNIPE,
        IDLE,
        SEARCH,
        EXPLODE
    }

    protected State state;

    // Use this for initialization
    protected new void Start()
    {
        base.Start();

        resetVariables();

        // Initiate the driving variables.
        turningTimeNext = Random.Range(turningTimeMin, turningTimeMax);
        speedCurrent = m_Speed;

        // Set the enemy
        projTestScript.enemyTeamName = enemyTeamName;

        // Delay fire.
        canFire = true;

        // Initiate the FSM.
        setToExplore();
        trackPlayer();
        
        //TODO: test this with obstacles
        raycastLayer = ~(1 << LayerMask.NameToLayer("Ignore Raycasat"));
    }


    // Used in inheritance to change tank enemy variables.
    protected virtual void resetVariables()
    {
        // Null
    }

    // Called by the room to start the TankEnemy.
    public void startTankEnemy()
    {
        StartCoroutine(FSM());
    }

    // Called by the room to end the TankEnemy.
    public void endTankEnemy()
    {
        setToIdle();
    }

    // Finite State Machine representing the actions TankEnemy goes through
    protected virtual IEnumerator FSM()
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
                case State.IDLE:
                    Idle();
                    break;
            }
            yield return null;
        }
    }

    // Sets target player and vectorTowardsTarget as well as putting the tank
    // in IDLE state if there are no targets.
    protected virtual void trackPlayer()
    {
        if (state == State.IDLE)
        {
            return;
        }
        else if (targets.Count == 0)
        {
            setToIdle();
            return;
        }

        // Update vectorTowardTarget and remove destroyed tanks.
        float minDist = float.PositiveInfinity;
        for (int tankI = targets.Count - 1; tankI >= 0; tankI--)
        {
            if (!targets[tankI].GetComponent<Tank>().alive)
            {
                targets.RemoveAt(tankI);
            }
            else if (!targets[tankI])
            {
                targets.RemoveAt(tankI);
            }
            else
            {
                if (Vector3.Distance(targets[tankI].transform.position, transform.position) < minDist)
                {
                    minDist = Vector3.Distance(targets[tankI].transform.position, transform.position);
                    targetTank = targets[tankI];
                }
            }
        }

        if (targets.Count != 0)
        {
            vectorTowardTarget = targetTank.transform.position - transform.position;
            vectorVelocityTarget = targetTank.GetComponent<Tank>().SendVelocity();
        }
    }

    /*
    States representing the tanks action.
    Explore - Drive randomly and fire predictive shots.
    Chase - Drive toward player and fire direct.
    Fight - Drive randomly and fire direct.
    Evade - Drive away and aim scan.
    ChaseAggressive - Drive directly at the play and fire burst shots.
    */

    /*
    Functions for the Explore state:
    setToExplore() - Set the state to EXPLORE and change variables accordingly.
    exploreCS() - Change state from EXPLORE to CHASE when necessary.
    isExploreDistance() - Helper for exploreCS, returns true if the tank is in EXPLORE range.
    rotateDirection() - Rotates the EnemyTank to a random direction on random intervals.
    selectDirectionRandom() - Selects a random, unobstructed direction.
    delayTurn() - Helper function for selectDirection by keeping the canTurn bool false until the tank can turn.
    driveDirection() - Moves EnemyTank toward targetDirection when forward or backward is within 5 degrees.
    driveRandom() - Drives around the map and avoids walls.
    Fire() - Fires a projectile if canFire and TankEnemy has projectiles.
    firePredict() - Aims predictively at the the player and shoots.
    aimPredict() - Aims at where targetTank will be.
    obstructed() - Returns true if there is something in the way of driving.
    */
    protected void Explore()
    {
        firePredict();
        driveRandom();
        
        exploreCS();
    }
    protected void setToExplore()
    {
        state = TankEnemy.State.EXPLORE;

        speedCurrent = m_Speed;
        
        fireFreq = fireFreqExplore;
    }
    protected virtual void exploreCS()
    {
        //TODO: needs to include 2nd player tanks
        //float distance = Vector3.Distance(body.position, targetTank.transform.position);
        float distance = vectorTowardTarget.magnitude;
        if (!isExploreDistance())
        {
            setToChase();
        }
    }
    protected bool isExploreDistance()
    {
        //TODO: needs to include 2nd player tanks
        float distance = vectorTowardTarget.magnitude;// Vector3.Distance(body.position, targetTank.transform.position);
        if (distance > minExploreDist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected void rotateDirection()
    {
        // This is the targetDirection to rotate to.
        Vector3 target = targetDirectionDrive;

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
            targetDirectionDrive.Set(unitPlane[0], 0, unitPlane[1]);
            while (obstructed(targetDirectionDrive))
            {
                // Set targetDirection along the x-z plane.
                unitPlane = Random.insideUnitCircle.normalized;
                targetDirectionDrive.Set(unitPlane[0], 0, unitPlane[1]);
            }

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
    protected void driveDirection()
    {
        // If the tank isn't facing the targetDirection the joystick is pointing, the speed equals 0.
        float speed = 0;

        // Set the speed to speedCurrent if the tank is pointed to within 5 degrees of the target direction from either end.

        if (Vector3.Angle(body.forward, targetDirectionDrive) < 5 || 360 < Vector3.Angle(body.forward, targetDirectionDrive))
        {
            speed = speedCurrent;
        }
        else if (175 < Vector3.Angle(body.forward, targetDirectionDrive) && Vector3.Angle(body.forward, targetDirectionDrive) < 185)
        {
            speed = -speedCurrent;
        }

        // Move the rigidbody.
        Vector3 movement = body.forward * speed;
        m_RidgidbodyTank.velocity = movement;

        // Update velocity.
        velocity = body.forward * speed;
    }
    protected void driveRandom()
    {
        // Rotate the car toward targetDirection.
        selectDirectionRandom();
        rotateDirection();

        // Drive toward targetDirection.
        driveDirection();
    }
    protected new void Fire()
    {
        if (canFire && projectileCount > 0)
        {
            StartCoroutine(delayFire());
            base.Fire();
            recordShot(tower.forward);
        }
    }
    protected void firePredict()
    {
        // Aim directly at the player.
        aimPredict();

        // Fire if there are bullets, fire frequency has elapsed, and the tower is predicting.
        if (tower.forward == targetDirectionAim)
        {
            ProjectileTest.ShotReport report = projTestScript.beginShoot(tower.position, tower.forward);

            if (report.isHit)
            {
                Fire();
            }
        }
    }
    private void aimPredict()
    {
        float step = towerRotateSpeed * Time.deltaTime;
        targetDirectionAim = Vector3.Normalize(vectorTowardTarget - vectorVelocityTarget);
        Vector3 newDir = Vector3.RotateTowards(tower.forward, targetDirectionAim, step, .01F);
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
    chaseCS() - Change state from CHASE if the distance from player isn't in the CHASE range.
    isChaseDistance() - Returns true if EnemyTank is inside of CHASE range.
    driveToward() - Drives toward the player tank.
    selectDirectionToward() - Selects a toward, unobstructed direction.
    isToward(V) - Returns true if V is toward from the tank by less than towardDegreesMax.
    fireDirect() - Aims directly at the the player and shoots.
    aimDirect() - Aims directly at targetTank.
    delayFire() - Sets canFire to false, waits fireFreq, the sets canFire to true.
    */
    protected virtual void Chase()
    {
        fireDirect();
        driveToward();

        chaseCS();
    }
    protected virtual void setToChase()
    {
        fireFreq = fireFreqChase;
        state = TankEnemy.State.CHASE;
    }
    protected virtual void chaseCS()
    {
        if (isExploreDistance())
        {
            setToExplore();
        }
        else if (!isChaseDistance())
        {
            setToFight();
        }
    }
    private bool isChaseDistance()
    {
        //TODO: needs to include 2nd player tanks
        float distance = Vector3.Magnitude(vectorTowardTarget);
        if (minChaseDist < distance && distance <= minExploreDist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected void driveToward()
    {
        // Rotate the car toward targetDirection.
        selectDirectionToward();
        rotateDirection();

        // Drive toward targetDirection.
        driveDirection();
    }
    private void selectDirectionToward()
    {
        // If the turning counter exceeds the max count, look for a new direction.
        if (canTurn)
        {

            // Try at most 10 times to find a toward direction.
            int trys = 10;

            // Set targetDirection along the x-z plane and do so until a valid direction is selected.
            Vector2 unitPlane = Random.insideUnitCircle.normalized;
            targetDirectionDrive.Set(unitPlane[0], 0, unitPlane[1]);

            while ((obstructed(targetDirectionDrive) || !isToward(targetDirectionDrive)) && (trys > 0))
            {
                // Set targetDirection along the x-z plane.
                unitPlane = Random.insideUnitCircle.normalized;
                targetDirectionDrive.Set(unitPlane[0], 0, unitPlane[1]);

                --trys;
            }
            StartCoroutine(delayTurn());
        }
    }
    private bool isToward(Vector3 V)
    {
        // Return true if V is close enough toward from vectorTowardTarget.
        if (Mathf.Abs(Vector3.Angle(V, vectorTowardTarget)) < towardDegreesMax)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected void fireDirect()
    {
        // Aim directly at the player.
        aimDirect();

        // Fire if there are bullets and fire frequency has elapsed.
        if (tower.forward == targetDirectionAim)
        {
            Fire();
        }
    }
    protected void aimDirect()
    {
        // TankEnemy looks directly at the player.
        // TODO: incorporate player2.
        float step = towerRotateSpeed * Time.deltaTime;
        targetDirectionAim = Vector3.Normalize(vectorTowardTarget);
        Vector3 newDir = Vector3.RotateTowards(tower.forward, targetDirectionAim, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);
    }
    protected virtual IEnumerator delayFire()
    {
        canFire = false;
        yield return new WaitForSeconds(fireFreq);
        canFire = true;
    }

    /*
    Functions for the Fight state:
    setToFight() - Set the state to FIGHT and change variables accordingly.
    fightCS() - Change state from FIGHT to CHASE if the distance from player isn't in the FIGHT range.
    isFightDistance() - Returns true if EnemyTank is inside of FIGHT range.
    */
    protected void Fight()
    {
        fireDirect();
        driveRandom();

        fightCS();
    }
    protected void setToFight()
    {
        fireFreq = fireFreqFight;
        state = TankEnemy.State.FIGHT;
    }
    protected virtual void fightCS()
    {
        if (!isFightDistance())
        {
            setToChase();
        }
    }
    protected virtual bool isFightDistance()
    {
        float distance = Vector3.Magnitude(vectorTowardTarget);
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
    evadeCS() - Place holder for children tanks to conditionally change from EVADE state.
    aimRicochet() - Aims at a wall to hit the tank.
    driveAway() - Drives away from the player tank.
    selectDirectionAway() - Selects an away, unobstructed direction.
    isAway(V) - Returns true if V is away from the tank by at least awayDegreesMin.
    aimScan() - Scans around and looks for a tank player.
    isHit(V) - Returns true if a player tank can be hit from shooting at V.
    scanTo(V) - Rotates the tower toward V.
    aimDirection() - Rotates the tower toward 
    selectDirection() - Selects a random or recent direction for targetDirectionAim.
    recordShot() - Updates the recentShots queue with the most recent raycasts that hit a playerTank.
    */
    protected void Evade()
    {
        driveAway();
        aimScan();

        evadeCS();
    }
    protected void setToEvade()
    {
        Debug.Log("evade");
        state = TankEnemy.State.EVADE;

        // Start aiming at the player.
        selectDirectionAim();
    }
    protected void evadeCS()
    {
        
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
            targetDirectionDrive.Set(unitPlane[0], 0, unitPlane[1]);
            
            while ((obstructed(targetDirectionDrive) || !isAway()) && (trys > 0))
            {
                // Set targetDirection along the x-z plane.
                unitPlane = Random.insideUnitCircle.normalized;
                targetDirectionDrive.Set(unitPlane[0], 0, unitPlane[1]);

                --trys;
            }
            StartCoroutine(delayTurn());
        }
    }
    private bool isAway()
    {
        // Return true if V is far enough away from vectorTowardTarget.
        if (Mathf.Abs(Vector3.Angle(targetDirectionDrive, vectorTowardTarget)) > awayDegreesMin)
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
        scanTo();
    }
    protected bool isHit()
    {
        ProjectileTest.ShotReport report = projTestScript.beginShoot(m_ProjectileSpawnPoint.position, -tower.forward);
        return report.isHit;
    }
    protected void scanTo()
    {
        aimDirection();

        if (tower.forward == targetDirectionAim)
        {
            selectDirectionAim();
            Fire();
        }
    }
    protected void aimDirection()
    {
        float step = towerRotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, targetDirectionAim, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);
    }
    private void selectDirectionAim()
    {
        // If there are a maximum amount of vectors in the queue
        if (recentShots.Count > 0)
        {
            // Select the odds of a random direction or a recent direction to select.
            float odds = Random.Range(0, 1);

            if (odds > aimRandomChance)
            {
                // Select a random direction from recentShots to scan to.
                Vector3[] tempQ = recentShots.ToArray();

                int randomRecentShot = (int)Random.Range(0, recentShotMax);
                targetDirectionAim = tempQ[randomRecentShot];
            }
            else
            {
                // Otherwise, scan to a random direction.
                Vector2 unitPlane = Random.insideUnitCircle.normalized;
                targetDirectionAim.Set(unitPlane[0], 0, unitPlane[1]);
            }
        }
        else
        {
            // Otherwise, scan to a random direction.
            Vector2 unitPlane = Random.insideUnitCircle.normalized;
            targetDirectionAim.Set(unitPlane[0], 0, unitPlane[1]);
        }
    }
    private void recordShot(Vector3 s)
    {
        // Enqueue as direction and dequeue a direction if the queue exceeds the max count.
        recentShots.Enqueue(s);
        if (recentShots.Count > recentShotMax)
        {
            recentShots.Dequeue();
        }
    }

    /*
    Functions for the IDLE state:
    setToIdle() - Set the state to NOTHING.
    */
    protected virtual void Idle()
    {
        driveRandom();
    }
    public void setToIdle()
    {
        //Debug.Log("set to idle");
        state = TankEnemy.State.IDLE;

        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;
    }







    // Temp: for turning
    protected void OnCollisionEnter(Collision collisionInfo)
    {
        // The TankEnemy collided with a wall or block so set the turn counter to max.
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

    protected void FirePredict()
    {
        /*
        float step = m_RotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, targetTankHit + vectorTowardTarget, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);
        

        if (Vector3.Normalize(newDir) == Vector3.Normalize(targetTankHit + vectorTowardTarget))
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
        //tower.LookAt(targetTankHit + tower.position + vectorTowardTarget);
    }

    protected void RotateToward()
    {
        // Keep track of the targetDirection toward the player.
        Vector3 m_TargetDirection = vectorTowardTarget;

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
        if (Vector3.Angle(trueAngle, -vectorTowardTarget) < 5)
        {
            speed = m_Speed;
        }
        else if (175 < Vector3.Angle(trueAngle, -vectorTowardTarget))
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
        if (Vector3.Angle(trueAngle, -vectorTowardTarget) < 5)
        {
            speed = -m_Speed;
        }
        else if (175 < Vector3.Angle(trueAngle, -vectorTowardTarget))
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
        if (Vector3.Angle(trueAngle, -vectorTowardTarget) < drivingRange)
        {
            speed = m_Speed;
        }
        else if (180 - drivingRange < Vector3.Angle(trueAngle, -vectorTowardTarget))
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
        if (Vector3.Angle(trueAngle, -vectorTowardTarget) < drivingRange)
        {
            speed = -m_Speed;
        }
        else if (180 - drivingRange < Vector3.Angle(trueAngle, -vectorTowardTarget))
        {
            speed = m_Speed;
        }

        Vector3 movement = body.forward * speed * Time.deltaTime;

        m_RidgidbodyTank.MovePosition(m_RidgidbodyTank.position + movement);
    }
    
    protected void RotatePerpendicular()
    {
        // Keep track of the targetDirection toward the player.
        Vector3 m_TargetDirection = Vector3.Cross(vectorTowardTarget, Vector3.up);

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
        if (85 < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 95)
        {
            speed = -m_Speed;
        }
        else if (265 < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 275)
        {
            speed = -m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardTarget) < 1)
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
        if (90 - drivingRange < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 90 + drivingRange)
        {
            speed = -m_Speed;
        }
        else if (270 - drivingRange < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 270 + drivingRange)
        {
            speed = -m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardTarget) < 1)
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
        if (85 < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 95)
        {
            speed = m_Speed;
        }
        else if (265 < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 275)
        {
            speed = m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardTarget) < 1)
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
        if (90 - drivingRange < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 90 + drivingRange)
        {
            speed = m_Speed;
        }
        else if (270 - drivingRange < CalculateAngle(trueAngle, vectorTowardTarget) && CalculateAngle(trueAngle, vectorTowardTarget) < 270 + drivingRange)
        {
            speed = m_Speed;
        }
        // This prevents the tank from stopping when the angle is 0.
        else if (CalculateAngle(trueAngle, vectorTowardTarget) < 1)
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
    




    public override void DestroyTank()
    {
        base.DestroyTank();

        // Update the enemy count in the parent room.
        if (!testEnvironment)
        {
            parentRoom.GetComponent<RoomManager>().enemyDecrement();
            GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().UpdateEnemies(this.transform.parent);
        }
    }
}
