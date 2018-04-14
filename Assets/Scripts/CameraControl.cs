using UnityEngine;
using System.Collections;

namespace Completed
{
    public class CameraControl : MonoBehaviour
    {
        public Transform background;
        public GameObject m_Player1;                   // Reference to the player's transform.
        public GameObject m_Player2;                   // Reference to the player's transform.
        public new Transform camera;

        
        public Transform targetRoom;
        private Vector3 m_target;

        private float m_RoomLength = 23;
        private float m_WallThickness = 1.75f;
        private float stepLength = 25;
        private Vector3 centerOfRoom = new Vector3(6, 0, 6);

        private Vector3 m_cameraAngle = new Vector3(80, 0, 0);
        private Vector3 cameraOffset = Vector3.Normalize(new Vector3(0, 22, -2));
        private float patrolOffset = 27;
        private float battleOffset = 21;
        private float deadOffset = 35;

        private float cameraSpeedMinimum = 45;
        //private float cameraSpeedEnding = 5;

        private float shakeRadius = .25f;
        private float shakeTime = 1f;

        public Color colorMain;
        public Color colorAccent;

        // State Variables
        private enum State
        {
            PATROL,
            BATTLE,
            GAMEOVER,
            STATEFINISH
        }
        
        private State state;
        
        void Start()
        {
            //Initialize();
            //placeBackground();

            state = CameraControl.State.BATTLE;
            StartCoroutine(FSM());
        }

        public void Initialize(Transform room)
        {
            placeBackground();
            PlaceOnRoom(room);

            state = CameraControl.State.BATTLE;
            StartCoroutine(FSM());
        }

        private void placeBackground()
        {
            background.gameObject.SetActive(true);
            background.GetComponent<BackgroundWave>().Initialize(colorMain, colorAccent);
        }


        // Finite State Machine representing the actions TankEnemy goes through
        private IEnumerator FSM()
        {
            while (true)
            {
                switch (state)
                {
                    case State.PATROL:
                        TrackPlayer();
                        Patrol();
                        break;
                    case State.BATTLE:
                        TrackPlayer();
                        Battle();
                        break;
                    case State.GAMEOVER:
                        GameOverZoom();
                        break;
                    case State.STATEFINISH:
                        //Null
                        break;
                }
            yield return null;
            }
        }
        
        private void TrackPlayer()
        {
            if (m_Player1)
            {
                m_target = new Vector3(Mathf.Floor((m_Player1.transform.position.x + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2,
                        0,
                        Mathf.Floor((m_Player1.transform.position.z + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2);
                float step = Mathf.Max(Vector3.Distance(transform.position, m_target), cameraSpeedMinimum) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, m_target, step);
            }
        }
        private void Patrol()
        {
            Vector3 targetPos = patrolOffset * cameraOffset;
            float step = Mathf.Max(Vector3.Distance(camera.transform.localPosition, targetPos), cameraSpeedMinimum) * Time.deltaTime;
            camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, targetPos, step);

            camera.LookAt(transform);
        }

        private void Battle()
        {
            if (transform.position == m_target)
            {
                Vector3 targetPos = battleOffset * cameraOffset;
                float step = Mathf.Max(Vector3.Distance(camera.transform.localPosition, targetPos), cameraSpeedMinimum) * Time.deltaTime;
                camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, targetPos, step);
            }

            camera.LookAt(transform);
        }

        private void GameOverZoom()
        {
            Vector3 targetPos = deadOffset * cameraOffset;
            float step = Mathf.Max(Vector3.Distance(camera.transform.localPosition, targetPos), cameraSpeedMinimum) * Time.deltaTime;
            camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, targetPos, step);

            camera.LookAt(transform);

            // Check if the state is finished.
            if (camera.transform.localPosition == targetPos)
            {
                state = CameraControl.State.STATEFINISH;
            }
        }
        
        private void PlaceOnRoom(Transform room)
        {
            Vector3 t = new Vector3(Mathf.Floor((room.transform.position.x + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2,
                    0,
                    Mathf.Floor((room.transform.position.z + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2);
            
            camera.transform.localPosition = battleOffset * cameraOffset;
            transform.position = t;

            camera.LookAt(transform);
        }
        public void RoomEntered(Transform room)
        {
            PlaceOnRoom(room);
        }

        public IEnumerator shakeCamera()
        {
            // make sure camera still moves to !battling position
            Vector3 camPos = transform.GetChild(0).transform.position;
            float timeLeft = shakeTime;
            while (timeLeft > 0)
            {
                // Give a random position.
                transform.GetChild(0).transform.position = camPos + shakeRadius * Random.insideUnitSphere;
                yield return new WaitForSeconds(.05f);
                timeLeft -= .05f;
            }

            // Reset the camera's position to Vector3.zero.
            camera.transform.GetChild(0).transform.position = camPos;
        }
        
        public void startBattleCamera(Transform battleRoom)
        {
            state = CameraControl.State.BATTLE;

            //PlaceOnRoom(battleRoom);
            background.GetComponent<BackgroundWave>().activateMatch(true);
        }

        public IEnumerator endBattleCamera()
        {
            state = CameraControl.State.PATROL;

            //yield return shakeCamera();
            yield return new WaitForSeconds(.05f);
            background.GetComponent<BackgroundWave>().activateMatch(false);
        }

        public void gameOverCamera()
        {
            state = CameraControl.State.GAMEOVER;
        }
    }
}