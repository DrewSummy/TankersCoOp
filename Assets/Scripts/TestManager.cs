using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour {


    public List<GameObject> teamA;                                         // Tanks on team A.
    public List<GameObject> teamB;                                         // Tanks on team B.
    public List<Transform> spawnLocations = new List<Transform>();         // A list of spawn locations.
    public Transform projectileHolder;                                     // A variable to store a reference to the transform of the projectile holder object.

    
    // Use this for initialization
    void Start ()
    {
        // Place Tanks
        PlaceTanks();

        // Assign targets
        AssignTargets();

        // Enable tanks.
        EnableTanks();
	}


    private void PlaceTanks()
    {
        List<GameObject> allTanks = new List<GameObject>();
        allTanks.AddRange(teamA);
        allTanks.AddRange(teamB);

        List<Transform> currentSpawnLocations = spawnLocations;

        for (int tank = 0; tank < allTanks.Count; tank++)
        {
            // Get tank, remove from list.
            int tankIndex = Random.Range(0, allTanks.Count);
            GameObject enemy = Instantiate(allTanks[tankIndex]) as GameObject;
            allTanks.RemoveAt(tankIndex);

            // Place in location, remove from list.
            int locationIndex = Random.Range(0, currentSpawnLocations.Count);
            enemy.transform.position = currentSpawnLocations[locationIndex].position;
            currentSpawnLocations.RemoveAt(locationIndex);

            // Set leftover projectiles.
            enemy.GetComponent<Tank>().SetLeftoverProjectileHolder(projectileHolder);
        }
    }

    private void AssignTargets()
    {
        for (int tank = 0; tank < teamA.Count; tank++)
        {
            //teamA[tank].GetComponent<Tank>().targets = teamB;
        }

        for (int tank = 0; tank < teamB.Count; tank++)
        {
            //teamA[tank].GetComponent<Tank>().targets = teamA;
        }
    }

    private void EnableTanks()
    {
        List<GameObject> allTanks = new List<GameObject>();
        allTanks.AddRange(teamA);
        allTanks.AddRange(teamB);
        
        for (int tank = 0; tank < allTanks.Count; tank++)
        {
            allTanks[tank].GetComponent<TankEnemy>().enabled = true;
            allTanks[tank].GetComponent<TankEnemy>().startTankEnemy();
        }
    }
}
