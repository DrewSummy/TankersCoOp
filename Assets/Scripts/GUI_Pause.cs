using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_Pause : MonoBehaviour
{

    public GameObject panel;
    public GameObject tank;
    public GameObject Resume;
    public GameObject Restart;
    public GameObject Quit;
    private int[] P1Kills;
    private Transform killHolder;
    private GameObject P1;
    public GameObject killCountText;
    public Material[] tankColors;
    private bool m_PauseValue1;                                // The value of the bool for the pause.
    private string m_PauseName1 = "Pause" + 1;                 // The name of the bool for pausing.
    private bool paused = false;


    void Awake()
    {
        P1 = GameObject.FindGameObjectWithTag("Player");

    }

    // Use this for initialization
    void Start()
    {
        killHolder = new GameObject("KillHolder").transform;
        killHolder.SetParent(panel.transform);
        killHolder.position = panel.transform.position + new Vector3(10, 0, 0);

        // Load in the tank colors being used from the Resources folder in assets.
        tankColors = Resources.LoadAll<Material>("TankColors");
    }

    private void PlaceMenu()
    {
        P1 = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(P1.name);

        PlaceP1Kills();
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
                    tankImage.transform.SetParent(killHolder);
                    tankImage.transform.position = killHolder.position + new Vector3(0 + kill * 5 + offset, 20 - killType * 25, 0);
                    tankImage.GetComponent<Image>().color = tankColors[killType].color;
                    tankImage.GetComponent<RectTransform>().localScale = new Vector3(.1f, .1f, 1);
                    killCountText.GetComponent<Text>().enabled = false;
                }
            }
            // Else, just print a counter representing the number of kills like "x 26".
            else
            {
                GameObject tankImage = Instantiate(tank) as GameObject;
                tankImage.transform.SetParent(killHolder);
                tankImage.transform.position = killHolder.position + new Vector3(0, 20 - killType * 25, 0); ;
                tankImage.GetComponent<Image>().color = tankColors[killType].color;
                tankImage.GetComponent<RectTransform>().localScale = new Vector3(.1f, .1f, 1);

                GameObject tankText = Instantiate(killCountText) as GameObject;
                tankText.transform.SetParent(killHolder);
                tankText.GetComponent<Text>().text = "x  " + P1Kills[killType];
                tankText.GetComponent<Text>().fontSize = 14;
                tankText.transform.position = killHolder.position + new Vector3(129, 22 - killType * 25, 0);
                tankText.GetComponent<Text>().enabled = true;
            }
        }
    }


    private void RemoveKills()
    {
        foreach (Transform child in killHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {

        P1 = GameObject.FindGameObjectWithTag("Player");

        // Store the value for pauseing.
        m_PauseValue1 = Input.GetButtonDown(m_PauseName1);

        PauseAndUnpause();
    }

    private void PauseAndUnpause()
    {
        panel.SetActive(paused);

        if (!paused)
        {
            if (m_PauseValue1)
            {
                Time.timeScale = 0f;
                PlaceMenu();
                Resume.GetComponent<Button>().Select();
                Restart.GetComponent<Button>().Select();
                Resume.GetComponent<Button>().Select();
                paused = true;
            }
        }
        else
        {
            if (m_PauseValue1)
            {
                Time.timeScale = 1f;
                paused = false;
                RemoveKills();
            }
        }
    }
}
