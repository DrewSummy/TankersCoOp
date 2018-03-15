using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Completed;

namespace Completed
{
    public class GUI_Pause : MonoBehaviour
    {
        public Text title;
        public GameObject panel;
        public GameObject tank;
        public GameObject death;

        // Buttons
        public UnityEngine.UI.Selectable currentButton;
        public GameObject Resume;
        public GameObject Restart;
        public GameObject RestartYes;
        public GameObject RestartNo;
        public GameObject Quit;
        public GameObject QuitYes;
        public GameObject QuitNo;
        
        public Transform killHolder1;
        public Transform killHolder2;
        private GameObject P1;
        private GameObject P2;
        public GameObject killCountText;
        public Material[] tankColors;

        public GameMaster GM;
        public GUI_Controller controllerGUI;
        public bool paused = false;

        private Vector3 alignment = new Vector3(-25, 0, 0);
        private Vector3 textOffset = new Vector3(40, 0, 0);
        private Vector3 killOffset = new Vector3(12, 0, 0);
        private Vector3 deathOffset = new Vector3(20, 0, 0);
        



        void Awake()
        {
            P1 = GameObject.FindGameObjectWithTag("Player");
        }

        // Use this for initialization
        void Start()
        {
            // Load in the tank colors being used from the Resources folder in assets.
            tankColors = Resources.LoadAll<Material>("Prefab/GameObjectPrefab/TankPrefab/TankColors");
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
            //PlaceP2Kills();
        }

        private void PlaceP1Kills()
        {
            // Update player 1's kills.
            int[] P1Kills = P1.GetComponent<TankPlayer>().killCounter;
            int P1Deaths = P1.GetComponent<TankPlayer>().deaths;


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
                        tankImage.transform.SetParent(killHolder1.GetChild(killType));
                        tankImage.transform.SetSiblingIndex(0);
                        tankImage.transform.position = killHolder1.GetChild(killType).position + kill * killOffset + alignment;
                        tankImage.GetComponentsInChildren<Image>()[1].color = tankColors[killType].color;
                    }
                }
                // Else, just print a counter representing the number of kills like "x 26".
                else
                {
                    GameObject tankImage = Instantiate(tank) as GameObject;
                    tankImage.transform.SetParent(killHolder1.GetChild(killType));
                    tankImage.transform.SetSiblingIndex(0);
                    tankImage.transform.position = killHolder1.GetChild(killType).position + alignment;
                    tankImage.GetComponentsInChildren<Image>()[1].color = tankColors[killType].color;

                    GameObject tankText = Instantiate(killCountText) as GameObject;
                    tankText.transform.SetParent(killHolder1.GetChild(killType));
                    tankText.GetComponent<Text>().text = "x  " + P1Kills[killType];
                    tankText.GetComponent<Text>().fontSize = 14;
                    tankText.transform.position = killHolder1.GetChild(killType).position + textOffset + alignment;
                    tankText.GetComponent<Text>().enabled = true;
                }
            }

            P1Deaths = 5;
            if (P1Deaths < 5)
            {
                for (int deathi = 0; deathi < P1Deaths; deathi++)
                {
                    GameObject tankImage = Instantiate(death) as GameObject;
                    tankImage.transform.SetParent(killHolder1.GetChild(P1Kills.Length));
                    tankImage.transform.SetSiblingIndex(0);
                    tankImage.transform.position = killHolder1.GetChild(P1Kills.Length).position + deathi * deathOffset + alignment;
                }
            }
            else
            {
                GameObject tankImage = Instantiate(death) as GameObject;
                tankImage.transform.SetParent(killHolder1.GetChild(P1Kills.Length));
                tankImage.transform.SetSiblingIndex(0);
                tankImage.transform.position = killHolder1.GetChild(P1Kills.Length).position + alignment;

                GameObject tankText = Instantiate(killCountText) as GameObject;
                tankText.transform.SetParent(killHolder1.GetChild(P1Kills.Length));
                tankText.GetComponent<Text>().text = "x  " + P1Deaths;
                tankText.GetComponent<Text>().fontSize = 14;
                tankText.transform.position = killHolder1.GetChild(P1Kills.Length).position + textOffset + alignment;
                tankText.GetComponent<Text>().enabled = true;
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
                    //tankText.GetComponent<Text>().text = "x  " + P1Kills[killType];
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

                //TODO: display map

                paused = true;

                // Enable the controller.
                controllerGUI.enabled = true;
            }
        }
        public void Unpause()
        {
            if (enabled)
            {
                Time.timeScale = 1f;
                RemoveKills();
                panel.SetActive(false);

                //TODO: undisplay map

                paused = false;

                // Disable the controller.
                controllerGUI.enabled = false;
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

        // End and restart game.
        private void endGame()
        {
            currentButton = Resume.GetComponent<Button>();
            Unpause();
            GM.endGame();
        }
        private void restartGame()
        {
            currentButton = Resume.GetComponent<Button>();
            Unpause();
            GM.restart();
        }

        // Functions called by GUI_Controller
        public void back()
        {
            if (currentButton == Resume.GetComponent<Button>() ||
                currentButton == Restart.GetComponent<Button>() ||
                currentButton == Quit.GetComponent<Button>())
            {
                resume();
            }
            else if (currentButton == RestartYes.GetComponent<Button>() ||
                currentButton == RestartNo.GetComponent<Button>())
            {
                currentButton = Restart.GetComponent<Button>();
            }
            else if (currentButton == QuitYes.GetComponent<Button>() ||
                currentButton == QuitNo.GetComponent<Button>())
            {
                currentButton = Quit.GetComponent<Button>();
            }

            currentButton.Select();
        }
        public void select()
        {
            if (currentButton.GetComponent<Button>() == Resume.GetComponent<Button>())
            {
                Unpause();
            }
            else if (currentButton.GetComponent<Button>() == Restart.GetComponent<Button>())
            {
                currentButton = RestartYes.GetComponent<Button>();
                Restart.GetComponent<ButtonRestart>().restart();
            }
            else if (currentButton.GetComponent<Button>() == Quit.GetComponent<Button>())
            {
                currentButton = QuitYes.GetComponent<Button>();
                Quit.GetComponent<ButtonQuit>().quit();
            }
            else if (currentButton.GetComponent<Button>() == RestartYes.GetComponent<Button>())
            {
                Restart.GetComponent<Button>().Select();
                currentButton = Resume.GetComponent<Button>();
                currentButton.Select();
                restartGame();
            }
            else if (currentButton.GetComponent<Button>() == RestartNo.GetComponent<Button>())
            {
                currentButton = Restart.GetComponent<Button>();
            }
            else if (currentButton.GetComponent<Button>() == QuitYes.GetComponent<Button>())
            {
                Quit.GetComponent<Button>().Select();
                currentButton = Resume.GetComponent<Button>();
                currentButton.Select();
                endGame();
            }
            else if (currentButton.GetComponent<Button>() == QuitNo.GetComponent<Button>())
            {
                currentButton = Quit.GetComponent<Button>();
            }

            currentButton.Select();
        }
        public void up()
        {
            currentButton = currentButton.GetComponent<Button>().navigation.selectOnUp;
            currentButton.Select();
        }
        public void down()
        {
            currentButton = currentButton.GetComponent<Button>().navigation.selectOnDown;
            currentButton.Select();
        }
    }
}