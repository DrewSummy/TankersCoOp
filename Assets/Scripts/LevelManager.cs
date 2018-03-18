using UnityEngine;
using System;
using System.Collections.Generic;       // Allows us to use Lists.
using Random = UnityEngine.Random;      // Tells Random to use the Unity Engine random number generator.
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
        private int m_RoomLength = 50;                                           // Length of each room declared elsewhere also.
        private float wallThickness = 1f;                                        // Thickness of outside walls.
        private int numberOfRooms;                                               // Number of rooms based on level
        public Transform currentRoom;                                            // The transform for the current room.
        public GameObject player1;                                               // Reference to the player 1 game object.
        public GameObject player2;
        private int playersLeft;
        private Color colorMain;
        private Color colorAccent;

        public GameMaster GM;
        private GameObject levelHolder;


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

        

        private Transform[] FindCurrentNEWS()
        {
            // Find the alive player and use that.
            GameObject player = player1;
            if (!player1.GetComponent<TankPlayer>().alive)
            {
                player = player2;
            }

            Transform[] NEWS = new Transform [4];

            int stepLength = m_RoomLength + 2 * (int)wallThickness;
            int xCoord = (int)Mathf.Floor((player.transform.position.x + wallThickness) / stepLength);
            int yCoord = (int)Mathf.Floor((player.transform.position.z + wallThickness) / stepLength);

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
            // Determines numberOfRooms based on level.
            numberOfRooms = 3 + level + Random.Range(0, level);
            if (numberOfRooms > floorChart.GetLength(0) * floorChart.GetLength(1))
            {
                numberOfRooms = floorChart.GetLength(0) * floorChart.GetLength(1);
            }
            numberOfRooms = 28;

            // Enable a random first room.
            firstRoomCoordinate = new Vector2(Random.Range(0, floorChart.GetLength(0)), Random.Range(0, floorChart.GetLength(0)));
            //firstRoomCoordinate = new Vector2(0, 0);
            floorChart[(int)firstRoomCoordinate.x, (int)firstRoomCoordinate.y] = true;


            // Go through all but the last room and instantiate them.
            // Add the neighbors to validNextRoom.
            List<Vector2> validNextRooms = new List<Vector2>();
            validNextRooms = AddNeighbors(validNextRooms, firstRoomCoordinate);

            for (int rooms = 1; rooms < numberOfRooms - 1; rooms++)
            {
                // Get random, valid room.
                int randomRoomIndex = Random.Range(0, validNextRooms.Count);

                // Update floorChart and update validNextRooms.
                floorChart[(int)validNextRooms[randomRoomIndex].x, (int)validNextRooms[randomRoomIndex].y] = true;

                validNextRooms = AddNeighbors(validNextRooms, validNextRooms[randomRoomIndex]);
                validNextRooms.Remove(validNextRooms[randomRoomIndex]);

                // Set the lastRoomCoordinate.
                //TODO: fix this
                if (rooms == numberOfRooms - 2)
                {
                    floorChart[(int)validNextRooms[randomRoomIndex].x, (int)validNextRooms[randomRoomIndex].y] = true;
                    lastRoomCoordinate = validNextRooms[randomRoomIndex];
                }
            }
        }

        // Helper functions for InstantiateFloorChart. Determines the return value of floorChart[row, column].
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
        private bool IsNotEnabled(int row, int column)
        {
            // If the point is in bounds and disabled return true.
            if ((0 <= row && row < floorChart.GetLength(0) && 0 <= column && column < floorChart.GetLength(1)))
            {
                if (!floorChart[row, column])
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

        // Helper function for InstantiateFloorChart. This updates validNextRooms when a room is added.
        private List<Vector2> AddNeighbors(List<Vector2> vNR, Vector2 coord)
        {
            if (IsNotEnabled((int)coord.x - 1, (int)coord.y) && !vNR.Contains(new Vector2((int)coord.x - 1, (int)coord.y)))
            {
                vNR.Add(new Vector2((int)coord.x - 1, (int)coord.y));
            }
            if (IsNotEnabled((int)coord.x + 1, (int)coord.y) && !vNR.Contains(new Vector2((int)coord.x + 1, (int)coord.y)))
            {
                vNR.Add(new Vector2((int)coord.x + 1, (int)coord.y));
            }
            if (IsNotEnabled((int)coord.x, (int)coord.y - 1) && !vNR.Contains(new Vector2((int)coord.x, (int)coord.y - 1)))
            {
                vNR.Add(new Vector2((int)coord.x, (int)coord.y - 1));
            }
            if (IsNotEnabled((int)coord.x, (int)coord.y + 1) && !vNR.Contains(new Vector2((int)coord.x, (int)coord.y + 1)))
            {
                vNR.Add(new Vector2((int)coord.x, (int)coord.y + 1));
            }
            
            return vNR;
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

        // Retrieves the main and accent colors using the level.
        private void SetColors()
        {
            // Retrieve the colors and if main and accent are null, select random colors.
            string fileLoc = "Prefab/GameObjectPrefab/Room/RoomColors/" + Level.ToString();
            if (Resources.Load(fileLoc + "/main") as Material)
            {
                colorMain = (Resources.Load(fileLoc + "/main") as Material).color;
            }
            else
            {
                colorMain = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                Debug.Log("no");
            }
            if (Resources.Load(fileLoc + "/accent"))
            {
                colorAccent = (Resources.Load(fileLoc + "/accent") as Material).color;
            }
            else
            {
                colorAccent = Random.ColorHSV(1f, 1f, 1f, 1f, 0.5f, 1f);
            }
        }

        // Sets up each room dependent on floorChart.
        private void SetUpFloor(int level)
        {
            // Create the level GameObject.
            levelHolder = new GameObject("Level " + level);

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
                            currentRoom = roomHolder;
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
                            currentRoom = roomHolder;
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
                        roomHolder.GetComponent<RoomManager>().player1 = player1;
                        if (player2)
                        {
                            roomHolder.GetComponent<RoomManager>().player2 = player2;
                        }

                        roomHolder.SetParent(levelHolder.transform);
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

            // Get the main and accent color from resources depending on the level.
            SetColors();

            // Sets up each room dependent on floorChart.
            SetUpFloor(level);
            

            pause.enabled = true;

            
            // Equate this to the special floorChart for the GUI minimap
            // Initiate GUI.
            GameObject.FindGameObjectWithTag("MiniMap").GetComponent<GUI_MiniMap>().beginMap(floorChart, lastRoomCoordinate, firstRoomCoordinate, player1);
            

            // Set the first room coordinates.
            Transform firstRoom = roomGrid[(int)firstRoomCoordinate.x, (int)firstRoomCoordinate.y];
            // Link the players to their room and eachother.
            player1.GetComponent<TankPlayer>().currentRoom = firstRoom.gameObject;
            if (coop)
            {
                player1.GetComponent<TankPlayer>().teammate = player2.GetComponent<TankPlayer>();
                player2.GetComponent<TankPlayer>().teammate = player1.GetComponent<TankPlayer>();
                player2.GetComponent<TankPlayer>().currentRoom = firstRoom.gameObject;
            }
            m_camera.gameObject.SetActive(true);
            // Pass the colors.
            m_camera.GetComponent<CameraControl>().colorMain = colorMain;
            m_camera.GetComponent<CameraControl>().colorAccent = colorAccent;

            m_camera.GetComponent<CameraControl>().Initialize(firstRoom);
            m_camera.GetComponent<CameraControl>().m_Player1 = player1;
            m_camera.GetComponent<CameraControl>().m_Player2 = player2;

            /*
            // Set the players.
            m_camera.GetComponent<CameraControl>().m_Player1 = player1;
            if (coop)
            {
                m_camera.GetComponent<CameraControl>().m_Player2 = player2;
            }
            // Start the first room with the camera on it.
            */
            //m_camera.GetComponent<CameraControl>().PlaceOnFirstRoom(firstRoom);

            //TODO:fade from black to start
            StartCoroutine(fadeFromBlack());
            firstRoom.GetComponent<RoomManager>().startBeginningBattle();
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
        }
        public bool playersAlive()
        {
            return (playersLeft > 0);
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
                if (!tank.GetComponent<TankPlayer>().alive && coop)
                {
                    tank.GetComponent<TankPlayer>().respawn();
                }
            }
        }

        // Called when all players hav died.
        private void gameOver()
        {
            //TODO: this should also create some GUI and disable pause
            Debug.Log("GameOver");
            m_camera.GetComponent<CameraControl>().gameOverCamera();
            
            currentRoom.GetComponent<RoomManager>().endRoom();
        }

        // End level.
        public void endLevel()
        {
            InitializeList();

            Destroy(levelHolder);
        }
    }
}