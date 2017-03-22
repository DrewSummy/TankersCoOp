using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using System.Collections;
using UnityEngine.UI;

namespace Completed

{

    public class LevelManager : MonoBehaviour
    {
        //TODO: make the camera an object controlled by the levelManager

        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class Count
        {
            public int minimum;             //Minimum value for our Count class.
            public int maximum;             //Maximum value for our Count class.


            //Assignment constructor.
            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }


        public GameObject[] enemies;                                             // Array of enemy prefabs.
        public GameObject[] courses;                                             // Array of obstacle course prefabs.
        public Material[] floorMaterials;                                        // Array of materials prefabs for the floor.
        public Material[] blockMaterials;                                        // Array of materials prefabs for the blocks.
        public GameObject exit;                                                  // The GameObject of the exit.
        public GameObject blockTall;                                             // The GameObject of the tall block.
        public GameObject blockShort;                                            // Store a reference to our RoomManager which will set up the room.
        public RoomManager m_roomScript;                                         // The GameObject of the exit.
        public GameMaster GameScript;                                            // Reference to the parent tank game object.
        public bool coop;// = false;                                                // Reference to the parent tank game object.
        public GameObject m_camera;                                              // Reference to the parent tank game object.
        public Transform panel;

        public bool[,] floorChart = new bool[11, 11];                            // An array of arrays of bools if a room is enabled.
        private Transform[,] roomGrid = new Transform[11, 11];                   // An array of arrays of transforms for each room.
        private int Level;                                                       // Current level number.
        public Vector2 firstRoomCoordinate;                                      // A Vector2 for the first room.
        public Vector2 lastRoomCoordinate;                                       // A Vector2 for the last room.
        private Transform roomHolder;                                            // A variable to store a reference to the transform of the Room object.
        private Transform obstacleHolder;                                        // A variable to store a reference to the transform of the Obstacle object.
        private List<float> textureDistributions = new List<float>();            // A list of likelihoods for each obstacle to show up depending on the level.
        private List<Vector3> spawnPositions = new List<Vector3>();              // A list of possible locations to place spawns.
        private int m_RoomLength = 50;                                           // Length of each room declared elsewhere also.
        private float wallThickness = 1f;                                        // Thickness of outside walls.
        private float blockThickness = 2.5f;                                     // Thickness of blocks.
        private int numberOfRooms;                                               // Number of rooms based on level
        private string blockTag = "Block";                                       // String to apply the tag on blocks.
        private string playerSpawnTag = "PlayerSpawn";                           // String to apply the tag on player spawns.
        private string enemySpawnTag = "EnemySpawn";                             // String to apply the tag on enemy spawns.
        private string exitTag = "Exit";                                         // String to apply the tag on the exit.
        private Transform currentRoom;                                           // The transform for the current room.
        private GameObject player1;                                              // Reference to the player 1 game object.
        private GameObject player2;
        //private Vector3 player2Position;
        private bool needToStartRoom = true;                                     // Boolean of whether the first room needs to start. TODO: should be obsolete
        private int playersLeft;

        public GameMaster GM;


        // GUI stuff
        public GUI_Pause pause;                                                  // Reference to the GUI for pausing the game.
        public List<Vector2> roomsVisited;
        public List<Vector2> roomsUnvisited;
        //firstRoomCoordinate;
        //lastRoomCoordinate;


        // Helper function to print out room grid and see what it looks like.
        private void PrintRoomGrid()
        {
            for (int row = 0; row < roomGrid.GetLength(0); row++)
            {
                // create a string of x's and o's to represent enabled and unenabled rooms
                string printThis = "";
                for (int column = 0; column < roomGrid.GetLength(1); column++)
                {
                    if (roomGrid[row, column] != null)
                    {
                        printThis += "x";
                    }
                    else
                    {
                        printThis += "o";
                    }
                }
                Debug.Log(printThis);
            }
        }


        public void FindOccupiedRoom()
        {
            // Do some math and get the room coordinate player1 is in.
            int stepLength = m_RoomLength + 2 * (int)wallThickness;
            int xCoord = (int)Mathf.Floor((player1.transform.position.x + wallThickness) / stepLength);
            int yCoord = (int)Mathf.Floor((player1.transform.position.z + wallThickness) / stepLength);

            currentRoom = roomGrid[xCoord, yCoord];
        }


        private Transform[] FindCurrentNEWS()
        {
            Transform[] NEWS = new Transform [4];

            int stepLength = m_RoomLength + 2 * (int)wallThickness;
            int xCoord = (int)Mathf.Floor((player1.transform.position.x + wallThickness) / stepLength);
            int yCoord = (int)Mathf.Floor((player1.transform.position.z + wallThickness) / stepLength);

            if (floorChart[xCoord, yCoord + 1])
            {
                NEWS[0] = roomGrid[xCoord, yCoord + 1];
            }
            if (floorChart[xCoord + 1, yCoord])
            {
                NEWS[1] = roomGrid[xCoord + 1, yCoord];
            }
            if (floorChart[xCoord - 1, yCoord])
            {
                NEWS[2] = roomGrid[xCoord - 1, yCoord];
            }
            if (floorChart[xCoord, yCoord - 1])
            {
                NEWS[3] = roomGrid[xCoord, yCoord - 1];
            }

            return NEWS;
        }

        
        public Transform[] SendNEWS(Transform room)
        {
            Transform[] NEWS = new Transform[4];

            int stepLength = m_RoomLength + 2 * (int)wallThickness;
            int xCoord = (int)Mathf.Floor((room.transform.position.x + wallThickness) / stepLength);
            int yCoord = (int)Mathf.Floor((room.transform.position.z + wallThickness) / stepLength);

            if (floorChart[xCoord, yCoord + 1])
            {
                NEWS[0] = roomGrid[xCoord, yCoord + 1];
            }
            if (floorChart[xCoord + 1, yCoord])
            {
                NEWS[1] = roomGrid[xCoord + 1, yCoord];
            }
            if (floorChart[xCoord - 1, yCoord])
            {
                NEWS[2] = roomGrid[xCoord - 1, yCoord];
            }
            if (floorChart[xCoord, yCoord - 1])
            {
                NEWS[3] = roomGrid[xCoord, yCoord - 1];
            }

            return NEWS;

        }

        // TODO: might be used to keep track of the room the player is in.
        public Transform SendCurrentRoom()
        {
            FindOccupiedRoom();
            return currentRoom;
        }

        // Clears our array floorChart and prepares it to generate a new floor.
        private void InitializeList()
        {
            // Set floorChart to all false and roomGrid to null.
            for (int row = 0; row < floorChart.GetLength(0); row++)
            {
                for (int column = 0; column < floorChart.GetLength(1); column++)
                {
                    floorChart[row, column] = false;
                }
            }            
        }

        // Enable some rooms in the floorChart based on the level.
        private void InstantiateFloorChart(int level)
        {
            // Create an int for number of rooms based on level.
            numberOfRooms = 3 + level + Random.Range(0, level);
            if (numberOfRooms > floorChart.GetLength(0) * floorChart.GetLength(1))
            {
                numberOfRooms = floorChart.GetLength(0) * floorChart.GetLength(1);
            }
            numberOfRooms = 2;

            // Enable a random first room.
            firstRoomCoordinate = new Vector2(Random.Range(0, floorChart.GetLength(0)), Random.Range(0, floorChart.GetLength(0)));
            floorChart[(int)firstRoomCoordinate.x, (int)firstRoomCoordinate.y] = true;


            // Go through all but the last room and instantiate them.
            for (int rooms = 1; rooms < numberOfRooms; rooms++)
            {
                // Place random room.
                int randomRowIndex = Random.Range(0, floorChart.GetLength(0));
                int randomColumnIndex = Random.Range(0, floorChart.GetLength(1));
                // If room is already enabled, rooms-- and keep searching.
                if (IsEnabled(randomRowIndex, randomColumnIndex))
                {
                    rooms--;
                }
                // If room has enabled neighbors, enable room.
                else if (IsEnabled(randomRowIndex - 1, randomColumnIndex) ||
                    IsEnabled(randomRowIndex + 1, randomColumnIndex) ||
                    IsEnabled(randomRowIndex, randomColumnIndex - 1) ||
                    IsEnabled(randomRowIndex, randomColumnIndex + 1))
                {
                    // If it is the last room keep track of it to put the exit.
                    if (rooms == numberOfRooms - 1)
                    {
                        lastRoomCoordinate = new Vector2(randomRowIndex, randomColumnIndex);
                    }
                    floorChart[randomRowIndex, randomColumnIndex] = true;
                }
                // No neighboring rooms, rooms-- and keep searching.
                else
                {
                    rooms--;
                }
            }
        }

        // Helper function for InstantiateFloorChart
        private bool IsEnabled(int row, int column)
        {
            // If the point is in bounds and enabled return true.
            if ((0 <= row && row < floorChart.GetLength(0) && 0 <= column && column < floorChart.GetLength(1)))
            {
                if (floorChart[row, column] == true)
                {
                    return true;
                }
                // Else the point is enabled return false.
                else
                {
                    return false;
                }
            }
            // Else the point is out of bounds return false.
            else
            {
                return false;
            }
        }

        // Helper function to print out floor chart and see what it looks like.
        private void PrintFloorChart()
        {
            for (int row = 0; row < floorChart.GetLength(0); row++)
            {
                // create a string of x's and o's to represent enabled and unenabled rooms
                string printThis = "";
                for (int column = 0; column < floorChart.GetLength(1); column++)
                {
                    if (floorChart[row, column])
                    {
                        printThis += "x";
                    }
                    else
                    {
                        printThis += "o";
                    }
                }
                Debug.Log(printThis);
            }
        }

        // Sets up each room dependent on floorChart.
        private void SetUpFloor(int level)
        {
            // Set up the floor chart.
            InstantiateFloorChart(level);

            // Iterate through the floorChart and place rooms when enabled.
            for (int row = 0; row < floorChart.GetLength(0); row++)
            {
                for (int column = 0; column < floorChart.GetLength(1); column++)
                {
                    if (floorChart[row, column])
                    {
                        // Create a game object roomHolder for every enabled room in floorChart.
                        roomHolder = new GameObject("Room #" + (floorChart.GetLength(0) * column + row)).transform;
                        roomHolder.position = new Vector3(row * (m_RoomLength + 2 * wallThickness), 0, column * (m_RoomLength + 2 * wallThickness));

                        // Update the grid.
                        roomGrid[row, column] = roomHolder;

                        // Add the script to roomHolder.
                        roomHolder.gameObject.AddComponent<RoomManager>();

                        // Set the coordinate.
                        roomHolder.gameObject.GetComponent<RoomManager>().coordinate = new Vector2(row, column);

                        // Set coop.
                        roomHolder.gameObject.GetComponent<RoomManager>().coop = coop;

                        // If this is the starting room and level 1, TODO: place the instructions as the obstacle course.
                        if (new Vector2(row, column) == firstRoomCoordinate && level == 1)
                        {
                            // Call the RoomManager function for a room with instructions and instantiate respective variables.
                            roomHolder.GetComponent<RoomManager>().SetUpRoom
                                (roomHolder, Level, blockMaterials, floorMaterials, NEWSWall(row, column), GM);
                            roomHolder.GetComponent<RoomManager>().CreateStartingRoomWithInstructions();
                            roomHolder.GetComponent<RoomManager>().levelScript = this;
                            roomHolder.GetComponent<RoomManager>().m_camera = m_camera;
                        }
                        // If this is the starting room, place nothing.
                        //TODO: could be vector2
                        else if (new Vector2(row, column) == firstRoomCoordinate)
                        {
                            // Call the RoomManager function for an empty room and instantiate respective variables.
                            roomHolder.GetComponent<RoomManager>().SetUpRoom
                                (roomHolder, Level, blockMaterials, floorMaterials, NEWSWall(row, column), GM);
                            roomHolder.GetComponent<RoomManager>().CreateStartingRoom();
                            roomHolder.GetComponent<RoomManager>().levelScript = this;
                            roomHolder.GetComponent<RoomManager>().m_camera = m_camera;

                        }
                        // If this is the last room, place the exit. Otherwise place an obstacle course.
                        else if (new Vector2(row, column) == lastRoomCoordinate)
                        {
                            // Call the RoomManager function for the last room with an exit and instantiate respective variables.
                            roomHolder.GetComponent<RoomManager>().SetUpRoom
                                (roomHolder, Level, blockMaterials, floorMaterials, NEWSWall(row, column), GM);
                            roomHolder.GetComponent<RoomManager>().CreateLastRoom();
                            roomHolder.GetComponent<RoomManager>().levelScript = this;
                            roomHolder.GetComponent<RoomManager>().m_camera = m_camera;
                            roomHolder.GetComponent<RoomManager>().isLastRoom = true;

                            roomHolder.GetComponent<RoomManager>().disappearRoom();
                        }
                        else
                        {
                            // Call the RoomManager function for a room with an obstacle course.
                            roomHolder.GetComponent<RoomManager>().SetUpRoom
                                (roomHolder, Level, blockMaterials, floorMaterials, NEWSWall(row, column), GM);
                            roomHolder.GetComponent<RoomManager>().CreateObstacleCourse();
                            roomHolder.GetComponent<RoomManager>().levelScript = this;
                            roomHolder.GetComponent<RoomManager>().m_camera = m_camera;
                            
                            roomHolder.GetComponent<RoomManager>().disappearRoom();

                        }
                        // Give each room the roomCoord.
                        int[] toSend = new int[2];
                        toSend[0] = row;
                        toSend[1] = column;
                        roomHolder.GetComponent<RoomManager>().roomCoord = toSend;
                    }
                }
            }

            // Now pass NEWSRoom to each room.
            for (int row = 0; row < floorChart.GetLength(0); row++)
            {
                for (int column = 0; column < floorChart.GetLength(1); column++)
                {
                    if (floorChart[row, column])
                    {
                        int[] check = new int[2];
                        check[0] = row;
                        check[1] = column;
                        Transform[] toPass = new Transform[4];
                        if (column < floorChart.GetLength(1) - 1)
                        {
                            if (floorChart[row, column + 1])
                            {
                                toPass[0] = roomGrid[row, column + 1];
                            }
                        }
                        if (row < floorChart.GetLength(0) - 1)
                        {
                            if (floorChart[row + 1, column])
                            {
                                toPass[1] = roomGrid[row + 1, column];
                            }
                        }
                        if (row > 0)
                        {
                            if (floorChart[row - 1, column])
                            {
                                toPass[2] = roomGrid[row - 1, column];
                            }
                        }
                        if (column > 0)
                        {
                            if (floorChart[row, column - 1])
                            {
                                toPass[3] = roomGrid[row, column - 1];
                            }
                        }


                        roomGrid[row, column].GetComponent<RoomManager>().PassNEWSRooms(toPass);
                    }
                }
            }
        }

        // Creates the array of bools to place open and closed walls.
        private bool[] NEWSWall(int row, int column)
        {
            bool[] NEWSValue = new bool[4];

            for (int wall = 0; wall < NEWSValue.Length; wall++)
            {
                NEWSValue[wall] = false;
            }

            if (IsEnabled(row, column + 1))
            {
                NEWSValue[0] = true;
            }
            if (IsEnabled(row + 1, column))
            {
                NEWSValue[1] = true;
            }
            if (IsEnabled(row - 1, column))
            {
                NEWSValue[2] = true;
            }
            if (IsEnabled(row, column - 1))
            {
                NEWSValue[3] = true;
            }

            return NEWSValue;
        }

        // SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene(int level)
        {
            // Get the players.
            playersLeft = 1;
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (tank.GetComponent<TankPlayer>().m_PlayerNumber == 1)
                {
                    player1 = tank;
                }
                if (coop)
                {
                    playersLeft = 2;
                    if (tank.GetComponent<TankPlayer>().m_PlayerNumber == 2)
                    {
                        player2 = tank;
                    }
                }
            }

            // Reset our list of spawn positions and clear floorChart.
            InitializeList();

            // Sets up each room dependent on floorChart.
            SetUpFloor(level);
            

            pause.enabled = true;

            
            // Equate this to the special floorChart for the GUI minimap
            // Initiate GUI.
            GameObject.FindGameObjectWithTag("MiniMap").GetComponent<GUI_MiniMap>().beginMap(floorChart, lastRoomCoordinate, firstRoomCoordinate);
            

            // Set the first room coordinates.
            Transform firstRoom = roomGrid[(int)firstRoomCoordinate.x, (int)firstRoomCoordinate.y];
            firstRoom.GetComponent<RoomManager>().startBeginningBattleCorrected();
            // Link the players to their room and eachother.
            player1.GetComponent<TankPlayer>().currentRoom = firstRoom.gameObject;
            if (coop)
            {
                player1.GetComponent<TankPlayer>().teammate = player2.GetComponent<TankPlayer>();
                player2.GetComponent<TankPlayer>().teammate = player1.GetComponent<TankPlayer>();
                player2.GetComponent<TankPlayer>().currentRoom = firstRoom.gameObject;
            }

            // Start the first room with the camera on it.
            //TODO: redundantly setting camera's tanks
            m_camera.GetComponent<CameraControl>().m_Player1 = player1;
            m_camera.GetComponent<CameraControl>().m_Player2 = player2;
            m_camera.GetComponent<CameraControl>().PlaceOnFirstRoom(firstRoomCoordinate);

            //TODO:fade from black to start
            StartCoroutine(fadeFromBlack());
        }


        // Fade panel to black.
        private IEnumerator fadeFromBlack()
        {
            // Decrement the alpha until invisible.
            float alpha = 1;
            float increment = .08f;
            Image imageAlpha = panel.GetComponent<Image>();
            imageAlpha.color = Color.black;
            while (alpha > 0)
            {
                alpha -= increment;
                imageAlpha.color = new Color(0, 0, 0, alpha);
                yield return new WaitForSeconds(.01f);
            }
            alpha = 0;
            imageAlpha.color = new Color(0, 0, 0, alpha);
        }

        public void startRoom(Transform room)
        {
            RoomManager RoomScript = room.GetComponent<RoomManager>();
            if (RoomScript.roomCompleted)
            {
                RoomScript.PlacePlayers();
            }
            else
            {
                RoomScript.battleBegin = true;
            }

        }

        // Called by player to decrease the player count and end game when there are no players.
        public void playerDied()
        {
            if (--playersLeft <= 0)
            {
                gameOver();
            }
            Debug.Log(playersLeft);
        }

        // Resets the player count when
        public void resetPlayers()
        {
            // Reset to the correct playersLeft.
            if (coop)
            {
                playersLeft = 2;
            }
            else
            {
                playersLeft = 1;
            }

            // If a player isn't alive, respawn the player.
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (!tank.GetComponent<TankPlayer>().alive)
                {
                    tank.GetComponent<TankPlayer>().respawn();
                }
            }
        }

        // Called when all plaayers hav died.
        private void gameOver()
        {
            //TODO: this should also create some GUI and disable pause
            Debug.Log("GameOver");
            m_camera.GetComponent<CameraControl>().gameOver = true;
        }
    }
}