using UnityEngine;
using System.Collections;

namespace Completed
{
    public class CameraControl : MonoBehaviour
    {
        public GameObject m_Player1;                   // Reference to the player's transform.
        public GameObject m_Player2;                   // Reference to the player's transform.
        public bool battling = false;
        public bool gameOver = false;

        private Vector3 m_CameraPosRel;             // The offset of the camera from the tank.
        private float m_RoomLength = 50;            // The length of a side of the square room.
        private float m_WallThickness = 1;
        private float stepLength = 52;              // = m_RoomLength + 2 * m_WallThickness;
        private Vector3 m_target;
        private Vector3 patrolOffset;               // The offsets are set relative to each other.
        private Vector3 battleOffset = new Vector3(0, 40, -15);
        private Vector3 deadOffset;
        private Quaternion m_cameraAngle = new Quaternion
            (.6f, 0, 0, .8f);                       // The rotation put on the camera.
        private float cameraSpeedMinimum = 45;
        private float cameraSpeedEnding = 5;

        private float shakeRadius = .25f;
        private float shakeTime = 1f;

        //TODO: this can be more efficient by not using update but instead calling functions in the camera

        //TODO: figure out camera blur to hide the levels on the side during a battle
        


        private void Start()
        {
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (tank.GetComponent<TankPlayer>().m_PlayerNumber == 1)
                {
                    m_Player1 = tank;
                }
                else
                {
                    m_Player2 = tank;
                }
            }
            //playerScript2 = m_Player2.GetComponent<TankPlayer>();
            // TODD: set up a better method for angling the camera
            transform.rotation = m_cameraAngle;

            //stepLength = m_RoomLength + 2 * m_WallThickness;

            battleOffset.Set(0, 40, -15);
            patrolOffset = battleOffset * 1.5f;
            deadOffset = battleOffset * .5f;
        }

        private void Update()
        {
            UpdateTargetPos();
        }

        private void UpdateTargetPos()
        {
            m_target = new Vector3(Mathf.Floor((m_Player1.transform.position.x + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2,
                0,
                Mathf.Floor((m_Player1.transform.position.z + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2);
        }

        private void ZoomOut()
        {
            float step = Mathf.Max(Vector3.Distance(transform.position, m_target + deadOffset), cameraSpeedEnding) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, m_target + deadOffset, step);
        }

        private void FixedUpdate()
        {
            // Move the camera depending on the tank's position.if (playerScript1.alive)
            if (!gameOver)
            {
                Move();
            }
            else
            {
                //TODO: this is searching for the player still and so it moves off screen
                ZoomOut();
            }
        }

        private void Move()
        {
            if (battling)
            {
                float step = Mathf.Max(Vector3.Distance(transform.position, m_target + battleOffset), cameraSpeedMinimum) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, m_target + battleOffset, step);
            }
            else
            {
                float step = Mathf.Max(Vector3.Distance(transform.position, m_target + patrolOffset), cameraSpeedMinimum) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, m_target + patrolOffset, step);
            }
        }

        public void PlaceOnFirstRoom(Vector2 firstRoomCoord)//Transform room)
        {
            //TODO: place player first
            m_target = new Vector3(Mathf.Floor((m_Player1.transform.position.x + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2,
            0,
            Mathf.Floor((m_Player1.transform.position.z + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2);
            
            transform.position = m_target + patrolOffset;
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
            transform.GetChild(0).transform.position = camPos;
        }
    }
}