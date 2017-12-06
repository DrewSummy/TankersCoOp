using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using Completed;

public class TestManager : MonoBehaviour {


    public List<GameObject> teamA;                                         // A list of gameobjects representing tanks on team A.
    public List<GameObject> teamB;                                         // A list of gameobjects representing tanks on team B.
    public List<GameObject> teamAInstance = new List<GameObject>();       // A list of gameobject instances representing tanks on team A.
    public List<GameObject> teamBInstance = new List<GameObject>();       // A list of gameobject instances representing tanks on team B.
    public List<Transform> spawnLocations = new List<Transform>();         // A list of transforms representing spawn locations.
    public Transform projectileHolder;                                     // A transform for the projectile holder object.
    public Transform testRoom;                                             // A transform to hold the room object.
    public Transform tankHolder;
    private string teamAName = "A";
    private string teamBName = "B";
    public GUI_HUD HUDGUI;

    // Use this for initialization
    void Start()
    {
        // Place tanks
        InitializeTanks();

        // Assign targets
        AssignTargets();

        // Enable tanks
        EnableTanks();

        HUDGUI.enableHUD();
    }

    // Initialize the tank instances and add them to the correct lists.
    private void InitializeTanks()
    {
        // Go through each team and instantiate the tanks while placing them in random locations.
        List<Transform> currentSpawnLocations = spawnLocations;


        foreach (GameObject a in teamA)
        {
            // Get tank, remove from list.
            GameObject t = Instantiate(a) as GameObject;
            t.transform.SetParent(testRoom);

            // Add t to teamAInstance to enable later.
            teamAInstance.Add(t);

            // Place in location, remove from list.
            int locationIndex = Random.Range(0, currentSpawnLocations.Count);
            t.transform.position = currentSpawnLocations[locationIndex].position;
            currentSpawnLocations.RemoveAt(locationIndex);

            // Set leftover projectiles.
            t.GetComponent<Tank>().SetLeftoverProjectileHolder(projectileHolder);

            // Set the tags.
            if (t.GetComponent<TankEnemy>())
            {
                t.GetComponent<TankEnemy>().teamName = teamAName;
                t.GetComponent<TankEnemy>().enemyTeamName = teamBName;
            }
        }
        foreach (GameObject b in teamB)
        {
            // Get tank, remove from list.
            GameObject t = Instantiate(b) as GameObject;
            t.transform.SetParent(testRoom);

            // Add t to teamAInstance to enable later.
            teamBInstance.Add(t);

            // Place in location, remove from list.
            int locationIndex = Random.Range(0, currentSpawnLocations.Count);
            t.transform.position = currentSpawnLocations[locationIndex].position;
            currentSpawnLocations.RemoveAt(locationIndex);

            // Set leftover projectiles.
            t.GetComponent<Tank>().SetLeftoverProjectileHolder(projectileHolder);

            // Set the tags.
            if (t.GetComponent<TankEnemy>())
            {
                t.GetComponent<TankEnemy>().teamName = teamBName;
                t.GetComponent<TankEnemy>().enemyTeamName = teamAName;
            }
        }
    }

    // Go through each teamInstance and set the targets to each other.
    private void AssignTargets()
    {
        for (int t = 0; t < teamAInstance.Count; t++)
        {
            // Clear the list.
            teamAInstance[t].GetComponent<Tank>().targets.Clear();
            // Add targets with the helper function.
            deepCopyTeam(teamAInstance[t].GetComponent<Tank>(), teamBInstance);
        }
        for (int t = 0; t < teamBInstance.Count; t++)
        {
        
            teamBInstance[t].GetComponent<Tank>().targets.Clear();
            deepCopyTeam(teamBInstance[t].GetComponent<Tank>(), teamAInstance);
        }
    }
    // Helper function for assigning targets.
    private void deepCopyTeam(Tank tank, List<GameObject> team)
    {
        foreach (GameObject t in team)
        {
            tank.targets.Add(t);
        }
    }

    // Go through all tanks and call their start function.
    private void EnableTanks()
    {
        List<GameObject> tempTanks = new List<GameObject>();
        tempTanks.AddRange(teamAInstance);
        tempTanks.AddRange(teamBInstance);

        for (int tank = 0; tank < tempTanks.Count; tank++)
        {
            //tempTanks[tank].GetComponent<TankEnemy>().enabled = true;
            if (tempTanks[tank].GetComponent<TankEnemy>())
            {
                tempTanks[tank].GetComponent<TankEnemy>().startTankEnemy();
                tempTanks[tank].GetComponent<TankEnemy>().testEnvironment = true;
            }
            else
            {
                // Load in the Tank being used from the Resources folder in assets.
                // Player1
                tempTanks[tank].transform.SetParent(transform);
                tempTanks[tank].name = "Player1";
                tempTanks[tank].GetComponent<TankPlayerTest>().m_PlayerNumber = 1;
                //tempTanks[tank].GetComponent<Tank>().teamName = playerTeamName;
            }
        }
    }
}