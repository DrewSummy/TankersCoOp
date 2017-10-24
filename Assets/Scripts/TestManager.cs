using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class TestManager : MonoBehaviour {


    public List<GameObject> teamA;                                         // A list of gameobjects representing tanks on team A.
    public List<GameObject> teamB;                                         // A list of gameobjects representing tanks on team B.
    private List<GameObject> teamAInstance = new List<GameObject>();       // A list of gameobject instances representing tanks on team A.
    private List<GameObject> teamBInstance = new List<GameObject>();       // A list of gameobject instances representing tanks on team B.
    public List<Transform> spawnLocations = new List<Transform>();         // A list of transforms representing spawn locations.
    public Transform projectileHolder;                                     // A transform for the projectile holder object.
    public Transform testRoom;                                             // A transform to hold the room object.
    public Transform tankHolder;
    
    // Use this for initialization
    void Start()
    {
        // Place tanks
        InitializeTanks();

        // Assign targets
        AssignTargets();

        // Enable tanks
        EnableTanks();
    }

    // Initialize the tank instances and add them to the correct lists.
    private void InitializeTanks()
    {
        List<GameObject> tempTanks = new List<GameObject>();
        tempTanks.AddRange(teamA);
        tempTanks.AddRange(teamB);

        List<Transform> currentSpawnLocations = spawnLocations;
        
        while (tempTanks.Count > teamA.Count)
        {
            // Get tank, remove from list.
            int tankIndex = Random.Range(0, tempTanks.Count);
            GameObject t = Instantiate(tempTanks[tankIndex]) as GameObject;
            tempTanks.RemoveAt(tankIndex);
            t.transform.SetParent(testRoom);

            // Add t to teamAInstance to enable later.
            teamAInstance.Add(t);

            // Place in location, remove from list.
            int locationIndex = Random.Range(0, currentSpawnLocations.Count);
            t.transform.position = currentSpawnLocations[locationIndex].position;
            currentSpawnLocations.RemoveAt(locationIndex);

            // Set leftover projectiles.
            t.GetComponent<Tank>().SetLeftoverProjectileHolder(projectileHolder);
        }

        while (tempTanks.Count > 0)
        {
            // Get tank, remove from list.
            int tankIndex = Random.Range(0, tempTanks.Count);
            GameObject t = Instantiate(tempTanks[tankIndex]) as GameObject;
            t.name = "wooh";
            tempTanks.RemoveAt(tankIndex);
            t.transform.SetParent(testRoom);

            // Add t to teamBInstance to enable later.
            teamBInstance.Add(t);

            // Place in location, remove from list.
            int locationIndex = Random.Range(0, currentSpawnLocations.Count);
            t.transform.position = currentSpawnLocations[locationIndex].position;
            currentSpawnLocations.RemoveAt(locationIndex);

            // Set leftover projectiles.
            t.GetComponent<Tank>().SetLeftoverProjectileHolder(projectileHolder);
        }
    }

    // Go through each teamInstance and set the targets to each other.
    private void AssignTargets()
    {
        for (int t = 0; t < teamAInstance.Count; t++)
        {
            Debug.Log(teamAInstance[t]);
            teamAInstance[t].GetComponent<Tank>().targets = teamBInstance;
        }
        for (int t = 0; t < teamBInstance.Count; t++)
        {
            teamBInstance[t].GetComponent<Tank>().targets = teamAInstance;
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
            tempTanks[tank].GetComponent<TankEnemy>().startTankEnemy();
        }
    }
}