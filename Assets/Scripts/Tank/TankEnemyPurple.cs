using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TankEnemyPurple : TankEnemy
{

    // General variables
    List<Vector3> hitAngles = new List<Vector3>();
    List<float> hitWeights = new List<float>();
    float hitCount = 0;

    // State variables
    float snipeDelay = 1.0f;

    // Shooting variables
    //float shootChance = .8f;

    // Driving variables

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
        setToSnipe();
    }

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
        for (int tankI = targets.Count - 1; tankI >= 0; tankI--)
        {
            if (!targets[tankI])
            {
                targets.RemoveAt(tankI);
            }
        }
    }


    protected override IEnumerator FSM()
    {
        while (alive)
        {
            trackPlayer();
            switch (state)
            {
                case State.SNIPE:
                    Snipe();
                    break;
                case State.IDLE:
                    Idle();
                    break;
            }
            yield return null;
        }
    }

    /*
    Functions for the SNIPE state:
    setToSnipe() - 
    setBackToSnipe() -
    findShot() - 
    selectDirectionAim() - 
    selectWeightedTarget() - Selects a random target direction based on weights.

    */
    protected void Snipe()
    {
        findShot();
    }
    private void setToSnipe()
    {
        state = TankEnemy.State.SNIPE;
        targetDirectionAim = Vector3.zero;

        needDirection = true;
    }
    private IEnumerator setBackToSnipe()
    {
        setToIdle();
        yield return new WaitForSeconds(snipeDelay);
        setToSnipe();
    }

    private void findShot()
    {
        selectDirectionAim();
        aimDirection();
        if (-tower.forward == targetDirectionAim)
        {
            Fire();
            needDirection = true;
        }

        selectDirectionAim();
    }
    private void selectDirectionAim()
    {
        if (needDirection)
        {
            // Goes through every possible shot and records everyone that hits a player tank.
            needDirection = false;
            hitAngles.Clear();
            hitWeights.Clear();
            hitCount = 0;
            float eps = .1f;
            float angle = 0;
            Vector3 testShot = Vector3.forward;

            while (angle < 360)
            {
                float weight = projTestScript.beginShoot(tower.position, testShot, false);

                // If there was a hit, record it.
                if (weight > -1)
                {
                    hitAngles.Add(testShot);
                    hitWeights.Add(weight);
                    hitCount += 10f;
                }

                angle += eps;
                testShot = new Vector3(-Mathf.Sin(angle * Mathf.PI / 180), 0, Mathf.Cos(angle * Mathf.PI / 180));
            }

            // Set targetDirectionAim if hits isn't empty.
            if (hitAngles.Count != 0)
            {
                //targetDirectionAim = hitAngles[Random.Range(0, hitAngles.Count)];
                targetDirectionAim = selectWeightedTarget();
            }
            else
            {
                needDirection = true;

                // Change to idle and set a coroutine to set back to snipe.
                //StartCoroutine(setBackToSnipe());
            }
        }
    }
    private Vector3 selectWeightedTarget()
    {
        float counter = Random.Range(0, hitCount);
        int counterI = 0;
        counter -= hitWeights[counterI];
        while (counter > 0)
        {
            counterI++;
            counter -= hitWeights[counterI];
        }
        return hitAngles[counterI];
    }

    /*
    Functions for the IDLE state:
    Idle() - Overrides the change Idle funciton to look around randomly.
    */
    protected override void Idle()
    {
        // Never change states
        findNoShoot();
    }
    private void findNoShoot()
    {
        aimRandom();
        aimDirection();
        if (-tower.forward == targetDirectionAim)
        {
            needDirection = true;
        }
    }
    private void aimRandom()
    {
        if (needDirection)
        {
            needDirection = false;
            // Set targetDirection along the x-z plane and do so until a valid direction is selected.
            Vector2 unitPlane = Random.insideUnitCircle.normalized;
            targetDirectionAim.Set(unitPlane[0], 0, unitPlane[1]);
        }
    }
}