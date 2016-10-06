using UnityEngine;
using System.Collections;

namespace Completed
{
    using System.Collections.Generic;       // Allows us to use Lists. 

    public class GameMaster : MonoBehaviour
    {

        public static GameMaster instance = null;                  // Static instance of GameMaster which allows it to be accessed by any other script.
        public GameObject player1;                                 // The object that player 1 controls.
        public GameObject ai;
        public GameObject camera;



        private LevelManager levelScript;                          // Store a reference to our LevelManager which will set up the level.
        private int Level = 1;                                     // Current level number.
        private int maxLevel = 5;                                  // Max level number.
        private bool coop = false;                                 // Makes a player 2 if true.
        private Vector3 player1Position = new Vector3(285, 0, 285);// Position of player 1.
        private TankPlayer player1Script;
        private Vector3 aiTankPosition = new Vector3(295, 0, 295);
        //private Vector3 player2Position;                         // Position of player 2.
        private Transform playerHolder;                            // A variable to store a reference to the transform of the player object.
        private Transform enemyHolder;                             // A variable to store a reference to the transform of the enemy object.

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Get a component reference to the attached LevelManager script
            levelScript = GetComponent<LevelManager>();

            //Call the InitGame function to initialize the first level 
            InitGame();
        }

        void Start()
        {
            // Create playerHolder to hold both players for organization.
            playerHolder = new GameObject("PlayerHolder").transform;
            // Load in the Tank being used from the Resources folder in assets.
            player1 = Resources.Load("PlayerTank") as GameObject;
            GameObject Player1 = Instantiate(player1) as GameObject;
            player1.transform.position = player1Position;
            Player1.transform.SetParent(playerHolder);
            player1.name = "Player1";
            player1Script = player1.GetComponent<TankPlayer>();

            // Create playerHolder to hold both players for organization.
            enemyHolder = new GameObject("EnemyHolder").transform;
        }
        //Initializes the game for each level.
        void InitGame()
        {
            // Set the bool coop of levelScript before setting up the scene.
            levelScript.coop = coop;

            // Pass the camera to the level.
            levelScript.m_camera = camera;

            // Call the SetupScene function of the LevelManager script, pass it current level number.
            levelScript.SetupScene(Level);
        }
        
        //Update is called every frame.
        public Vector3[] SendPlayerPositions()
        {

            Vector3[] SendPlayersPositions = new Vector3[2];
            SendPlayersPositions[0] = player1.transform.position;

            if (coop)
            {
                //SendPlayers[1] = player2.transform.position;
            }

            return SendPlayersPositions;
        }
    }
}