using UnityEngine;
using System.Collections;

namespace Completed
{

    public class Gate : MonoBehaviour
    {

        public RoomManager parentRoomScript;
        public AudioSource gateAudioSource;
        public AudioClip closeAudio;
        public AudioClip lastRoomAudio;
        private bool audioPlayed = false;
        private float gateHeight = 6f;
        private Vector3 originalPos;
        private Quaternion originalRot;
        public Transform gate;
        public BoxCollider boundary;
        public bool triggered = false;
        private bool lastRoomAudioBool = true;


        private void Awake()
        {
            gate = GetComponent<Transform>();
            gateAudioSource = GetComponent<AudioSource>();

            boundary = gate.GetComponentsInChildren<BoxCollider>()[1];
            //Debug.Log(boundary.name);
        }

        private void Start()
        {
            originalPos = gate.position;

            closeAudio = Resources.Load("38724__metamorphmuses__long-deep-bass-boom") as AudioClip;
            lastRoomAudio = Resources.Load("34942__sir-yaro__download-complete") as AudioClip;
            //gateAudioSource.volume = 0f; //TODO
            gateAudioSource.clip = closeAudio;
            gateAudioSource.playOnAwake = false;
        }

        void Update()
        {

        }

        // Lowers a door slowly.
        public void lowerDoorSlow()
        {
            // How fast it shakes.
            float speed = 40f;
            // How much it shakes.
            float amount = .08f;

            if (!audioPlayed)
            {
                gateAudioSource.Play();
                audioPlayed = true;
            }

            float lowerSpeed = gateHeight / (closeAudio.length + 1.0f);
            if (gate.position.y > -gateHeight / 2)
            {
                float Angle2Amount = (Mathf.Cos(Time.time * 30) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, 0, 0);

                // Lowering
                Vector3 lowering = Vector3.down * lowerSpeed * Time.deltaTime;
                gate.Translate(lowering);
                // Rotating
                float AngleAmount = (Mathf.Cos(Time.time * speed) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, AngleAmount, 0);
            }
            else
            {
                gate.localRotation = originalRot;
                gate.transform.position = originalPos + Vector3.down * 4;
            }
        }

        // Lowers a door quickly.
        public void lowerDoorFast()
        {
            float speed = 40f; // How fast it shakes.
            float amount = .08f; // How much it shakes.

            if (!audioPlayed)
            {
                gateAudioSource.pitch = 2f;
                gateAudioSource.Play();
                audioPlayed = true;
            }

            float lowerSpeed = 4 * gateHeight / (closeAudio.length + 1.0f);
            if (gate.position.y > -gateHeight / 2)
            {
                float Angle2Amount = (Mathf.Cos(Time.time * 30) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, 0, 0);
                
                // Lowering
                Vector3 lowering = Vector3.down * lowerSpeed * Time.deltaTime;
                gate.Translate(lowering);
                // Rotating
                float AngleAmount = (Mathf.Cos(Time.time * speed) * 180) / Mathf.PI * amount;
                gate.localRotation = Quaternion.Euler(Angle2Amount, AngleAmount, 0);
            }
            else
            {
                gate.localRotation = originalRot;
                gate.transform.position = originalPos + Vector3.down * 4;
            }
        }

        // Lowers the last room's door.
        public void lowerDoorLastRoom()
        {
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
            else
            {
                    lowerDoorSlow();
            }
        }

    protected void OnCollisionEnter(Collision collisionInfo)
        {
            // The object has collided with another projectile.
            if (collisionInfo.transform.tag == "Player")
            {
                if (parentRoomScript.roomCompleted)
                {
                    triggered = true;
                    boundary.enabled = false;
                }
            }
        }
    }
}