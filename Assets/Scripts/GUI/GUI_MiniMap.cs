using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

namespace Completed
{
    public class GUI_MiniMap : MonoBehaviour
    {
        //TODO: figure out aspect ratio stuff http://answers.unity3d.com/questions/1065133/black-bars-on-top-bottom-in-aspect-ratio.html


        public GameObject panelSample;                                           // The GameObject with the sample panel.
        public GameObject panelFull;                                             // The GameObject with the full panel.
        
        public GameObject floorSample;                                           // The GameObject with the sample floor image.
        public GameObject unknownSample;                                         // The GameObject with the sample unknown room image.

        public GameObject floorFull;                                             // The GameObject with the full floor image.
        public GameObject unknownFull;                                           // The GameObject with the full unknown room image.

        public GameObject player;                                                // The GameObject with the player sprite.
        public GameObject playerSample;                                          // The GameObject with the sample player image.
        private GameObject playerIndicator;
        public GameObject exit;                                                  // The GameObject with the exit sprite.
        public LevelManager levelScript;                                         // Store a reference to the LevelManager which will set up the level.

        private int[,] floorChart = new int[11, 11];                             // An array of arrays of ints representing room statuses.
        private Transform[,] floorsFull = new Transform[11, 11];
        private RectTransform floorHolderFull;                                       // edit A variable to store a reference to the transform of the floor object.
        private RectTransform floorHolderSample;                                       // edit A variable to store a reference to the transform of the floor object.
        private RectTransform itemHolderFull;                                        // edit A variable to store a reference to the transform of the outline object.
        

        private GameObject P1;                                                   // Reference to the player 1 game object.
        private Vector2 lastRoom;
        private Vector2 startRoom;
        private int roomLengthFull = 60;                                             // Length of each full room image on the canvas. Equal to the length of the outlineFull.
        private int roomLengthSample = 28;                                             // Length of each sample room image on the canvas. Equal to the length of the outlineSample.
        private int m_RoomLength = 50;                                           // Length of each room declared elsewhere also.
        private float wallThickness = 1f;                                        // Thickness of outside walls.
        private Vector2 playerCoord;                                             // Reference to coordinate of players position.
        private Vector3 mapOffset = new Vector3(80, 100, 0);                                         // Amount to scale by on neighborless wall. Proportional to outlineBorderFull.
        private bool selected = false;
        private int sampleRadius = 1;

        private GameObject tank; // Instance of player.

        private Vector2 topRight;                                                 //TODO: keeps track of the top and right most coordinate to adjust the full map to be in the furthest top right position


        private void callAwake()
        {
            // Create the full floor holder.
            floorHolderFull = new GameObject("Full Floor Holder").AddComponent<RectTransform>();
            floorHolderFull.SetParent(panelFull.transform);
            floorHolderFull.transform.position = panelFull.transform.position + mapOffset;

            // Create the full item holder.
            itemHolderFull = new GameObject("Items").AddComponent<RectTransform>();
            itemHolderFull.SetParent(panelFull.transform);
            itemHolderFull.transform.position = panelFull.transform.position + mapOffset;
            
            // Create the sample floor holder.
            floorHolderSample = new GameObject("Sample Floor Holder").AddComponent<RectTransform>();
            floorHolderSample.SetParent(panelSample.transform);
            floorHolderSample.transform.position = panelSample.transform.position;

            //TODO: sample item holder            
        }

        // Use this for initialization
        private void callStart()
        {
            // Load relevant images.
            floorSample = Resources.Load("MiniMapImages/Image_FloorSample") as GameObject;
            unknownSample = Resources.Load("MiniMapImages/Image_UnknownSample") as GameObject;

            floorFull = Resources.Load("MiniMapImages/Image_FloorFull") as GameObject;
            unknownFull = Resources.Load("MiniMapImages/Image_UnknownFull") as GameObject;
            
            playerSample = Resources.Load("MiniMapImages/Image_PlayerSample") as GameObject;

        }

        // This is called once to begin the display. Every update is displayed elsewhere.
        public void beginMap(bool[,] fC, Vector2 lR, Vector2 sR, GameObject player)
        {
            callAwake();
            callStart();

            // Initiate floorChart.
            initiateFloorChart(fC, lR);

            // Set start and last room.
            lastRoom = lR;
            startRoom = sR;

            // The initial top right position in the full map is startRoom.
            topRight = startRoom;

            // Set the player.
            P1 = player;

            // Go through floorChart and place the rooms while initializing floorsFull.
            initialPlacement();

            
            // Place the sample map and hide the full map to start.
            panelSample.SetActive(true);
            panelFull.SetActive(false);
        }

        // Go through the floorChart from LevelManager and update GUI_MiniMap's floorChart.
        private void initiateFloorChart(bool[,] fC, Vector2 lR)
        {
            // floorChart key
            // 0 - unoccupied
            // 1 - unvisited
            // 2 - visited

            // Copy the floorChart over. No rooms are visited.
            for (int row = 0; row < 11; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    if (fC[row, column])
                    {
                        floorChart[row, column] = 1;
                    }
                    else
                    {
                        floorChart[row, column] = 0;
                    }
                }
            }
        }
        
        // Place both maps.
        private void initialPlacement()
        {
            initialPlacementFull();
            initialPlacementSample();
        }

        // Place the full map initially and instantiate floorsFull.
        private void initialPlacementFull()
        {
            // Place the tank in the starting room.
            tank = Instantiate(player) as GameObject;
            tank.transform.SetParent(itemHolderFull);
            tank.transform.position = itemHolderFull.position + new Vector3(startRoom[0] * roomLengthFull, startRoom[1] * roomLengthFull, 0);

            // Go through the floorChart and place all the rooms for full map and instantiate floorsFull.
            for (int row = 0; row < floorChart.GetLength(0); row++)
            {
                for (int column = 0; column < floorChart.GetLength(1); column++)
                {
                    if (floorChart[row, column] != 0)
                    {
                        RectTransform currFloor = new GameObject("Floor #" + (floorChart.GetLength(0) * column + row)).AddComponent<RectTransform>();
                        currFloor.transform.SetParent(floorHolderFull);
                        currFloor.transform.position = floorHolderFull.position + new Vector3(row * roomLengthFull, column * roomLengthFull, 0);
                        currFloor.gameObject.SetActive(false);

                        // Initialize floorsFull where all rooms are initially unvisited.
                        floorsFull[row, column] = currFloor;
                        
                        // The floor starts off as unknown.
                        GameObject floor = Instantiate(unknownFull) as GameObject;
                        floor.transform.SetParent(currFloor);
                        floor.transform.position = currFloor.position;
                        floor.GetComponent<RectTransform>().localScale = floor.GetComponent<RectTransform>().localScale;
                    }
                }
            }
        }

        // Place the sample map initially.
        private void initialPlacementSample()
        {
            // The initial placement of the sample map is no different than any other placement.
            // There are only 9 rooms to display so it shouldn't be slow.
            // Could make this better by having all the rooms placed but only display the 9.

            playerCoord = startRoom;
            placeSample();


            // Place the indicator for the middle room.
            playerIndicator = Instantiate(playerSample) as GameObject;
            playerIndicator.GetComponent<RectTransform>().SetParent(panelSample.transform);
            playerIndicator.GetComponent<RectTransform>().position = floorHolderSample.position;
            
            //TODO: color should be set by player tank color
            Color c = Color.blue;
            playerIndicator.transform.GetChild(0).gameObject.GetComponent<Image>().color = c;
        }

        // Places sample map while adding to the outlineHolderFull and floorHolderSample.
        private void placeSample()
        {
            //
            panelSample.SetActive(true);

            // Iterate through the rooms within the sampleRadius.
            // Set the beginning and ending boundaries to avoid out of bound indexing.
            // TODO: make cleaner.
            int begR = 0;
            if (((int)playerCoord[0] - sampleRadius) > 0)
            {
                begR = ((int)playerCoord[0] - sampleRadius);
            }
            int endR = 10;
            if (((int)playerCoord[0] + sampleRadius) < 10)
            {
                endR = ((int)playerCoord[0] + sampleRadius);
            }
            int begC = 0;
            if (((int)playerCoord[1] - sampleRadius) > 0)
            {
                begC = ((int)playerCoord[1] - sampleRadius);
            }
            int endC = 10;
            if (((int)playerCoord[1] + sampleRadius) < 10)
            {
                endC = ((int)playerCoord[1] + sampleRadius);
            }

            for (int row = begR; row <= endR; row++)
            {
                for (int column = begC; column <= endC; column++)
                {
                    // Only place a room if it is visited.
                    if (floorChart[row, column] == 2)
                    {
                        RectTransform currFloor = new GameObject("Floor #" + (floorChart.GetLength(0) * column + row)).AddComponent<RectTransform>();
                        currFloor.transform.SetParent(floorHolderSample);
                        currFloor.transform.position = floorHolderSample.position + new Vector3(row - (int)playerCoord[0], column - (int)playerCoord[1], 0) * roomLengthSample;                

                        
                        
                        GameObject floor = Instantiate(floorSample) as GameObject;
                        floor.transform.SetParent(currFloor);
                        floor.transform.position = currFloor.position;
                        floor.GetComponent<RectTransform>().localScale = floor.GetComponent<RectTransform>().localScale;

                        // Set the door indicators using FindCurrentNEWS.
                        bool[] currNEWS = FindCurrentNEWS(row, column);
                        Transform doorIndicators = floor.transform.GetChild(3);
                        int doorCount = doorIndicators.childCount;

                        for (int i = 0; i < doorCount; ++i)
                        {
                            doorIndicators.GetChild(i).gameObject.SetActive(currNEWS[i]);
                        }
                    }

                    // Only place an unvisited room as unknown if it has a neighboring, visited room.
                    if (floorChart[row, column] == 1 & HasNEWSUnknown(row, column))
                    {
                        RectTransform currFloor = new GameObject("Floor #" + (floorChart.GetLength(0) * column + row)).AddComponent<RectTransform>();
                        currFloor.transform.SetParent(floorHolderSample);
                        currFloor.transform.position = floorHolderSample.position + new Vector3(row - (int)playerCoord[0], column - (int)playerCoord[1], 0) * roomLengthSample;

                        GameObject floor = Instantiate(unknownSample) as GameObject;
                        floor.transform.SetParent(currFloor);
                        floor.transform.position = currFloor.position;
                        floor.GetComponent<RectTransform>().localScale = floor.GetComponent<RectTransform>().localScale;
                    }
                }
            }
        }        

        // Destroys room objects for the sample map.
        private void clearSample()
        {
            foreach (Transform room in floorHolderSample)
            {
                Destroy(room.gameObject);
            }

            panelSample.SetActive(false);
        }

        // Activates the full panel.
        private void placeFull()
        {
            panelFull.SetActive(true);
        }

        // Inactivates the full panel.
        private void clearFull()
        {
            panelFull.SetActive(false);
        }

        // The room at the input coordinate is now visited.
        public void visitedRoom(Vector2 coord)
        {
            // Update the floor chart.
            floorChart[(int)coord[0], (int)coord[1]] = 2;

            // This is for sample.
            playerCoord = coord;
            if (!selected)
            {
                clearSample();
                placeSample();
            }

            // This is for full.
            visitedRoomFull(coord);
        }

        // Helper for visited room that alters the full map.
        private void visitedRoomFull(Vector2 coord)
        {
            //TODO: alter the scale of floorHolderFull to fit all the rooms.
            int row = (int)coord[0];
            int column = (int)coord[1];

            // Alter topRight if necessary. Consider that the topRight room may be influenced by unvisited rooms.
            int rightMost = row;
            if (FindCurrentNEWS(row, column)[1])
            {
                rightMost += 1;
            }
            if (rightMost > topRight[0])
            {
                topRight[0] = rightMost;
            }
            
            int topMost = column;
            if (FindCurrentNEWS(row, column)[0])
            {
                topMost += 1;
            }
            if (topMost > topRight[1])
            {
                topRight[1] = topMost;
            }

            // Move the children of panel according to topRight.
            floorHolderFull.transform.position = panelFull.transform.position - new Vector3(topRight[0], topRight[1], 0) * roomLengthFull + mapOffset;
            itemHolderFull.transform.position = panelFull.transform.position - new Vector3(topRight[0], topRight[1], 0) * roomLengthFull + mapOffset;


            // Update floorsFull.
            floorsFull[(int)row, (int)column].gameObject.SetActive(true);


            // Destroy the children of the floor and outline.
            //TODO: this is destroying the object itself
            foreach (Transform image in floorsFull[(int)row, (int)column])
            {
                Destroy(image.gameObject);
            }

            // Replace the children of the floor and outline.
            RectTransform currFloor = floorsFull[(int)row, (int)column].GetComponent<RectTransform>();
            
            // The floor starts off as unknown.
            GameObject floor = Instantiate(floorFull) as GameObject;
            floor.transform.SetParent(currFloor);
            floor.transform.position = currFloor.position;
            floor.GetComponent<RectTransform>().localScale = floor.GetComponent<RectTransform>().localScale;

            // Set the door indicators using FindCurrentNEWS.
            bool[] currNEWS = FindCurrentNEWS(row, column);
            Transform doorIndicators = floor.transform.GetChild(2);
            int doorCount = doorIndicators.childCount;
            //Debug.Log(doorIndicators.name);

            // Set active the necessary door indicators.
            for (int i = 0; i < doorCount; ++i)
            {
                doorIndicators.GetChild(i).gameObject.SetActive(currNEWS[i]);
            }

            // If this is the last room, add an instance of exit to the floor.
            if (lastRoom == new Vector2((int)row, (int)column))
            {
                // Place the exit as SetActive(false).
                GameObject ladder = Instantiate(exit) as GameObject;
                ladder.transform.SetParent(currFloor);
                ladder.transform.position = currFloor.position;
            }
            
            // Set active the current floor and the neighbors.
            currFloor.gameObject.SetActive(true);
            if (currNEWS[0])
            {
                floorsFull[row, column + 1].gameObject.SetActive(true);
            }
            if (currNEWS[1])
            {
                floorsFull[row + 1, column].gameObject.SetActive(true);
            }
            if (currNEWS[2])
            {
                floorsFull[row - 1, column].gameObject.SetActive(true);
            }
            if (currNEWS[3])
            {
                floorsFull[row, column - 1].gameObject.SetActive(true);
            }
        }

        // This is called by the gates to update the players image position.
        public void movePlayer()
        {
            FindOccupiedRoom();

            // Rerender the sample map if in selected mode.
            if (!selected)
            {
                clearSample();
                placeSample();
            }

            // Adjust the tank position in the full map.
            tank.transform.position = itemHolderFull.position + new Vector3(playerCoord[0] * roomLengthFull, playerCoord[1] * roomLengthFull, 0);
        }

        // Helper function for movePlayer; updates playerCoord.
        private void FindOccupiedRoom()
        {
            // Do some math and get the room coordinate player1 is in.
            int stepLength = m_RoomLength + 2 * (int)wallThickness;
            int xCoord = (int)Mathf.Floor((P1.transform.position.x + wallThickness) / stepLength);
            int yCoord = (int)Mathf.Floor((P1.transform.position.z + wallThickness) / stepLength);

            playerCoord = new Vector2(xCoord, yCoord);
        }

        // Helper function for placeSample and initialPlacementFull.
        private bool[] FindCurrentNEWS(int xCoord, int yCoord)
        {
            // Initialized as false.
            bool[] NEWS = new bool[4];

            if (yCoord < 10)
            {
                if (floorChart[xCoord, yCoord + 1] != 0)
                {
                    NEWS[0] = true;
                }
            }
            if (xCoord < 10)
            {
                if (floorChart[xCoord + 1, yCoord] != 0)
                {
                    NEWS[1] = true;
                }
            }
            if (xCoord > 0)
            {
                if (floorChart[xCoord - 1, yCoord] != 0)
                {
                    NEWS[2] = true;
                }
            }
            if (yCoord > 0)
            {
                if (floorChart[xCoord, yCoord - 1] != 0)
                {
                    NEWS[3] = true;
                }
            }

            return NEWS;
        }

        // Helper function for placeSample and initialPlacementFull.
        private bool HasNEWSUnknown(int xCoord, int yCoord)
        {
            // Return true if there is a neighboring, visited room.
            if (yCoord < 10)
            {
                if (floorChart[xCoord, yCoord + 1] == 2)
                {
                    return true;
                }
            }
            if (xCoord < 10)
            {
                if (floorChart[xCoord + 1, yCoord] == 2)
                {
                    return true;
                }
            }
            if (xCoord > 0)
            {
                if (floorChart[xCoord - 1, yCoord] == 2)
                {
                    return true;
                }
            }
            if (yCoord > 0)
            {
                if (floorChart[xCoord, yCoord - 1] == 2)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void MapAndUnmap()
        {
            if (!selected)
            {
                placeFull();
                clearSample();
                selected = true;
            }
            else
            {
                clearFull();
                placeSample();
                selected = false;
            }
        }

        // Used by GameMaster when clearing a game.
        public void clearMap()
        {
            Destroy(floorHolderFull.gameObject);
            Destroy(floorHolderSample.gameObject);
            Destroy(itemHolderFull.gameObject);
            Destroy(tank);
            Destroy(playerIndicator);

            clearFull();
            clearSample();
        }
    }
}