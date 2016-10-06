using UnityEngine;
using System.Collections;

namespace Completed
{
    using System.Collections.Generic;       //Allows us to use Lists.
    public class RoomManager : MonoBehaviour
    {
        public GameObject blockTall;                                             // The GameObject of the tall block.
        public GameObject blockShort;                                            // The GameObject of the short block.
        public GameObject floor;                                                 // Array of open wall prefabs.
        public Material[] blockMaterials;                                        // Array of materials prefabs for the obstacles.
        public Material[] floorMaterials;                                        // Array of materials prefabs for the obstacles.
        public GameObject exit;                                                  // The GameObject of the exit.
        public GameObject wallOpen;                                              // The GameObject of the open wall.
        public GameObject wallClosed;                                            // The GameObject of the closed wall.
        public GameObject m_camera;

        private Transform m_room;                                                // Store a reference to the room transform.
        private int m_level;                                                     // Store a reference to our RoomManager which will set up the room.
        private Transform[] m_doors = new Transform[4];                          // An array of the doors.
        private bool isDoorLowered = false;                                      // The bool for is the doors are all lowered.
        private bool isPlayer1;                                                  // The bool for if player 2 is alive.
        private bool isPlayer2;                                                  // The bool for if player 2 is alive.
        private GameObject player1;
        //private GameObject player2;
        public GameObject[] enemyList;// = new List<GameObject>();             // A list of enemies for the level.
        private List<Vector3> playerSpawnLocations = new List<Vector3>();        // A list of player spawn locations.
        private List<Vector3> enemySpawnLocations = new List<Vector3>();         // A list of enemy spawn locations.
        private string exitTag = "Exit";                                         // String to apply the tag on the exit.
        private string wallTag = "Wall";
        private string blockTag = "Block";
        private Transform enemyHolder;                                           // A variable to store a reference to the transform of the enemy object.
        private Transform projectileHolder;
        private Transform wallHolder;                                            // A variable to store a reference to the transform of the wall holder object.
        private Transform courseHolder;                                          // A variable to store a reference to the transform of the obstacle course object.
        private float wallThickness = 1f;                                        // Thickness of outside walls.
        private float blockThickness = 2.5f;                                     // Thickness of blocks.
        private int m_RoomLength = 50;                                           // Length of each room declared elsewhere also.
        private float gateHeight = 6f;
        private struct obstacleCourse                                            // A struct for an obstacle course.
        {
            public int[,] room;

            public int[,] get()
            {
                return room;
            }

            public void set(int[,] rm)
            {
                room = rm;
            }
        }
        private List<obstacleCourse> obstacleCourses = new List<obstacleCourse>();// A list of obstacle courses.
        private bool[] m_NEWSWall = new bool[4];                                 // An array of bools to represent if the wall is open.

        private Gate doorScript;
        private bool battleOver = false;
        private bool proceedEndingSequence = true;
        public bool battleBegin = false;
        private bool proceedBeginningSequence = false;
        public bool roomCompleted = false;
        private bool battleEnsuing = false;
        private bool roomIdle = false;
        private Transform lightHolder;
        private Light example;
        private bool lightsEquiped = false;
        private Transform[] NEWSRoom;
        public Transform roomTo;
        public bool needToTravel = true;
        public LevelManager levelScript;
        public int[] roomCoord = new int[2];
        private bool[] hasTriggeredNEWS = new bool[4];
        public bool isLastRoom = false;

        // Instantiates the 2D arrays representing the room's obstacle course.
        private void SetObstacleCourses()
        {
            // 0 = empty
            // 1 = tall block
            // 2 = short block
            // 3 = tall, removable block
            // 4 = short, removable block
            // 5 = player spawn point
            // 6 = dynamic enemy spawn point
            // 7 = stagnent enemy spawn point

            obstacleCourse roomChart1 = new obstacleCourse
            {
                room = new int[20, 20] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 5, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 6, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 5, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} }
            };
            obstacleCourses.Add(roomChart1);

            obstacleCourse roomChart2 = new obstacleCourse
            {
                room = new int[20, 20] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 6, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 5, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 5, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 6, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} }
            };
            obstacleCourses.Add(roomChart2);

            obstacleCourse roomChart3 = new obstacleCourse
            {
                room = new int[20, 20] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} }
            };
            obstacleCourses.Add(roomChart3);

            obstacleCourse roomChart4 = new obstacleCourse
            {
                room = new int[20, 20] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 6, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 6, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} }
            };
            obstacleCourses.Add(roomChart4);

            obstacleCourse roomChart5 = new obstacleCourse
            {
                room = new int[20, 20] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 6, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {4, 4, 4, 4, 2, 2, 2, 2, 2, 4, 4, 2, 2, 2, 2, 2, 4, 4, 4, 4},
                {4, 4, 4, 4, 2, 2, 2, 2, 2, 4, 4, 2, 2, 2, 2, 2, 4, 4, 4, 4},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} }
            };
            obstacleCourses.Add(roomChart5);

            obstacleCourse roomChart6 = new obstacleCourse
            {
                room = new int[20, 20] {
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 1, 1, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 2, 0, 0, 7, 0, 0, 2, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 1, 1, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3} }
            };
            obstacleCourses.Add(roomChart6);

            obstacleCourse roomChart7 = new obstacleCourse
            {
                room = new int[20, 20] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 5, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 5, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3} }
            };
            //Debug.Log(levelScript.coop);
            if (false)//levelScript.coop)
            {
                obstacleCourses.Add(roomChart7);
            }
        }


        private void startEndingBattle()
        {
            if (proceedEndingSequence)
            {
                // Not getting every projectile, might want to kill every projectile during ending sequence
                player1.GetComponent<Tank>().SetLeftoverProjectileHolder(projectileHolder);
                player1.GetComponent<Tank>().TransferProjectiles();


                // Disable projectiles.
                for (int proj = 0; proj < projectileHolder.GetComponentsInChildren<Projectile>().Length; proj++)
                {
                    Projectile currentProj = projectileHolder.GetComponentsInChildren<Projectile>()[proj];
                    currentProj.GetComponent<Projectile>().DisableProjectile();
                }

                //wait

                proceedEndingSequence = false;
            }
                removeDoorsAndObstaclesAndProjectiles();
        }

        // Helper for the end of a battle.
        private void removeDoorsAndObstaclesAndProjectiles()
        {
            // Lower doors.
            for (int door = 0; door < m_doors.Length; door++)
            {
                if (m_NEWSWall[door])
                {
                    doorScript = m_doors[door].GetComponent<Gate>();
                    if (isLastRoom)
                    {
                        doorScript.lowerDoorLastRoom();
                    }
                    else if (NEWSRoom[door].GetComponent<RoomManager>().roomCompleted)
                    {
                        doorScript.lowerDoorFast();
                    }
                    else
                    {
                        doorScript.lowerDoorSlow();
                    }

                    // Also remove the boundaries of neighboring, completed rooms.
                    if (NEWSRoom[door].GetComponent<RoomManager>().roomCompleted)
                    {
                        if (door == 0)
                        {
                            NEWSRoom[door].GetComponent<RoomManager>().m_doors[3].GetComponent<Gate>().boundary.enabled = false;
                        }
                        if (door == 1)
                        {
                            NEWSRoom[door].GetComponent<RoomManager>().m_doors[2].GetComponent<Gate>().boundary.enabled = false;
                        }
                        if (door == 2)
                        {
                            NEWSRoom[door].GetComponent<RoomManager>().m_doors[1].GetComponent<Gate>().boundary.enabled = false;
                        }
                        if (door == 3)
                        {
                            NEWSRoom[door].GetComponent<RoomManager>().m_doors[0].GetComponent<Gate>().boundary.enabled = false;
                        }
                    }
                }
            }

            // Remove obstacles.
            foreach (Transform block in courseHolder) if (block.CompareTag(blockTag))
                {
                    float speed = 40f; //how fast it shakes
                                     
                    //TODO: audio                                     
                    /*if (!audioPlayed)
                    {
                    gateAudioSource.Play();
                    audioPlayed = true;
                    }
                    float lowerSpeed = gateHeight / (closeAudio.length + 2.0f);
                    */

                    if (block.position.y > -20)
                    {
                        Vector3 lowering = Vector3.down * speed * Time.deltaTime;
                        block.Translate(lowering);
                    }
                }
            isDoorLowered = true;
        }
        
        
        private void startBeginningBattle()
        {
            // Place enemy tanks.
            if (!proceedBeginningSequence)
            {
                StartCoroutine(BeginSetUp());

                // Set the camera's battling variable to true;
                m_camera.GetComponent<CameraControl>().battling = true;

                proceedBeginningSequence = true;
                battleEnsuing = true;
                battleBegin = false;
            }
        }

        IEnumerator BeginSetUp()
        {
            // Place the unenabled enemies and players.
            //TODO: make the colors of the enemies happen on awake not enable
            PlaceEnemies();
            PlacePlayers();

            // Place the enemy sprite HUD based on the enemies in the room.
            GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().PlaceEnemies(enemyHolder);

            GameObject.FindGameObjectWithTag("HUD").GetComponent<GUI_HUD>().PlayCountDown();
            yield return new WaitForSeconds(4f);

            EnableTanks();
        }



        private void PlaceEnemies()
        {
            //TODO: somehow make random and eliminate each point from the list
            for (int location = 0; location < enemySpawnLocations.Count; location++)
            {
                GameObject enemy = Instantiate(enemyList[Random.Range(0, enemyList.Length)]) as GameObject;
                enemy.transform.position = enemySpawnLocations[location];
                enemy.transform.SetParent(enemyHolder);

                // Send the projectile holder to each tank to hold projectiles when the enemy is killed.
                enemy.GetComponent<Tank>().SetLeftoverProjectileHolder(projectileHolder);

                // Disable until battle starts.
                enemy.GetComponent<Tank>().enabled = false;
            }
        }


        public void PlacePlayers()
        {
            //TODO: somehow make sure there are only 2 spawn points
            int placePlayerFirst = Random.Range(1, 2);
            for (int location = 0; location < playerSpawnLocations.Count; location++)
            {
                if (placePlayerFirst == 1)
                {
                    // Remove projectiles.
                    for (int proj = 0; proj < player1.GetComponent<Tank>().projectileHolder.GetComponentsInChildren<Projectile>().Length; proj++)
                    {
                        Projectile currentProj = player1.GetComponent<Tank>().projectileHolder.GetComponentsInChildren<Projectile>()[proj];
                        currentProj.GetComponent<ProjectilePlayer>().RemoveProjectile();
                    }
                    player1.transform.position = playerSpawnLocations[location];
                    player1.GetComponent<Tank>().SetLeftoverProjectileHolder(player1.GetComponent<Tank>().projectileHolder);
                    player1.GetComponent<Tank>().body.rotation = Quaternion.LookRotation(Vector3.back);// player1.GetComponent<Tank>().body.forward);

                    // Disable until battle starts.
                    player1.GetComponent<TankPlayer>().disabled = true;

                    placePlayerFirst = 2;
                }
                else
                {
                    //TODO: implement with player2
                    //player1.transform.position = playerSpawnLocations[location];
                    //player1.GetComponent<Tank>().SetLeftoverProjectileHolder(player1.GetComponent<Tank>().projectileHolder);
                    // Remove projectiles.
                    /*for (int proj = 0; proj < player2.GetComponent<Tank>().projectileHolder.GetComponentsInChildren<Projectile>().Length; proj++)
                    {
                        Projectile currentProj = player2.GetComponent<Tank>().projectileHolder.GetComponentsInChildren<Projectile>()[proj];
                        currentProj.GetComponent<Projectile>().KillProjectile();
                    }*/
                    placePlayerFirst = 1;
                }
                
            }
        }


        private void EnableTanks()
        {
            foreach (Transform enemy in enemyHolder)
            {
                enemy.GetComponent<Tank>().enabled = true;
            }

            
            player1.GetComponent<TankPlayer>().disabled = false;
            //player2.GetComponent<Tank>().disableMovement = false;
        }


        public bool isRoomIdle()
        {
            return roomIdle;
        }


        private void exitRoomCheck()
        {
            // If every gate has been traveled through, there is no need to still check exit room.
            List<Transform> walls = new List<Transform>();
            foreach (Transform wall in wallHolder) if (wall.CompareTag(wallTag))
                {
                    walls.Add(wall);
                }
            for (int room = 0; room < NEWSRoom.Length; room++)
            {
                if (NEWSRoom[room])
                {
                    if (walls[room].GetComponentsInChildren<Transform>()[1].GetComponent<Gate>().triggered &&
                        !hasTriggeredNEWS[room])
                    {
                        // If going to a old room...
                        if (NEWSRoom[room].GetComponent<RoomManager>().roomCompleted)
                        {
                            hasTriggeredNEWS[room] = true;
                        }
                        // If going to new room...
                        else
                        {
                            roomTo = NEWSRoom[room];
                            levelScript.startRoom(roomTo);
                            hasTriggeredNEWS[room] = true;
                        }
                    }
                }
            }
            if (NEWSRoom[0])
            {
                if (walls[0].GetComponentsInChildren<Transform>()[1].GetComponent<Gate>().triggered &&
                    !hasTriggeredNEWS[0])
                {
                    Debug.Log("here");
                    if (NEWSRoom[0].GetComponent<RoomManager>().roomCompleted)
                    {
                        hasTriggeredNEWS[0] = true;
                    }
                    else
                    {
                        roomTo = NEWSRoom[0];
                        levelScript.startRoom(roomTo);
                        hasTriggeredNEWS[0] = true;
                    }
                }
            }
            if (NEWSRoom[1])
            {
                if (walls[1].GetComponentInChildren<Gate>().triggered &&
                    !hasTriggeredNEWS[1])
                {
                    Debug.Log("here");
                    if (NEWSRoom[1].GetComponent<RoomManager>().roomCompleted)
                    {
                        hasTriggeredNEWS[1] = true;
                    }
                    else
                    {
                        roomTo = NEWSRoom[1];
                        levelScript.startRoom(roomTo);
                        hasTriggeredNEWS[1] = true;
                    }
                }
            }
            if (NEWSRoom[2])
            {
                if (walls[2].GetComponentInChildren<Gate>().triggered &&
                    !hasTriggeredNEWS[2])
                {
                    if (NEWSRoom[2].GetComponent<RoomManager>().roomCompleted)
                    {
                        Debug.Log("here");
                        hasTriggeredNEWS[2] = true;
                    }
                    else
                    {
                        roomTo = NEWSRoom[2];
                        levelScript.startRoom(roomTo);
                        hasTriggeredNEWS[2] = true;
                    }
                }
            }
            if (NEWSRoom[3])
            {
                if (walls[3].GetComponentInChildren<Gate>().triggered &&
                    !hasTriggeredNEWS[3])
                {
                    if (NEWSRoom[3].GetComponent<RoomManager>().roomCompleted)
                    {
                        hasTriggeredNEWS[3] = true;
                    }
                    else
                    {
                        roomTo = NEWSRoom[3];
                        levelScript.startRoom(roomTo);
                        hasTriggeredNEWS[3] = true;
                    }
                }
            }
        }

        // Places the desired obstacle course as the child of parentObject.
        private void PlaceObstacleCourse(int roomChartNumber)
        {
            //TODO: might want to place removable obstacles in a special parent transform
            // 0 = empty
            // 1 = tall block
            // 2 = short block
            // 3 = tall, removable block
            // 4 = short, removable block
            // 5 = player spawn point
            // 6 = enemy spawn point

            int[,] grid = obstacleCourses[roomChartNumber].get();

            // Create an object to make the parent of each object.
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int column = 0; column < grid.GetLength(1); column++)
                {
                    // Place the objects for each coordinate.
                    if (grid[row, column] == 1)
                    {
                        GameObject block =
                            Instantiate(blockTall,
                            new Vector3(column * blockThickness + blockThickness / 2, 3f, 50 - (row + 1) * blockThickness + blockThickness / 2)
                            + m_room.transform.position,
                            Quaternion.identity) as GameObject;
                        block.transform.SetParent(courseHolder);

                        block.GetComponent<MeshRenderer>().material = blockMaterials[Random.Range(0, blockMaterials.Length)];
                        // Set the texture of the floor depending on the level.
                        block.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(.01f, .01f));
                        // Set a random offset on the texture.
                        block.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2((float)Random.Range(0, m_RoomLength) / (float)m_RoomLength, (float)Random.Range(0, m_RoomLength) / (float)m_RoomLength));

                        //Texturize(block, "Cube");
                        //block.GetComponent<MeshRenderer>().material = blockMaterials[Random.Range(0, blockMaterials.Length)];
                    }
                    else if (grid[row, column] == 2)
                    {
                        GameObject block =
                            Instantiate(blockShort,
                            new Vector3(column * blockThickness + blockThickness / 2, .4f, 50 - (row + 1) * blockThickness + blockThickness / 2)
                            + m_room.transform.position,
                            Quaternion.identity) as GameObject;
                        block.transform.SetParent(courseHolder);
                    }
                    else if (grid[row, column] == 3)
                    {
                        GameObject block =
                            Instantiate(blockTall,
                            new Vector3(column * blockThickness + blockThickness / 2, 3f, 50 - (row + 1) * blockThickness + blockThickness / 2)
                            + m_room.transform.position,
                            Quaternion.identity) as GameObject;
                        block.transform.SetParent(courseHolder);
                        //TODO: figure out how to remove the blocks when the level is complete
                    }
                    else if (grid[row, column] == 4)
                    {
                        GameObject block =
                            Instantiate(blockShort,
                            new Vector3(column * blockThickness + blockThickness / 2, .4f, 50 - (row + 1) * blockThickness + blockThickness / 2)
                            + m_room.transform.position,
                            Quaternion.identity) as GameObject;
                        block.transform.SetParent(courseHolder);
                        //TODO: figure out how to remove the blocks when the level is complete
                    }
                    else if (grid[row, column] == 5)
                    {
                        playerSpawnLocations.Add(new Vector3(column * blockThickness + blockThickness / 2, 0f, 50 - (row + 1) * blockThickness + blockThickness / 2)
                            + m_room.transform.position);
                    }
                    else if (grid[row, column] == 6)
                    {
                        enemySpawnLocations.Add(new Vector3(column * blockThickness + blockThickness / 2, 0f, 50 - (row + 1) * blockThickness + blockThickness / 2)
                               + m_room.transform.position);
                    }
                }
            }
        }
        
        // Give the object that levels texture and randomize the offset and scale.
        private void Texturize(GameObject objectToChange, string objectType)
        {
            // Types of objectTypes are "plane", "cube", "wall", and "cylinder".
            if (objectType == "plane")
            {
                objectToChange.GetComponent<MeshRenderer>().material = floorMaterials[m_level];
                // Set the texture of the floor depending on the level.
                objectToChange.GetComponent<MeshRenderer>().material.SetTextureScale
                    ("_MainTex", new Vector2(1f, 1f));
                // Set a random offset on the texture.
                objectToChange.GetComponent<MeshRenderer>().material.SetTextureOffset
                    ("_MainTex", new Vector2((float)Random.Range(0, m_RoomLength) / (float)m_RoomLength, (float)Random.Range(0, m_RoomLength) / (float)m_RoomLength));
            }

            else if (objectType == "cube")
            {
                // Set texture distributions.
                SetTextureDistribution();
                // now use textureDistributions

                foreach (Renderer child in objectToChange.GetComponentsInChildren<Renderer>())
                {
                    child.material = blockMaterials[Random.Range(0, blockMaterials.Length)];
                    // Set the texture of the floor depending on the level.
                    child.material.SetTextureScale("_MainTex", new Vector2(.01f, .01f));
                    // Set a random offset on the texture.
                    child.material.SetTextureOffset("_MainTex", new Vector2((float)Random.Range(0, m_RoomLength) / (float)m_RoomLength, (float)Random.Range(0, m_RoomLength) / (float)m_RoomLength));

                }
            }
        }

        // TODO: Sets the texture distribution for this room dependent on the level.
        private void SetTextureDistribution()
        {
            //TODO: make it based on level
        }
        
        // Sets up room by taking in the transform of the room, the level, block and floor materials, and NEWSWall.
        public void SetUpRoom(Transform room, int level, Material[] blockM, Material[] floorM, bool[] NEWSWall)
        {
            // Get a reference to the transform of the room.
            m_room = room;

            // Set the level number.
            m_level = level;

            // Take the block and floor materials in.
            blockMaterials = blockM;
            floorMaterials = floorM;

            // Take the array of bools
            m_NEWSWall = NEWSWall;

            // Load in the GameObjects.
            blockTall = Resources.Load("Blocks/BlockTall") as GameObject;
            blockShort = Resources.Load("Blocks/BlockShort") as GameObject;
            exit = Resources.Load("Miscellaneous/Ladder") as GameObject;
            wallOpen = Resources.Load("WallOpen") as GameObject;
            wallClosed = Resources.Load("WallClosed") as GameObject;

            // Fill enemyList.
            enemyList = Resources.LoadAll<GameObject>("TankEnemy");

            // Create enemyHolder for this room.
            enemyHolder = new GameObject("EnemyHolder").transform;
            enemyHolder.SetParent(m_room);
            enemyHolder.position = m_room.position;

            // Create lightHolder for this room.
            lightHolder = new GameObject("LightHolder").transform;
            lightHolder.transform.SetParent(m_room);
            lightHolder.position = m_room.position;

            // Create lightHolder for this room.
            wallHolder = new GameObject("WallHolder").transform;
            wallHolder.transform.SetParent(m_room);
            wallHolder.position = m_room.position;

            // Create projectileHolder for this room.
            projectileHolder = new GameObject("ProjectileHolder").transform;
            projectileHolder.SetParent(m_room);
            projectileHolder.position = m_room.position;

            // Create projectileHolder for this room.
            courseHolder = new GameObject("ObstacleCourseHolder").transform;
            courseHolder.SetParent(m_room);
            courseHolder.position = m_room.position;

            // Place the floor.
            PlaceFloor();

            // Place the walls.
            PlaceWalls();
        }

        // Helper for the SetUpRoom.
        private void PlaceFloor()
        {
            floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.position = new Vector3(m_RoomLength / 2, 0, m_RoomLength / 2)
            + m_room.transform.position;
            floor.name = "Floor";
            floor.transform.SetParent(m_room);
            floor.transform.localScale = new Vector3(5, 1, 5);
            floor.GetComponent<MeshRenderer>().material = floorMaterials[Random.Range(0, floorMaterials.Length)];
            // Set the texture of the floor depending on the level.
            floor.GetComponent<MeshRenderer>().material.SetTextureScale
                ("_MainTex", new Vector2(1f, 1f));
            // Set a random offset on the texture.
            floor.GetComponent<MeshRenderer>().material.SetTextureOffset
                ("_MainTex", new Vector2((float)Random.Range(0, m_RoomLength) / (float)m_RoomLength, (float)Random.Range(0, m_RoomLength) / (float)m_RoomLength));

        }

        // Helper for the SetUpRoom.
        private void PlaceWalls()
        {
            // NorthWall
            if (m_NEWSWall[0])
            {
                GameObject placeNorthWall = Instantiate(wallOpen,
                    m_room.transform.position + new Vector3(m_RoomLength / 2, 0, m_RoomLength + .5f * wallThickness),
                    Quaternion.identity) as GameObject;
                placeNorthWall.transform.Rotate(0, 90, 0);
                placeNorthWall.transform.SetParent(wallHolder);
                placeNorthWall.tag = wallTag;
                // Add this to m_doors.
                m_doors[0] = placeNorthWall.GetComponentsInChildren<Transform>()[1];
                //doorScript = placeNorthWall.GetComponentsInChildren<Transform>()[1].GetComponent<Gate>();
                doorScript = placeNorthWall.GetComponentInChildren<Gate>();
                doorScript.parentRoomScript = this;
                // Place light.
                GameObject northWallLight = new GameObject();
                northWallLight.name = "NorthLight";
                northWallLight.transform.SetParent(lightHolder);
                northWallLight.transform.position = placeNorthWall.transform.position + new Vector3(0, 10, -20);
                example = northWallLight.AddComponent<Light>(); //Light light = northWallLight.AddComponent<Light>();
                example.color = Color.red; example.range = 35; example.intensity = 5; example.type = LightType.Spot;
                northWallLight.transform.rotation = Quaternion.Euler(32, 0, 0);
                example.enabled = false;
            }
            else
            {
                GameObject placeNorthWall = Instantiate(wallClosed,
                    m_room.transform.position + new Vector3(m_RoomLength / 2, 0, m_RoomLength + .5f * wallThickness),
                    Quaternion.identity) as GameObject;
                placeNorthWall.transform.Rotate(0, 90, 0);
                placeNorthWall.transform.SetParent(wallHolder);
                placeNorthWall.tag = wallTag;
            }
            // EastWall
            if (m_NEWSWall[1])
            {
                GameObject placeEastWall = Instantiate(wallOpen,
                    m_room.transform.position + new Vector3(m_RoomLength + .5f * wallThickness, 0, m_RoomLength / 2),
                    Quaternion.identity) as GameObject;
                placeEastWall.transform.Rotate(0, 180, 0);
                placeEastWall.transform.SetParent(wallHolder);
                placeEastWall.tag = wallTag;
                // Add this to m_doors.
                m_doors[1] = placeEastWall.GetComponentsInChildren<Transform>()[1];
                doorScript = placeEastWall.GetComponentsInChildren<Transform>()[1].GetComponent<Gate>();
                doorScript.parentRoomScript = this;
                // Place light.
                GameObject northEastLight = new GameObject();
                northEastLight.name = "EastLight";
                northEastLight.transform.SetParent(lightHolder);
                northEastLight.transform.position = placeEastWall.transform.position + new Vector3(-20, 10, 0);
                Light light = northEastLight.AddComponent<Light>();
                light.color = Color.red; light.range = 35; light.intensity = 5; light.type = LightType.Spot;
                northEastLight.transform.rotation = Quaternion.Euler(32, 90, 0);
                light.enabled = false;
            }
            else
            {
                GameObject placeEastWall = Instantiate(wallClosed,
                    m_room.transform.position + new Vector3(m_RoomLength + .5f * wallThickness, 0, m_RoomLength / 2),
                    Quaternion.identity) as GameObject;
                placeEastWall.transform.Rotate(0, 180, 0);
                placeEastWall.transform.SetParent(wallHolder);
                placeEastWall.tag = wallTag;
            }
            // WestWall
            if (m_NEWSWall[2])
            {
                GameObject placeWestWall = Instantiate(wallOpen,
                    m_room.transform.position + new Vector3(-.5f * wallThickness, 0, m_RoomLength / 2),
                    Quaternion.identity) as GameObject;
                placeWestWall.transform.SetParent(wallHolder);
                placeWestWall.tag = wallTag;
                // Add this to m_doors.
                m_doors[2] = placeWestWall.GetComponentsInChildren<Transform>()[1];
                doorScript = placeWestWall.GetComponentsInChildren<Transform>()[1].GetComponent<Gate>();
                doorScript.parentRoomScript = this;
                // Place light.
                GameObject westWallLight = new GameObject();
                westWallLight.name = "WestLight";
                westWallLight.transform.SetParent(lightHolder);
                westWallLight.transform.position = placeWestWall.transform.position + new Vector3(20, 10, 0);
                Light light = westWallLight.AddComponent<Light>();
                light.color = Color.red; light.range = 35; light.intensity = 5; light.type = LightType.Spot;
                westWallLight.transform.rotation = Quaternion.Euler(32, 270, 0);
                light.enabled = false;
            }
            else
            {
                GameObject placeWestWall = Instantiate(wallClosed,
                    m_room.transform.position + new Vector3(-.5f * wallThickness, 0, m_RoomLength / 2),
                    Quaternion.identity) as GameObject;
                placeWestWall.transform.SetParent(wallHolder);
                placeWestWall.tag = wallTag;
            }
            // SouthWall
            if (m_NEWSWall[3])
            {
                GameObject placeSouthWall = Instantiate(wallOpen,
                    m_room.transform.position + new Vector3(m_RoomLength / 2, 0, -.5f * wallThickness),
                    Quaternion.identity) as GameObject;
                placeSouthWall.transform.SetParent(wallHolder);
                placeSouthWall.transform.Rotate(0, 270, 0);
                placeSouthWall.tag = wallTag;
                // Add this to m_doors.
                m_doors[3] = placeSouthWall.GetComponentsInChildren<Transform>()[1];
                doorScript = placeSouthWall.GetComponentsInChildren<Transform>()[1].GetComponent<Gate>();
                doorScript.parentRoomScript = this;
                // Place light.
                GameObject southWallLight = new GameObject();
                southWallLight.name = "SouthLight";
                southWallLight.transform.SetParent(lightHolder);
                southWallLight.transform.position = placeSouthWall.transform.position + new Vector3(0, 10, 20);
                Light light = southWallLight.AddComponent<Light>();
                light.color = Color.red; light.range = 35; light.intensity = 5; light.type = LightType.Spot;
                southWallLight.transform.rotation = Quaternion.Euler(32, 180, 180);
                light.enabled = false;
            }
            else
            {
                GameObject placeSouthWall = Instantiate(wallClosed,
                    m_room.transform.position + new Vector3(m_RoomLength / 2, 0, -.5f * wallThickness),
                    Quaternion.identity) as GameObject;
                placeSouthWall.transform.SetParent(wallHolder);
                placeSouthWall.transform.Rotate(0, 270, 0);
                placeSouthWall.tag = wallTag;
            }
        }
        
        // Creates an obstacle course after the room is set up.
        public void CreateObstacleCourse()
        {

            // Place a random obstacle course.
            SetObstacleCourses();
            int randomCourseIndex = Random.Range(0, obstacleCourses.Count);
            PlaceObstacleCourse(randomCourseIndex);
        }

        // Creates a starting room after the room is set up.
        public void CreateStartingRoom()
        {
            playerSpawnLocations.Add(new Vector3(25f, 0, 25f) + m_room.transform.position);
            enemySpawnLocations.Add(new Vector3(10f, 0f, 10f) + m_room.transform.position);
        }

        // Creates a starting room with instructions after the room is set up.
        public void CreateStartingRoomWithInstructions()
        {
            //TODO: add the instructions
            GameObject instructionCourse = new GameObject();
            instructionCourse.transform.position = 
                new Vector3(m_RoomLength / 2 + m_room.position.x, 0, m_RoomLength / 2 + m_room.position.z);
            instructionCourse.transform.SetParent(courseHolder);
            playerSpawnLocations.Add(new Vector3(25f, 0, 25f) + m_room.transform.position);
            enemySpawnLocations.Add(new Vector3(10f, 0f, 10f) + m_room.transform.position);
        }

        // Creates a last room after the room is set up.
        public void CreateLastRoom()
        {
            //TODO: figure out what to do about spawns
            // This is the last room, so place the exit in the room.
            GameObject ladderExit = Instantiate(exit) as GameObject;
            ladderExit.transform.position = m_room.transform.position + new Vector3(m_RoomLength / 2 - 2, 0, m_RoomLength / 2);

            ladderExit.transform.SetParent(courseHolder);
            ladderExit.tag = exitTag;
        }


        private void endBattle()
        {
            //TODO: add completion audio
            //TODO: reset where the players projectiles go
            //TODO: when traveling to new room check if room is completed


            // Set the camera's battling variable to true;
            m_camera.GetComponent<CameraControl>().battling = false;

            roomIdle = true;
            roomCompleted = true;
            battleOver = true;
            battleEnsuing = false;
            StartCoroutine(FlickerLights());
            lightsEquiped = true;
        }


        IEnumerator FlickerLights()
        {
            for (int flickerAmount = 0; flickerAmount < 5; flickerAmount++)
            {
                for (int i = 0; i < lightHolder.transform.childCount; i++)
                {
                    lightHolder.GetComponentsInChildren<Light>()[i].enabled = true;
                }
                yield return new WaitForSeconds(.1f);
                for (int i = 0; i < lightHolder.transform.childCount; i++)
                {
                    lightHolder.GetComponentsInChildren<Light>()[i].enabled = false;
                }
                yield return new WaitForSeconds(.65f);
            }
            for (int i = 0; i < lightHolder.transform.childCount; i++)
            {
                lightHolder.GetComponentsInChildren<Light>()[i].enabled = true;
            }
        }
        

        public void PassNEWSRooms(Transform[] NEWS)
        {
            NEWSRoom = NEWS;
        }

        public void GetNEWSRooms()
        {
            //Debug.Log(levelScript.SendNEWS(m_room).Length);
            NEWSRoom = levelScript.SendNEWS(m_room);
        }

        void Update()
        {
            if (battleBegin)
            {
                startBeginningBattle();
            }
            else if (battleOver)
            {
                startEndingBattle();
                roomIdle = true;
                
            }
            else if (battleEnsuing)
            {
                if (enemyHolder.childCount == 0)
                {
                    endBattle();
                }
            }

            if (roomIdle)
            {
                exitRoomCheck();
            }
        }


        void Start()
        {
            player1 = GameObject.FindGameObjectWithTag("Player");

            for(int wall = 0; wall < hasTriggeredNEWS.Length; wall++)
            {
                hasTriggeredNEWS[wall] = false;
            }
        }
    }
}
 