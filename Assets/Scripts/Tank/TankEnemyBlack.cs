using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TankEnemyBlack : TankEnemy
{
    // General variables
    List<Vector3> hitAngles = new List<Vector3>();
    List<float> hitWeights = new List<float>();
    float hitCount = 0;

    // State variables
    float snipeDelay = 1f;
    float chaseTime = 3f;
    Coroutine chaseTimer;
    bool chaseTimerRunning = false;

    // Shooting variables
    bool rotateCW = true;
    float rotateRandomSpeed = 10f;
    float explodeTriggerRadius = 7.5f;
    float explodeRadius = 10f;
    public GameObject suicideExplosion;
    private bool isExploding = false;

    // Driving variables
    float speedChase = 17.5f;
    Vector3 recentPos;
    float retrivalBuffer = 1f;
    Coroutine retrieveRecentPos;

    private bool needDirection = false;
    //TODO: chance of shot, figure out why the first shot doesn't happen immediately
    // value shots based on success

    // Driving variables


    protected override void resetVariables()
    {
        // General variables

        // State variables

        // Shooting variables
        fireFreq = 2f;
        projectileAmount = 1;

        // Driving variables
        m_RotateSpeed = 3f;
    }

    protected new void Start()
    {
        base.Start();
        
        // The FSM begins on Evade.
        setToSearch();
    }

    /*
    protected override void trackPlayer()
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
            if (!targets[tankI])
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
    }*/


    protected override IEnumerator FSM()
    {
        while (alive)
        {
            Debug.Log(state);
            trackPlayer();
            switch (state)
            {
                case State.CHASE:
                    Chase();
                    break;
                case State.SEARCH:
                    Search();
                    break;
                case State.IDLE:
                    Idle();
                    break;
            }
            yield return null;
        }
    }


    /*
    Functions for the CHASE state:
    Chase() - Overrides the change Idle funciton to look around randomly.
    setToChase() - Change variables and record the a recent position.
    recordRecentPos() - Coroutine that repeatedly gets the most recent position the target tank is at.
    chaseCS() - Change from CHASE state if an enemy tank hasn't been seen in chaseTime.
    chaseCSTimer() - Starts a timer to change states after chaseTime.
    driveChase() - Drives at the target tank or where the tank was is it isn't viewable.
    driveAt() - Rotates toward the target tank and drives.
    driveRemember() - Rotates toward the recent position the target tank was seen at.
    aimSpin() - Continuously rotate the tower CW or CCW.
    tryToExplode() - Call explode if the tank is close enough to an enemy.
    isExplodeDistance() - Returns true if the tank is within explodeDistance of an enemy.
    explode() - Destroys the tank and any tanks within the explodeRadius of the tank..
    DestroyTank() - Overriden to explode if the coroutine for exploding hasn't started.
    */
    protected override void Chase()
    {
        aimSpin();
        driveChase();

        tryToExplode();

        chaseCS();
    }
    protected override void setToChase()
    {
        base.setToChase();

        // Change the rotation direction.
        rotateCW = !rotateCW;
        
        // Change the drive speed.
        speedCurrent = speedChase;

        // Start recording recent positions.
        retrieveRecentPos = StartCoroutine(recordRecentPos());
    }
    private IEnumerator recordRecentPos()
    {//TODO: this isn't always updated since a tank can die inbetween collecting recentPos
        recentPos = targetTank.transform.position - transform.position;
        yield return new WaitForSeconds(retrivalBuffer);
        retrieveRecentPos = StartCoroutine(recordRecentPos());
    }
    protected override void chaseCS()
    {
        if (isEnemyViewable())
        {
            if (chaseTimerRunning)
            {
                StopCoroutine(chaseTimer);
                chaseTimerRunning = false;
            }
        }
        else
        {
            if (!chaseTimerRunning)
            {
                chaseTimer = StartCoroutine(chaseCSTimer());
            }
        }
    }
    private IEnumerator chaseCSTimer()
    {
        chaseTimerRunning = true;
        yield return new WaitForSeconds(chaseTime);
        StopCoroutine(retrieveRecentPos);
        setToSearch();
    }
    private void driveChase()
    {
        // If the enemy is viewable drive at the enemy, otherwise drive at the most recent position.
        if (isEnemyViewable())
        {
            driveAt();
        }
        else
        {
            driveRemember();
        }
    }
    private void driveAt()
    {
        targetDirectionDrive = vectorTowardTarget;

        rotateDirection();
        driveDirection();
    }
    private void driveRemember()
    {
        Debug.Log("remember");
        targetDirectionDrive = recentPos;

        rotateDirection();
        driveDirection();
    }
    private void aimSpin()
    {
        Quaternion rotation;

        if (rotateCW)
        {
            rotation = Quaternion.Euler(0, -10, 0);
        }
        else
        {
            rotation = Quaternion.Euler(0, 10, 0);
        }

        targetDirectionAim = rotation * -tower.forward;
        float step = towerRotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, targetDirectionAim, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);
    }
    private void tryToExplode()
    {
        if (isExplodeDistance())
        {
            //StartCoroutine(explode());
            DestroyTank();
        }
    }
    private bool isExplodeDistance()
    {
        float distance = Vector3.Magnitude(vectorTowardTarget);
        if (distance < explodeTriggerRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private IEnumerator explode()
    {
        isExploding = true;

        // Switch states to avoid previous momentum.
        setToIdle();

        // Display the explosion.
        GameObject explosion = Instantiate(suicideExplosion, gameObject.transform.position, Quaternion.identity) as GameObject;

        // Disappear tank.
        enabled = false;

        yield return new WaitForSeconds(.5f);

        // Destroy the tanks in targets and teammates if they are within explodeRadius.
        List<GameObject> targetsAndTeammates = new List<GameObject>();
        targetsAndTeammates.AddRange(targets);
        targetsAndTeammates.AddRange(teammates);
        foreach (GameObject tank in targetsAndTeammates)
        {
            if (Vector3.Magnitude(transform.position - tank.transform.position) < explodeRadius)
            {
                tank.GetComponent<Tank>().DestroyTank();
            }
        }

        // Destroy own tank.
        base.DestroyTank();
    }
    public override void DestroyTank()
    {
        if (!isExploding)
        {
            StartCoroutine(explode());
        }
    }


    /*
    Functions for the SEARCH state:
    Search() - Drives randomly and switches to CHASE when there is a path to an enemy tank.
    setToSearch() - Set the state to SEARCH and change variables accordingly.
    searchCS() - Place holder for children tanks to conditionally change from SEARCH state.
    */
    private void Search()
    {
        aimDirect();
        driveRandom();

        searchCS();
    }
    private void setToSearch()
    {
        //Debug.Log("set to search");
        state = TankEnemy.State.SEARCH;

        // Change the drive speed.
        speedCurrent = m_Speed;
    }
    private void searchCS()
    {
        if (isEnemyViewable())
        {
            setToChase();
        }
    }
    private bool isEnemyViewable()
    {
        // Go through the targets and see if one is viewable.
        for (int tankI = targets.Count - 1; tankI >= 0; tankI--)
        {
            Vector3 toEnemy = targets[tankI].transform.position - transform.position;

            RaycastHit hit;
            Physics.Raycast(tower.position, toEnemy, out hit, raycastLayer);

            //Debug.DrawLine(tower.position, tower.position + toEnemy * 30, Color.white, 1.5f);
            
            if (hit.transform.gameObject == targets[tankI])
            {
                return true;
            }
        }
        return false;
    }


    /*
    Functions for the IDLE state:
    Idle() - Overrides the change Idle funciton to do nothing.
    */
    protected override void Idle()
    {
        // Null
    }
}