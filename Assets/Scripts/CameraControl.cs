using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public GameObject m_Tank;                   // Reference to the player's transform.
    public bool battling = false;

    private Vector3 m_CameraPosRel;             // The offset of the camera from the tank.
    private float m_RoomLength = 50;            // The length of a side of the square room.
    private float m_WallThickness = 1;
    private float stepLength;
    private Vector3 m_target;
    private Vector3 patrolOffset;
    private Vector3 battleOffset;
    private Quaternion m_cameraAngle = new Quaternion 
        (.6f, 0, 0, .8f);                       // The rotation put on the camera.
    private float cameraSpeedMinimum = 45;

    //TODO: figure out camera blur to hide the levels on the side during a battle


    private void Start()
    {
        m_Tank = GameObject.FindWithTag("Player");
        // TODD: set up a better method for angling the camera
        transform.rotation = m_cameraAngle;

        stepLength = m_RoomLength + 2 * m_WallThickness;

        battleOffset.Set(0, 40, -15);
        patrolOffset = battleOffset * 1.5f;
    }

    private void Update()
    {
        UpdateTargetPos();
    }

    private void UpdateTargetPos()
    {
        m_target = new Vector3(Mathf.Floor((m_Tank.transform.position.x + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2,
            0,
            Mathf.Floor((m_Tank.transform.position.z + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2);

    }

    private void FixedUpdate()
    {
        // Move the camera depending on the tank's position.
        Move();
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

    public void PlaceOnFirstRoom(Transform room)
    {
        Vector3 aboveRoom = new Vector3(Mathf.Floor((room.position.x + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2,
            0,
            Mathf.Floor((room.transform.position.z + m_WallThickness) / stepLength) * stepLength + m_RoomLength / 2);
        
        transform.position = aboveRoom + battleOffset;
    }
}