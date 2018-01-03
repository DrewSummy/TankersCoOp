using UnityEngine;
using Completed;
using System.Collections;
using System.Threading;

public class GameMaster : MonoBehaviour
{

    public static GameMaster instance = null;                  // Static instance of GameMaster which allows it to be accessed by any other script.
    public GameObject playerTemplate;                          // The game object for instantiating player tanks.
    public GameObject camera;
    public GameObject menu;
    public GUI_Menu menuGUI;                                   // GUI
    public GUI_HUD HUDGUI;                                     // GUI
    public GUI_MiniMap minimapGUI;                             // GUI
    public GUI_Pause pauseGUI;
    public GUI_Controller controllerGUI;



    private LevelManager levelScript;                          // Store a reference to our LevelManager which will set up the level.
    public int Level = 1;                                      // Current level number.
    private bool coop = false;                                 // Makes a player 2 if true.


    private GameObject player1;
    private GameObject player2;
    private TankPlayer player1Script;
    private TankPlayer player2Script;
    //private Vector3 player2Position;                         // Position of player 2.
    private Transform playerHolder;                            // A variable to store a reference to the transform of the player object.
    private string playerTeamName = "Player";                  // String to set the team of players.

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
        displaymenuGUI();

        //Temporary for testing.
        //CreateSoloGame();
        //CreateCoopGame();
    }

    private void PlacePlayers()
    {
        // Create playerHolder to hold both players for organization.
        playerHolder = new GameObject("PlayerHolder").transform;

        // Load in the Tank being used from the Resources folder in assets.
        // Player1
        player1 = Instantiate(playerTemplate) as GameObject;
        player1.transform.SetParent(playerHolder);
        player1.name = "Player1";
        player1Script = player1.GetComponent<TankPlayer>();
        player1Script.m_PlayerNumber = 1;
        player1.GetComponent<Tank>().teamName = playerTeamName;

        // Player2
        if (coop)
        {
            // Load in the Tank being used from the Resources folder in assets.
            player2 = Instantiate(playerTemplate) as GameObject;
            player2.transform.SetParent(playerHolder);
            player2.name = "Player2";
            player2Script = player2.GetComponent<TankPlayer>();
            player2Script.m_PlayerNumber = 2;
            player1.GetComponent<Tank>().teamName = playerTeamName;
        }
    }

    private void displaymenuGUI()
    {
        camera.SetActive(false);
        menu.SetActive(true);
        menuGUI.gameObject.SetActive(true);
        menuGUI.initialDisplay();

        controllerGUI.enabled = true;
    }

    // Initializes the game for the first level.
    public void CreateSoloGame()
    {
        // Set coop.
        coop = false;

        startGame();
    }
    public void CreateCoopGame()
    {
        // Set coop.
        coop = true;

        startGame();
    }

    public void startGame()
    {
        // Place the players.
        PlacePlayers();
        
        // Prepare the GUIs.
        prepareGUIs();

        // Fade from black.
        createLevel();
    }

    public void endGame()
    {
        clearGame();
        displaymenuGUI();
    }

    public void restart()
    {
        clearGame();
        //displaymenuGUI();

        startGame();
        //camera.SetActive(true);
    }

    private void clearGame()
    {
        camera.SetActive(false);

        levelScript.endLevel();

        Destroy(playerHolder.gameObject);

        // Reset the HUD so that the count down doesn't display.
        HUDGUI.resetHUD();
        minimapGUI.clearMap();
        //menuGUI.clearMenu();
}

    // Helper for CreateSoloGame().
    private void prepareGUIs()
    {
        // Enable the GUIs.
        menu.SetActive(false);
        menuGUI.gameObject.SetActive(false);
        HUDGUI.enableHUD();
        pauseGUI.enablePause();
        camera.SetActive(true);
    }

    // Helper for CreateSoloGame().
    private void createLevel()
    {
        // Disable the controller.
        controllerGUI.enabled = false;

        // Call the SetupScene function of the LevelManager script, pass it current level number.
        levelScript.coop = coop;
        levelScript.m_camera = camera;
        levelScript.SetupScene(Level);

        // Pass the LevelManager to each tank and each tank to the LevelManager.
        levelScript.player1 = player1;
        levelScript.player1.GetComponent<TankPlayer>().LM = levelScript;
        if (coop)
        {
            levelScript.player2 = player2;
            levelScript.player2.GetComponent<TankPlayer>().LM = levelScript;
        }
    }
}
