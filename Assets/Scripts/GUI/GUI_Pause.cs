using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Completed;
using TeamUtility.IO;

namespace Completed
{
    public class GUI_Pause : MonoBehaviour
    {
        public Canvas canvas;
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
        public GameObject crown1;
        public GameObject crown2;

        public GUI_MiniMap guiMiniMap;
        public GameObject mapInstance;
        private bool full;

        public GameMaster GM;
        public GUI_Controller controllerGUI;
        public bool paused = false;

        private int fSize = 25;

        private Vector3 alignment = new Vector3(-45, 0, 0);
        private Vector3 textOffset = new Vector3(100, 0, 0);
        private Vector3 killOffset = new Vector3(12, 0, 0);
        private Vector3 deathOffset = new Vector3(28, 0, 0);
        private Vector3 deathAlignment = new Vector3(0, -15, 0);

        private Vector3 mapOffset = new Vector3(0, 0, 0);

        private AudioSource ad;
        private AudioClip lowClick;
        private AudioClip click;
        private AudioClip hiClick;




        void Awake()
        {
            P1 = GameObject.FindGameObjectWithTag("Player");

            ad = gameObject.AddComponent<AudioSource>();
            ad.playOnAwake = false;

            lowClick = Resources.Load("Prefab/Audio/lowClick") as AudioClip;
            click = Resources.Load("Prefab/Audio/click") as AudioClip;
            hiClick = Resources.Load("Prefab/Audio/hiClick") as AudioClip;
        }

        // Use this for initialization
        void Start()
        {
            // Load in the tank colors being used from the Resources folder in assets.
            tankColors = Resources.LoadAll<Material>("Prefab/GameObjectPrefab/TankPrefab/TankColors");
        }

        protected void PlaceMenu()
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
            if (P2)
            {
                PlaceP2Kills();
            }


            PlaceCrown();
        }

        protected void PlaceP1Kills()
        {
            // Update player 1's kills.
            int[] P1Kills = P1.GetComponent<TankPlayer>().killCounter;
            int P1Deaths = P1.GetComponent<TankPlayer>().deathCounter;


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
                    tankText.GetComponent<Text>().fontSize = fSize;
                    tankText.transform.position = killHolder1.GetChild(killType).position + textOffset + alignment;
                    tankText.GetComponent<Text>().enabled = true;
                }
            }

            if (P1Deaths <= 5)
            {
                for (int deathi = 0; deathi < P1Deaths; deathi++)
                {
                    GameObject tankImage = Instantiate(death) as GameObject;
                    tankImage.transform.SetParent(killHolder1.GetChild(P1Kills.Length));
                    tankImage.transform.SetSiblingIndex(0);
                    tankImage.transform.position = killHolder1.GetChild(P1Kills.Length).position + deathi * deathOffset + alignment + deathAlignment;
                }
            }
            else
            {
                GameObject tankImage = Instantiate(death) as GameObject;
                tankImage.transform.SetParent(killHolder1.GetChild(P1Kills.Length));
                tankImage.transform.SetSiblingIndex(0);
                tankImage.transform.position = killHolder1.GetChild(P1Kills.Length).position + alignment + deathAlignment;

                GameObject tankText = Instantiate(killCountText) as GameObject;
                tankText.transform.SetParent(killHolder1.GetChild(P1Kills.Length));
                tankText.GetComponent<Text>().text = "x  " + P1Deaths;
                tankText.GetComponent<Text>().fontSize = fSize;
                tankText.transform.position = killHolder1.GetChild(P1Kills.Length).position + textOffset + alignment + deathAlignment;
                tankText.GetComponent<Text>().enabled = true;
            }
        }

        protected void PlaceP2Kills()
        {
            // Update player 2's kills.
            int[] P2Kills = P2.GetComponent<TankPlayer>().killCounter;
            int P2Deaths = P2.GetComponent<TankPlayer>().deathCounter;


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
                        tankImage.transform.SetParent(killHolder2.GetChild(killType));
                        tankImage.transform.SetSiblingIndex(0);
                        tankImage.transform.position = killHolder2.GetChild(killType).position + kill * killOffset + alignment;
                        tankImage.GetComponentsInChildren<Image>()[1].color = tankColors[killType].color;
                    }
                }
                // Else, just print a counter representing the number of kills like "x 26".
                else
                {
                    GameObject tankImage = Instantiate(tank) as GameObject;
                    tankImage.transform.SetParent(killHolder2.GetChild(killType));
                    tankImage.transform.SetSiblingIndex(0);
                    tankImage.transform.position = killHolder2.GetChild(killType).position + alignment;
                    tankImage.GetComponentsInChildren<Image>()[1].color = tankColors[killType].color;

                    GameObject tankText = Instantiate(killCountText) as GameObject;
                    tankText.transform.SetParent(killHolder2.GetChild(killType));
                    tankText.GetComponent<Text>().text = "x  " + P2Kills[killType];
                    tankText.GetComponent<Text>().fontSize = fSize;
                    tankText.GetComponent<Text>().color = Color.gray;
                    tankText.transform.position = killHolder2.GetChild(killType).position + textOffset + alignment;
                    tankText.GetComponent<Text>().enabled = true;
                }
            }

            if (P2Deaths <= 5)
            {
                for (int deathi = 0; deathi < P2Deaths; deathi++)
                {
                    GameObject tankImage = Instantiate(death) as GameObject;
                    tankImage.transform.SetParent(killHolder2.GetChild(P2Kills.Length));
                    tankImage.transform.SetSiblingIndex(0);
                    tankImage.transform.position = killHolder2.GetChild(P2Kills.Length).position + deathi * deathOffset + alignment + deathAlignment;
                }
            }
            else
            {
                GameObject tankImage = Instantiate(death) as GameObject;
                tankImage.transform.SetParent(killHolder2.GetChild(P2Kills.Length));
                tankImage.transform.SetSiblingIndex(0);
                tankImage.transform.position = killHolder2.GetChild(P2Kills.Length).position + alignment + deathAlignment;

                GameObject tankText = Instantiate(killCountText) as GameObject;
                tankText.transform.SetParent(killHolder2.GetChild(P2Kills.Length));
                tankText.GetComponent<Text>().text = "x  " + P2Deaths;
                tankText.GetComponent<Text>().fontSize = fSize;
                tankText.transform.position = killHolder2.GetChild(P2Kills.Length).position + textOffset + alignment + deathAlignment;
                tankText.GetComponent<Text>().enabled = true;
            }
        }

        protected void PlaceCrown()
        {
            if (!P2)
            {
                crown1.gameObject.SetActive(false);
                crown2.gameObject.SetActive(false);
                return;
            }

            int p1Kills = 0;
            int p2Kills = 0;
            int[] P1Kills = P1.GetComponent<TankPlayer>().killCounter;
            int[] P2Kills = P2.GetComponent<TankPlayer>().killCounter;


            for (int killType = 0; killType < P1Kills.Length; killType++)
            {
                p1Kills += P1Kills[killType];
            }
            for (int killType = 0; killType < P2Kills.Length; killType++)
            {
                p2Kills += P2Kills[killType];
            }

            Debug.Log("here");
            crown1.gameObject.SetActive(false);
            crown2.gameObject.SetActive(false);
            if (p2Kills > p1Kills)
            {
                Debug.Log("2");
                crown1.gameObject.SetActive(false);
                crown2.gameObject.SetActive(true);
            }
            else if (p2Kills < p1Kills)
            {
                Debug.Log("1");
                crown1.gameObject.SetActive(true);
                crown2.gameObject.SetActive(false);
            }
        }

        protected void RemoveKills()
        {
            foreach (Transform child in killHolder1.transform)
            {

                foreach (Transform image in child)
                {

                    GameObject.Destroy(image.gameObject);
                }
            }
            foreach (Transform child in killHolder2.transform)
            {

                foreach (Transform image in child)
                {

                    GameObject.Destroy(image.gameObject);
                }
            }
        }
        
        protected void RemoveMap()
        {
            foreach (Transform child in mapInstance.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public virtual void Pause(PlayerID pid)
        {
            if (enabled)
            {
                PlaceMenu();
                panel.SetActive(true);
                Time.timeScale = 0f;

                // Set the resume button as selected. For some reason there has to be a different button selected previously.
                Restart.GetComponent<Button>().Select();
                Resume.GetComponent<Button>().Select();

                // Display map and hide the GUI_MiniMap's map.
                guiMiniMap.Hide();
                guiMiniMap.PauseDisplay();

                paused = true;

                // Enable the controller.
                controllerGUI.enabled = true;
                controllerGUI.PID = pid;
            }
        }

        public virtual void Unpause()
        {
            if (enabled)
            {
                currentButton = Resume.GetComponent<Button>();

                Time.timeScale = 1f;
                RemoveKills();
                panel.SetActive(false);

                // Reveal the GUI_MiniMap's map.
                guiMiniMap.Reveal();

                // Disable the controller.
                controllerGUI.enabled = false;

                paused = false;
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
        protected virtual void endGame()
        {
            currentButton = Resume.GetComponent<Button>();
            Unpause();
            GM.endGame();
        }
        protected virtual void restartGame()
        {
            RemoveMap();
            guiMiniMap.clearMap();
            currentButton = Resume.GetComponent<Button>();
            Unpause();
            GM.restart();
        }

        // Functions called by GUI_Controller
        public virtual void back()
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
        public virtual void select()
        {
            if (currentButton.GetComponent<Button>() == Resume.GetComponent<Button>())
            {
                Unpause();
            }
            else if (currentButton.GetComponent<Button>() == Restart.GetComponent<Button>())
            {
                currentButton = RestartYes.GetComponent<Button>();
                Restart.GetComponent<ButtonRestart>().restart();
                PlayLowClick();
            }
            else if (currentButton.GetComponent<Button>() == Quit.GetComponent<Button>())
            {
                currentButton = QuitYes.GetComponent<Button>();
                PlayHiClick();
                Quit.GetComponent<ButtonQuit>().quit();
            }
            else if (currentButton.GetComponent<Button>() == RestartYes.GetComponent<Button>())
            {
                Restart.GetComponent<Button>().Select();
                currentButton = Resume.GetComponent<Button>();
                currentButton.Select();
                PlayHiClick();
                restartGame();
            }
            else if (currentButton.GetComponent<Button>() == RestartNo.GetComponent<Button>())
            {
                currentButton = Restart.GetComponent<Button>();
                PlayLowClick();
            }
            else if (currentButton.GetComponent<Button>() == QuitYes.GetComponent<Button>())
            {
                Quit.GetComponent<Button>().Select();
                currentButton = Resume.GetComponent<Button>();
                currentButton.Select();
                PlayHiClick();
                endGame();
            }
            else if (currentButton.GetComponent<Button>() == QuitNo.GetComponent<Button>())
            {
                currentButton = Quit.GetComponent<Button>();
                PlayLowClick();
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

        private void PlayLowClick()
        {
            ad.clip = lowClick;
            ad.Play();
        }
        private void PlayClick()
        {
            ad.clip = click;
            ad.Play();
        }
        private void PlayHiClick()
        {
            ad.clip = hiClick;
            ad.Play();
        }
    }
}