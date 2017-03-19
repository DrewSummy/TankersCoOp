using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Completed;

namespace Completed
{
    public class GUI_Pause : MonoBehaviour
    {

        public GameObject panel;
        public GameObject tank;
        public GameObject Resume;
        public GameObject Restart;
        public GameObject RestartYes;
        public GameObject RestartNo;
        public GameObject Quit;
        public GameObject QuitYes;
        public GameObject QuitNo;
        private int[] P1Kills;
        private Transform killHolder1;
        private Transform killHolder2;
        private GameObject P1;
        private GameObject P2;
        public GameObject killCountText;
        public Material[] tankColors;

        private bool enabled = false;                              // Bool for when pause is usable.


        void Awake()
        {
            P1 = GameObject.FindGameObjectWithTag("Player");
        }

        // Use this for initialization
        void Start()
        {
            killHolder1 = new GameObject("KillHolder1").transform;
            killHolder1.SetParent(panel.transform);
            killHolder1.position = panel.transform.position + new Vector3(10, 0, 0);
            killHolder2 = new GameObject("KillHolder2").transform;
            killHolder2.SetParent(panel.transform);
            killHolder2.position = panel.transform.position + new Vector3(230, 0, 0);

            // Load in the tank colors being used from the Resources folder in assets.
            tankColors = Resources.LoadAll<Material>("TankResources/TankColors");
        }

        private void PlaceMenu()
        {
            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (tank.GetComponent<TankPlayer>().m_PlayerNumber == 1)
                {
                    P1 = tank;
                }
                else
                {
                    P2 = tank;
                }
            }

            PlaceP1Kills();
            PlaceP2Kills();
        }

        private void PlaceP1Kills()
        {
            // Update player 1's kills.
            int[] P1Kills = P1.GetComponent<TankPlayer>().killCounter;
            //TODO: the same for player 2


            for (int killType = 0; killType < P1Kills.Length; killType++)
            {
                // This is to only count to 10.
                if (P1Kills[killType] <= 10)
                {
                    for (int kill = 0; kill < P1Kills[killType]; kill++)
                    {
                        // This is to seperate by groups of 5.
                        float offset = 8 * Mathf.Floor(kill / 5);

                        GameObject tankImage = Instantiate(tank) as GameObject;
                        tankImage.transform.SetParent(killHolder1);
                        tankImage.transform.position = killHolder1.position + new Vector3(0 + kill * 5 + offset, 20 - killType * 25, 0);
                        tankImage.GetComponent<Image>().color = tankColors[killType].color;
                        tankImage.GetComponent<RectTransform>().localScale = new Vector3(.1f, .1f, 1);
                        killCountText.GetComponent<Text>().enabled = false;
                    }
                }
                // Else, just print a counter representing the number of kills like "x 26".
                else
                {
                    GameObject tankImage = Instantiate(tank) as GameObject;
                    tankImage.transform.SetParent(killHolder1);
                    tankImage.transform.position = killHolder1.position + new Vector3(0, 20 - killType * 25, 0); ;
                    tankImage.GetComponent<Image>().color = tankColors[killType].color;
                    tankImage.GetComponent<RectTransform>().localScale = new Vector3(.1f, .1f, 1);

                    GameObject tankText = Instantiate(killCountText) as GameObject;
                    tankText.transform.SetParent(killHolder1);
                    tankText.GetComponent<Text>().text = "x  " + P1Kills[killType];
                    tankText.GetComponent<Text>().fontSize = 14;
                    tankText.transform.position = killHolder1.position + new Vector3(129, 22 - killType * 25, 0);
                    tankText.GetComponent<Text>().enabled = true;
                }
            }
        }

        private void PlaceP2Kills()
        {
            // Update player 1's kills.
            int[] P2Kills = P2.GetComponent<TankPlayer>().killCounter;
            //TODO: the same for player 2


            for (int killType = 0; killType < P2Kills.Length; killType++)
            {
                // This is to only count to 10.
                if (P2Kills[killType] <= 10)
                {
                    for (int kill = 0; kill < P2Kills[killType]; kill++)
                    {
                        // This is to seperate by groups of 5.
                        float offset = 8 * Mathf.Floor(kill / 5);

                        GameObject tankImage = Instantiate(tank) as GameObject;
                        tankImage.transform.SetParent(killHolder2);
                        tankImage.transform.position = killHolder2.position + new Vector3(0 + kill * 5 + offset, 20 - killType * 25, 0);
                        tankImage.GetComponent<Image>().color = tankColors[killType].color;
                        tankImage.GetComponent<RectTransform>().localScale = new Vector3(.1f, .1f, 1);
                        killCountText.GetComponent<Text>().enabled = false;
                    }
                }
                // Else, just print a counter representing the number of kills like "x 26".
                else
                {
                    GameObject tankImage = Instantiate(tank) as GameObject;
                    tankImage.transform.SetParent(killHolder2);
                    tankImage.transform.position = killHolder2.position + new Vector3(0, 20 - killType * 25, 0); ;
                    tankImage.GetComponent<Image>().color = tankColors[killType].color;
                    tankImage.GetComponent<RectTransform>().localScale = new Vector3(.1f, .1f, 1);

                    GameObject tankText = Instantiate(killCountText) as GameObject;
                    tankText.transform.SetParent(killHolder2);
                    tankText.GetComponent<Text>().text = "x  " + P1Kills[killType];
                    tankText.GetComponent<Text>().fontSize = 14;
                    tankText.transform.position = killHolder2.position + new Vector3(129, 22 - killType * 25, 0);
                    tankText.GetComponent<Text>().enabled = true;
                }
            }
        }


        private void RemoveKills()
        {
            foreach (Transform child in killHolder1.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in killHolder2.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        
        public void Pause()
        {
            if (enabled)
            {
                PlaceMenu();
                panel.SetActive(true);
                Time.timeScale = 0f;

                // Set the resume button as selected. For some reason there has to be a different button selected previously.
                Restart.GetComponent<Button>().Select();
                Resume.GetComponent<Button>().Select();
            }
        }
        public void Unpause()
        {
            if (enabled)
            {
                Time.timeScale = 1f;
                RemoveKills();
                panel.SetActive(false);
            }
        }

        // OnClick button for resume. Needs to be here because it calls Unpause.
        public void resume()
        {
            Unpause();
        }

        public void onEnterResume()
        {
            Debug.Log("enter res");
            //setinactive the small button
            //setactive the large button
            // move large button
            // correct the other buttons
            // add sound like sliding on metal
        }

        // Called when game starts.
        public void enablePause()
        {
            // Pause and Unpause are now usable.
            enabled = true;
        }
    }
}