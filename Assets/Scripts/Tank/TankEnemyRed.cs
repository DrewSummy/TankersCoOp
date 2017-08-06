using UnityEngine;
using System.Collections;

public class TankEnemyRed : TankEnemy
{
    // State variables
    private float coolDownEnd = 4f;             // The length of time the tank stays aggressive without seeing the player.
    private float coolDownTimer;                // The length of time the tank has gone without seeing the player.

    // Shooting variables
    private float burstFireFreq = 2.5f;         // The amount of time between burst fires.
    protected float fireBurstFreq = .15f;       // The amount of time between each individual shot during burst fire.
    private float burstAccuracy = 15f;          // The max amount of degrees a burst shot can miss the player by.

    // Driving variables
    private float stopDistance = 20f;           // The distance the tank gets to the player tank before stopping.
    private float speedAggressive = 12f;        // The speed the tank drives at.


    new void Awake()
    {
        base.Awake();
        // This needs to be called in awake so that it is instantiated earlier than GUI_HUD.
        tankColor = tankColors[2];
        ColorizeTank();
    }

    private new void Start()
    {
        base.Start();

        // Set projectile.
        projectile = Resources.Load("TankResources/Projectile/ShellEnemyRed") as GameObject;

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
                case State.CHASEAGGRESSIVE:
                    ChaseAggressive();
                    break;
                case State.FIGHTAGGRESSIVE:
                    FightAggressive();
                    break;
                case State.EXPLORE:
                    Explore();
                    break;
            }
            yield return null;
        }
    }



    /*
    Functions for the ChaseAggressive state:
    setToChaseAggressive() - Set the state to CHASEAGGRESSIVE and change variables accordingly.
    setToChaseAggressiveNoCD() - Same as setToChaseAggressive but doesn't reset coolDownTimer.
    driveAggressive() - Drives directly at player if far enough away.
    selectDirectionDirect() - Sets targetDirectionDrive to directly at player.
    coolDown() - Change tank to different state if the player hasn't been seen in coolDownEnd seconds.
    playerInSight() - Returns true if player is in sight.
    burstFire() - Starts a coroutine to change the state to FightAggressive
    */
    protected void ChaseAggressive()
    {
        driveAggressive();
        aimDirect();
    }
    protected void setToChaseAggressive()
    {
        Debug.Log("chaseAggressive");
        state = TankEnemy.State.CHASEAGGRESSIVE;

        // Change the drive speed.
        m_CurrentSpeed = speedAggressive;

        // Start timer until next burst shot.
        StartCoroutine(burstFire());
    }
    private void driveAggressive()
    {
        // Rotate the car toward targetDirection.
        selectDirectionDirect();
        rotateDirection();

        // Drive toward targetDirection if farther than stopDistance from player1.
        if (Vector3.Distance(transform.position, player1.transform.position) > stopDistance)
        {
            driveDirection();
        }
    }
    private void selectDirectionDirect()
    {
        targetDirectionDrive = vectorTowardPlayer1;
    }
    private IEnumerator coolDown()
    {
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
        setToExplore();
    }
    protected bool playerInSight()
    {
        RaycastHit hit;
        return (Physics.Raycast(tower.position, -tower.forward, out hit, roomLength) && hit.transform.tag == playerTag);
    }
    private IEnumerator burstFire()
    {
        yield return new WaitForSeconds(burstFireFreq);
        // Change states.
        if (state == TankEnemy.State.CHASEAGGRESSIVE)
        {
            setToFightAggressiveNoCD();
        }
    }


    /*
    Functions for the FightAggressive state:
    setToFightAggressive() - Set the state to CHASEAGGRESSIVE and change variables accordingly.
    fireBurst() - Shoots accurately until out of projectiles.
    scanToBurst() - Scans toward the targetDirectionAim and fires.
    selectDirectionAimBurst() - Selects targetDirectionAim to be at most burstAccuracy away from the vector toward the player.
    */
    protected void FightAggressive()
    {
        driveAggressive();
        fireBurst();
    }
    protected void setToFightAggressive()
    {
        Debug.Log("fightAggressive");
        state = TankEnemy.State.FIGHTAGGRESSIVE;

        // Change the drive speed.
        m_CurrentSpeed = speedAggressive;
        
        // Start cool down timer.
        //StartCoroutine(coolDown());

        // Set variables
        canFire = true;
        fireFreq = fireBurstFreq;
        targetDirectionAim = vectorTowardPlayer1;
    }
    protected void setToFightAggressiveNoCD()
    {
        Debug.Log("fightAggressive no cooldown");
        state = TankEnemy.State.FIGHTAGGRESSIVE;

        // Change the drive speed.
        m_CurrentSpeed = speedAggressive;
    }
    private void fireBurst()
    {
        scanToBurst();

        if (projectileCount <= 0)
        {
            setToChaseAggressive();
        }
    }
    private void scanToBurst()
    {
        if (canFire)
        {
            selectDirectionAimBurst();
        }

        float step = towerRotateSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(tower.forward, -targetDirectionAim, step, .01F);
        tower.rotation = Quaternion.LookRotation(newDir);

        if (tower.forward == -targetDirectionAim && canFire)
        {
            Fire();
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

    /*
    Functions for Explore state:
    exploreCS() - 
    playerIsVisible() - Returns true if player is visible from tank position.
    */
    protected override void exploreCS()
    {
        if (playerIsVisible())
        {
            setToFightAggressive();
        }
    }
    private bool playerIsVisible()
    {
        Debug.Log("player is visible");
        RaycastHit hit;
        return (Physics.Raycast(tower.position, vectorTowardPlayer1, out hit, roomLength) && hit.transform.tag == playerTag);
    }
}