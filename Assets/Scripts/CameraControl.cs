using UnityEngine;
using System.Collections;

namespace Completed
{
    public class CameraControl : MonoBehaviour
    {
        public Transform background;
        public GameObject m_Player1;                   // Reference to the player's transform.
        public GameObject m_Player2;                   // Reference to the player's transform.
        public Transform camera;


        public Transform targetRoom;
        private Vector3 m_target;

        private float m_RoomLength = 50;
        private float m_WallThickness = 1;
        private float stepLength = 52;
        private Vector3 centerOfRoom = new Vector3(25f, 0, 25f);

        private Vector3 m_cameraAngle = new Vector3(80, 0, 0);
        private Vector3 cameraOffset = Vector3.Normalize(new Vector3(0, 22, -1));
        private Vector3 lookOffset = new Vector3(0, 0, -25);
        //private Vector3 cameraOffset = new Vector3(0, 0, -4);
        private float patrolOffset = 55;
        private float battleOffset = 42;
        private float deadOffset = 70;

        private float cameraSpeedMinimum = 45;
        private float cameraSpeedEnding = 5;

        private float shakeRadius = .25f;
        private float shakeTime = 1f;

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
            //background.gameObject.SetActive(true);

            state = CameraControl.State.BATTLE;
            StartCoroutine(FSM());
        }


        // Finite State Machine representing the actions TankEnemy goes through
        private IEnumerator FSM()
        {
            while (true)
            {
                Debug.Log(state);
                switch (state)
                {
                    case State.PATROL:
                        Patrol();
                        break;
                    case State.BATTLE:
                        //Battle();
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
        

        private void Patrol()
        {
            Vector3 targetPos = patrolOffset * cameraOffset;
            float step = Mathf.Max(Vector3.Distance(camera.transform.localPosition, targetPos), cameraSpeedMinimum) * Time.deltaTime;
            camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, targetPos, step);

            camera.LookAt(transform);
        }

        private void Battle()
        {
            /*Vector3 targetPos = battleOffset * m_cameraAngle - camera.forward * 15 + cameraOffset;
            Debug.Log(targetPos);
            float step = Mathf.Max(Vector3.Distance(camera.transform.position, battleOffset * m_cameraAngle), cameraSpeedMinimum) * Time.deltaTime;
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPos, step);*/
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

        public void PlaceOnFirstRoom(Transform firstRoom)
        {
            //TODO: place player first
            /*m_target = firstRoom.position + centerOfRoom;
            Debug.Log(m_target);
            Debug.Log(centerOfRoom);

            trackPlayer();
            camera.transform.localPosition = -camera.forward * battleOffset + cameraOffset;

            transform.position = m_target;
            transform.position = firstRoom.position;*/
            PlaceOnRoom(firstRoom);
        }
        private void PlaceOnRoom(Transform room)
        {
            //TODO: this might need to switch to patrol
            transform.position = room.position + centerOfRoom;
            camera.transform.localPosition = battleOffset * cameraOffset;

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
            Debug.Log("camera battle start");

            PlaceOnRoom(battleRoom);
            //background.GetComponent<BackgroundWave>().activateMatch(true);
        }

        public IEnumerator endBattleCamera()
        {
            state = CameraControl.State.PATROL;
            Debug.Log("camera battle end");

            //yield return shakeCamera();
            yield return new WaitForSeconds(.05f);
            //background.GetComponent<BackgroundWave>().activateMatch(false);
        }

        public void gameOverCamera()
        {
            state = CameraControl.State.GAMEOVER;
        }
    }
}