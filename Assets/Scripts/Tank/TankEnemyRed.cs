using UnityEngine;
using System.Collections;

public class TankEnemyRed : TankEnemy
{
    //TODO: create new state for circling
    // General variables

    // State variables
    private float coolDownEnd = 4f;             // The length of time the tank stays aggressive without seeing the player.
    private float coolDownTimer;                // The length of time the tank has gone without seeing the player.
    private Coroutine cD;
    private bool coolingDown = false;

    // Shooting variables
    private bool canBurstFire = false;
    private float burstFireFreq = 12.5f;         // The amount of time between burst fires.
    protected float fireBurstFreq = .15f;       // The amount of time between each individual shot during burst fire.
    private float burstAccuracy = 15f;          // The max amount of degrees a burst shot can miss the player by.

    protected new float fireFreqFight = .35f;

    // Driving variables
    protected new float m_RotateSpeed = 5f;
    protected new float turningTimeMax = 2.5f;
    protected new float turningTimeMin = .5f;
    private float fightDistance = 20f;          // The distance the tank gets to the player tank before fighting.
    private float speedAggressive = 10f;        // The speed the tank drives at.

    private float fightDistanceMax = 30f;


    protected new void Start()
    {
        base.Start();

        // The FSM begins on Evade.
        setToExplore();
    }
    
    private new IEnumerator FSM()
    {
        while (alive)
        {
            trackPlayer();
            switch (state)
            {
                case State.AGGRESSIVERELOAD:
                    AggressiveReload();
                    break;
                case State.AGGRESSIVE:
                    Aggressive();
                    break;
                case State.EXPLORE:
                    Explore();
                    break;
                case State.FIGHT:
                    Fight();
                    break;
            }
            yield return null;
        }
    }


    /*
    Functions for the FightAggressive state:
    setToAggressive() - Set the state to AGGRESSIVE and change variables accordingly.
    setToAggressiveNoCD() - Same as setToAggressive() but don't restart the cooldown timer.
    driveAggressive() - Drives directly at player if far enough away.
    fireBurst() - Shoots accurately until out of projectiles.
    scanToBurst() - Scans toward the targetDirectionAim and fires.
    selectDirectionAimBurst() - Selects targetDirectionAim to be at most burstAccuracy away from the vector toward the player.
    burstFire() - Starts a coroutine to change the state to FightAggressive
    aggressiveCS() - Changes state from AGGRESSIVE if conditions are met.
    */
    protected void Aggressive()
    {
        driveAggressive();
        fireBurst();

        aggressiveCS();
    }
    protected void setToAggressive()
    {
        state = TankEnemy.State.AGGRESSIVE;

        // Change the drive speed.
        speedCurrent = speedAggressive;

        // Start cool down timer.
        if (!coolingDown)
        {
            cD = StartCoroutine(coolDown());
        }

        // Set variables
        canFire = true;
        fireFreq = fireBurstFreq;
        targetDirectionAim = vectorTowardPlayer1;
    }
    protected void setToAggressiveNoCD()
    {
        state = TankEnemy.State.AGGRESSIVE;

        // Change the drive speed.
        speedCurrent = speedAggressive;
    }
    private void driveAggressive()
    {
        // Rotate the car toward targetDirection.
        selectDirectionDirect();
        rotateDirection();

        driveDirection();
    }
    private void fireBurst()
    {
        scanToBurst();

        if (projectileCount <= 0)
        {
            setToAggressiveReload();
        }
    }
    private void scanToBurst()
    {
        float step = towerRotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, -targetDirectionAim, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);

        if (tower.forward == -targetDirectionAim && canFire)
        {
            Fire();
            selectDirectionAimBurst();
        }
    }
    private void selectDirectionAimBurst()
    {
        // Select random vector within burstAccuracy degrees of vector towards player.
        float angleOffset = Random.Range(-burstAccuracy, burstAccuracy);
        int sign = 1;
        if (Vector3.Cross(Vector3.forward, vectorTowardPlayer1).y < 0)
        {
            sign = -1;
        }
        float angle = sign * Vector3.Angle(Vector3.forward, vectorTowardPlayer1) + angleOffset;
        targetDirectionAim = new Vector3(Mathf.Sin(angle * Mathf.PI / 180), 0, Mathf.Cos(angle * Mathf.PI / 180));
    }
    private IEnumerator burstFire()
    {
        canBurstFire = false;
        yield return new WaitForSeconds(burstFireFreq);
        // Change states.
        canBurstFire = true;
    }
    private void aggressiveCS()
    {
        // Drive toward targetDirection if farther than fightDistance from player1.
        if (Vector3.Distance(transform.position, player1.transform.position) < fightDistance)
        {
            setToFight();
        }
    }
    
    /*
    Functions for the AGGRESSIVERELOAD state:
    setToAggressiveReload() - Set the state to AGGRESSIVE and change variables accordingly.
    selectDirectionDirect() - Sets targetDirectionDrive to directly at player.
    coolDown() - Change tank to different state if the player hasn't been seen in coolDownEnd seconds.
    playerInSight() - Returns true if player is in sight.
    aggressiveCS() - Changes state from AGGRESSIVERELOAD if conditions are met.
    */
    protected void AggressiveReload()
    {
        driveAggressive();
        aimDirect();

        aggressiveReloadCS();
    }
    protected void setToAggressiveReload()
    {
        state = TankEnemy.State.AGGRESSIVERELOAD;

        // Change the drive speed.
        speedCurrent = speedAggressive;

        // Start timer until next burst shot.
        StartCoroutine(burstFire());
    }
    private void selectDirectionDirect()
    {
        targetDirectionDrive = vectorTowardPlayer1;
    }
    private IEnumerator coolDown()
    {
        coolingDown = true;
        float eps = .01f;
        while (coolDownTimer < coolDownEnd)
        {
            yield return new WaitForSeconds(eps);

            coolDownTimer += eps;
            if (playerInSight())
            {
                coolDownTimer = 0;
            }
        }

        // Change states.
        coolingDown = false;
        setToExplore();
    }
    protected bool playerInSight()
    {
        RaycastHit hit;
        return (Physics.Raycast(tower.position, -tower.forward, out hit, roomLength) && hit.transform.tag == playerTag);
    }
    private void aggressiveReloadCS()
    {
        // Drive toward targetDirection if farther than fightDistance from player1.
        if (Vector3.Distance(transform.position, player1.transform.position) < fightDistance)
        {
            setToFight();
        }

        if (projectileCount == projectileAmount && canBurstFire)
        {
            setToAggressiveNoCD();
        }
    }

    /*
    Functions for the FIGHT state:
    fightCS() - Change state from FIGHT to AGGRESSIVE if the distance from player isn't in the FIGHT range.
    isFightDistance() - Returns true if EnemyTank is inside of FIGHT range.
    */
    protected new void Fight()
    {
        firePredict();
        driveRandom();

        fightCS();
    }
    protected new void setToFight()
    {
        base.setToFight();

        // Set variables.
        canFire = true;
        coolingDown = false;
        fireFreq = fireFreqFight;

        StopCoroutine(cD);
    }
    protected override void fightCS()
    {
        
        if (!isFightDistance())
        {
            setToExplore();
        }
    }
    protected override bool isFightDistance()
    {
        //TODO: needs to include 2nd player tanks
        float distance = Vector3.Distance(body.position, player1.transform.position);
        if (distance < fightDistanceMax)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
    Functions for EXPLORE state:
    exploreCS() - Changes state from EXPLORE if conditions are met.
    playerIsVisible() - Returns true if player is visible from tank position.
    */
    protected override void exploreCS()
    {
        if (playerIsVisible())
        {
            setToAggressive();
        }
    }
    private bool playerIsVisible()
    {
        RaycastHit hit;
        return (Physics.Raycast(tower.position, vectorTowardPlayer1, out hit, roomLength) && hit.transform.tag == playerTag);
    }
}