using UnityEngine;
using System.Collections;
using Completed;

namespace Completed
{

    public class Gate : MonoBehaviour
    {

        public RoomManager parentRoomScript;    // Reference to RoomManager of the parent room.
        public AudioSource gateAudioSource;     // Reference to the audio source used to play gate sounds.
        public AudioClip closeAudio;            // Reference to the audio clip used to play the gate audio source.
        public AudioClip lastRoomAudio;         // Reference to the audio clip used to play the gate audio source.
        private bool audioPlayed = false;       // Boolean for whether the audio from gateAudioSource played.
        private float gateHeight = 6f;          // The height of the gate.
        private Vector3 originalPos;            // The original position of the gate.
        private Quaternion originalRot;         // The original rotation of the gate.
        public Transform gate;                  // Reference to the gate transform.
        public BoxCollider triggerBoundary;     // Reference to the wall boundary.
        public BoxCollider wall;                // Reference to the wall boundary before the room is completed.
        public BoxCollider wallA;               // Reference to the wall boundary once the room is completed.
        public BoxCollider wallB;               // Reference to the wall boundary once the room is completed.
        public bool triggered = false;          // Boolean for whether the gate was triggered.

        private bool lastRoomAudioBool;         // Boolean for whether this is a gate of the last room.
        private float speed = 40f;              // The speed the gate shakes at on lowering.
        private float amount = .08f;            // The distance the gate shakes at on lowering.


        private void Awake()
        {
            gate = GetComponent<Transform>();
            gateAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            originalPos = gate.position;

            closeAudio = Resources.Load("Prefab/Audio/38724__metamorphmuses__long-deep-bass-boom") as AudioClip;
            lastRoomAudio = Resources.Load("Prefab/Audio/34942__sir-yaro__download-complete") as AudioClip;
            //gateAudioSource.volume = 0f; //TODO
            gateAudioSource.clip = closeAudio;
            gateAudioSource.playOnAwake = false;
        }

        // Lowers a door slowly.
        public IEnumerator lowerDoorSlow()
        {
            endRoomColliders();

            if (!audioPlayed)
            {
                gateAudioSource.Play();
                audioPlayed = true;
            }

            float lowerSpeed = gateHeight / (closeAudio.length + 1.0f);
            while (gate.position.y > -gateHeight / 2)
            {
                float Angle2Amount = (Mathf.Cos(Time.time * 30) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, 0, 0);

                // Lowering
                Vector3 lowering = Vector3.down * lowerSpeed * Time.deltaTime;
                gate.Translate(lowering);
                // Rotating
                float AngleAmount = (Mathf.Cos(Time.time * speed) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, AngleAmount, 0);
                yield return new WaitForSeconds(.01f);
            }
            gate.localRotation = originalRot;
            gate.transform.position = originalPos + Vector3.down * 4;
        }

        // Lowers a door quickly.
        public IEnumerator lowerDoorFast()
        {
            endRoomColliders();

            float speed = 40f; // How fast it shakes.
            float amount = .08f; // How much it shakes.

            if (!audioPlayed)
            {
                gateAudioSource.pitch = 2f;
                gateAudioSource.Play();
                audioPlayed = true;
            }

            float lowerSpeed = 4 * gateHeight / (closeAudio.length + 1.0f);
            while (gate.position.y > -gateHeight / 2)
            {
                float Angle2Amount = (Mathf.Cos(Time.time * 30) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, 0, 0);

                // Lowering
                Vector3 lowering = Vector3.down * lowerSpeed * Time.deltaTime;
                gate.Translate(lowering);
                // Rotating
                float AngleAmount = (Mathf.Cos(Time.time * speed) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, AngleAmount, 0);
                yield return new WaitForSeconds(.01f);
            }
            gate.localRotation = originalRot;
            gate.transform.position = originalPos + Vector3.down * 4;
        }

        // Lowers the last room's door.
        public void lowerDoorLastRoom()
        {
            endRoomColliders();

            // Play the last room audio, then lower the door slowly.
            if (lastRoomAudioBool)
            {
                if (!audioPlayed)
                {
                    gateAudioSource.clip = lastRoomAudio;
                    gateAudioSource.Play();
                    audioPlayed = true;
                }

                if (!gateAudioSource.isPlaying)
                {
                    lastRoomAudioBool = false;
                    audioPlayed = false;
                    gateAudioSource.clip = closeAudio;
                }
            }
            StartCoroutine(lowerDoorSlow());
        }

        // Change the box colliders of the walls when the room is complete.
        private void endRoomColliders()
        {
            triggerBoundary.enabled = false;
            wall.enabled = false;
            wallA.enabled = true;
            wallB.enabled = true;
        }

        protected void OnCollisionEnter(Collision collisionInfo)
        {
            Debug.Log("hmm");
            // The object has collided with another projectile.
            if (collisionInfo.transform.tag == "Player")
            {
                if (parentRoomScript.roomCompleted)
                {
                    //TODO: comment this out and test
                    //triggered = true;
                    //boundary.enabled = false;
                    Debug.Log("room is entered");
                    parentRoomScript.entered();
                }
                else
                {
                    parentRoomScript.startBeginningBattle();
                }
            }
        }

        protected void OnCollisionExit(Collision collisionInfo)
        {
            // The object has collided with another projectile.
            if (collisionInfo.transform.tag == "Player")
            {
                if (parentRoomScript.roomCompleted)
                {
                    // Update the minimap.
                    GameObject.FindGameObjectWithTag("MiniMap").GetComponent<GUI_MiniMap>().movePlayer();

                    // Reset the current room of the tank.
                    if (!collisionInfo.transform.GetComponent<TankPlayer>().battling)
                    {
                        collisionInfo.transform.GetComponent<TankPlayer>().currentRoom = parentRoomScript.gameObject;
                    }

                }
            }
        }
        ///////////////////////////FIND OUT WHEN A ROOM IS ENTERED
    }
}