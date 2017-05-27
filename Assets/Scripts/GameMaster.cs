using UnityEngine;
using System.Collections;
using System.Threading;

namespace Completed
{
    using System.Collections.Generic;       // Allows us to use Lists. 

    public class GameMaster : MonoBehaviour
    {

        public static GameMaster instance = null;                  // Static instance of GameMaster which allows it to be accessed by any other script.
        public GameObject player1;                                 // The object that player 1 controls.
        public GameObject player2;                                 // The object that player 2 controls.
        public GameObject ai;
        public GameObject camera;
        public GameObject menu;
        public GUI_Menu menuGUI;        // GUI
        public GUI_HUD HUDGUI;         // GUI
        public GUI_MiniMap minimapGUI;     // GUI
        public GUI_Pause pauseGUI;

        
        private LevelManager levelScript;                          // Store a reference to our LevelManager which will set up the level.
        private int Level = 1;                                     // Current level number.
        private int maxLevel = 5;                                  // Max level number.
        private bool coop = false;                                 // Makes a player 2 if true.

        // TODO: this is temporary, spawning players should be in initGame using sendPlayersPositions (i think)
        private Vector3 player1Position = new Vector3(285, 0, 285);// Position of player 1.
        private TankPlayer player1Script;
        private TankPlayer player2Script;
        private Vector3 aiTankPosition = new Vector3(295, 0, 295);
        //private Vector3 player2Position;                         // Position of player 2.
        private Transform playerHolder;                            // A variable to store a reference to the transform of the player object.
        private Transform enemyHolder;                             // A variable to store a reference to the transform of the enemy object.

        //Awake is always called before any Start functions
        void Awake()
        {
            // Check if instance already exists
            if (instance == null)

                // if not, set instance to this
                instance = this;

            // If instance already exists and it's not this:
            else if (instance != this)

                // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameMaster.
                Destroy(gameObject);

            // Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            // Get a component reference to the attached LevelManager script
            levelScript = GetComponent<LevelManager>();

            // Call the InitGame function to initialize the first level.
            //displaymenuGUI();

            //Temporary for testing.
            CreateSoloGame();
            //CreateCoopGame();
        }

        private void PlacePlayers()
        {
            // Create playerHolder to hold both players for organization.
            playerHolder = new GameObject("PlayerHolder").transform;

            // Load in the Tank being used from the Resources folder in assets.
            // Player1
            player1 = Resources.Load("TankResources/PlayerTank") as GameObject;
            GameObject Player1 = Instantiate(player1) as GameObject;
            Player1.transform.SetParent(playerHolder);
            player1.name = "Player1";
            player1Script = player1.GetComponent<TankPlayer>();
            player1Script.m_PlayerNumber = 1;

            // Player2
            if (coop)
            {
                // Load in the Tank being used from the Resources folder in assets.
                player2 = Resources.Load("TankResources/PlayerTank") as GameObject;
                GameObject Player2 = Instantiate(player2) as GameObject;
                Player2.transform.SetParent(playerHolder);
                player2.name = "Player2";
                player2Script = player2.GetComponent<TankPlayer>();
                player2Script.m_PlayerNumber = 2;
            }

            // Create playerHolder to hold both players for organization.
            enemyHolder = new GameObject("EnemyHolder").transform;
        }

        private void displaymenuGUI()
        {
            camera.SetActive(false);
            menu.SetActive(true);
            menuGUI.gameObject.SetActive(true);
            menuGUI.initialDisplay();
        }

        // Initializes the game for the first level.
        public void CreateSoloGame()
        {
            // Set coop.
            coop = false;
            
            // Place the players.
            PlacePlayers();

            camera.SetActive(true);
            menu.SetActive(false);
            menuGUI.gameObject.SetActive(false);
            // Create a thread to only start setting up the level when the GUIs are prepared.
            prepareGUIs();

            // Fade from black.
            createLevel();
        }
        public void CreateCoopGame()
        {
            // Set coop.
            coop = true;
            
            // Place the players.
            PlacePlayers();

            camera.SetActive(true);
            menu.SetActive(false);
            menuGUI.gameObject.SetActive(false);
            // Create a thread to only start setting up the level when the GUIs are prepared.
            prepareGUIs();

            // Fade from black.
            createLevel();
        }

        // Helper for CreateSoloGame().
        private void prepareGUIs()
        {
            // Enable the GUIs.
            HUDGUI.enableHUD();
            pauseGUI.enablePause();
            camera.SetActive(true);
        }

        // Helper for CreateSoloGame().
        private void createLevel()
        {
            // Call the SetupScene function of the LevelManager script, pass it current level number.
            levelScript.coop = coop;
            levelScript.m_camera = camera;
            levelScript.SetupScene(Level);

            // Pass the LevelManager to each tank.
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Player"))
            {
                tank.GetComponent<TankPlayer>().LM = levelScript;
            }
        }

        // Don't know what this is for.
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