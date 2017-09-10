using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TankEnemyPurple : TankEnemy
{
    // General variables
    List<Vector3> hits = new List<Vector3>();

    // State variables

    // Shooting variables
    //new protected float fireFreq = 10f;
    //new protected int projectileAmount = 2;
    private bool needDirection = false;
    //TODO: chance of shot, figure out why the first shot doesn't happen immediately
    // value shots based on success

    // Driving variables


    protected override void resetVariables()
    {
        // Shooting variables
        fireFreq = 10f;
        projectileAmount = 2;
    }

    protected new void Start()
    {
        base.Start();

        // The FSM begins on Evade.
        setToSnipe();
    }


    private new IEnumerator FSM()
    {
        while (alive)
        {
            trackPlayer();
            switch (state)
            {
                case State.SNIPE:
                    Snipe();
                    break;
                case State.NOTHING:
                    Nothing();
                    break;
            }
            yield return null;
        }
    }

    /*
    Functions for the SNIPE state:
    setToSnipe() - 
    findShot() - 
    find() - 
    selectDirectionAim() - 
    */
    protected void Snipe()
    {
        findShot();


    }
    private void setToSnipe()
    {
        state = TankEnemy.State.SNIPE;

        //selectDirectionAim();
        needDirection = true;
    }
    protected void findShot()
    {
        //find all possible directions
        find();

        //shoot projectileTest in each
    }
    private void find()
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
            hits.Clear();
            float eps = .1f;
            float angle = 0;
            Vector3 testShot = Vector3.forward;

            while (angle < 360)
            {
                if (projTestScript.beginShoot(tower.position, testShot))
                {
                    hits.Add(testShot);
                }

                angle += eps;
                testShot = new Vector3(-Mathf.Sin(angle * Mathf.PI / 180), 0, Mathf.Cos(angle * Mathf.PI / 180));
            }

            // Set targetDirectionAim if hits isn't empty.
            if (hits.Count != 0)
            {
                targetDirectionAim = hits[Random.Range(0, hits.Count)];
            }
            else
            {
                needDirection = true;
            }
        }
    }
}