using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;


public class GUI_HUD : MonoBehaviour
{

    public GameObject projectile;
    public GameObject projectileEmpty;
    public GameObject tank;
    public Material[] enemiesColor;        //TODO: use this for specific sprite colors instead of the tanks colors
    public GameObject[] countDownSprites;
    public AudioSource HUDAudio;
    public AudioClip countdown;

    private int P1ProjectileCount;
    private int P1ProjectileMax;
    private Transform projectileHolder;
    private Transform enemyHolder;
    private Transform countdownHolder;
    private GameObject P1;
    
    //TODO: place light behind HUD



    void Awake()
    {
        P1 = GameObject.FindGameObjectWithTag("Player");
    }

    // Use this for initialization
    void Start()
    {
        P1 = GameObject.FindGameObjectWithTag("Player");

        // Create the projectile holder.
        projectileHolder = new GameObject("ProjectileHolder").transform;
        projectileHolder.SetParent(this.transform);
        projectileHolder.position = this.transform.position + new Vector3(-420, -150, 0);

        // Create the enemy holder.
        enemyHolder = new GameObject("EnemyHolder").transform;
        enemyHolder.SetParent(this.transform);
        enemyHolder.position = this.transform.position + new Vector3(-400, 235, 0);

        // Create the countdown holder.
        countdownHolder = new GameObject("CountdownHolder").transform;
        countdownHolder.SetParent(this.transform);
        countdownHolder.position = this.transform.position + new Vector3(0, 0, 0);// this.transform.position + new Vector3(100, 100, 0);

        // Load relevant sprites.
        projectile = Resources.Load("Image_Projectile") as GameObject;
        projectileEmpty = Resources.Load("Image_ProjectileEmpty") as GameObject;
        tank = Resources.Load("Image_TankBig") as GameObject;

        GameObject[] sprites = new GameObject[4];
        sprites[0] = Resources.Load("Image_3") as GameObject;
        sprites[1] = Resources.Load("Image_2") as GameObject;
        sprites[2] = Resources.Load("Image_1") as GameObject;
        sprites[3] = Resources.Load("Image_Start") as GameObject;
        countDownSprites = sprites;

        // Load audio objects.
        HUDAudio = GetComponent<AudioSource>();
        countdown = Resources.Load("150207__killkhan__countdown-1") as AudioClip;


        PlaceP1Projectiles();
        //PlaceP2Projectiles();
        }
    

    private void PlaceP1Projectiles()
    {
        // Update player 1's projectiles.
        P1ProjectileMax = P1.GetComponent<Tank>().projectileAmount;
        P1ProjectileCount = P1.GetComponent<Tank>().projectileCount;
        
        //TODO: the same for player 2

        for (int bullet = 0; bullet < P1ProjectileMax; bullet++)
        {
            if (bullet < P1ProjectileCount)
            {
                GameObject projectileImage = Instantiate(projectile) as GameObject;
                projectileImage.transform.SetParent(projectileHolder);
                projectileImage.transform.position = projectileHolder.position + new Vector3(bullet * 25, 0, 0);
            }
            else
            {
                GameObject projectileImage = Instantiate(projectileEmpty) as GameObject;
                projectileImage.transform.SetParent(projectileHolder);
                projectileImage.transform.position = projectileHolder.position + new Vector3(bullet * 25, 0, 0);
            }
        }
    }

    public void UpdateP1Projectiles()
    {
        foreach (Transform projectile in projectileHolder.transform)
        {
            GameObject.Destroy(projectile.gameObject);
        }

        PlaceP1Projectiles();
    }

    public void PlaceEnemies(Transform eH)
    {
        // Place sprites for enemyGameObjectHolder.
        int currentChild = 0;
        foreach (Transform enemy in eH.transform)
        {
            if (enemy.gameObject.activeSelf)
            {
                GameObject tankImage = Instantiate(tank) as GameObject;
                tankImage.transform.SetParent(enemyHolder);
                tankImage.transform.position = enemyHolder.position + new Vector3(currentChild * 90, 0, 0);
                tankImage.GetComponent<Image>().color = enemy.GetComponentInChildren<TankEnemy>().tankColor.color;
                currentChild++;
            }
        }
        /*
        for (int enemy = 0; enemy < eH.childCount; enemy++)
        {
            GameObject currentEnemy = eH.GetComponentsInChildren<Transform>()[enemy].gameObject;
            if (currentEnemy.activeSelf)
            {
                GameObject tankImage = Instantiate(tank) as GameObject;
                tankImage.transform.SetParent(enemyHolder);
                tankImage.transform.position = enemyHolder.position + new Vector3(enemy * 90, 0, 0);
                //tankImage.GetComponent<Image>().color = currentEnemy.GetComponentInChildren<TankEnemy>().tankColor.color;
            }
        }*/
    }

    public void UpdateEnemies(Transform eH)
    {
        // Remove the objects.
        foreach (Transform enemyImage in enemyHolder.transform)
        {
            GameObject.Destroy(enemyImage.gameObject);
        }
        
        PlaceEnemies(eH);
    }

    public void PlayCountDown()
    {
        StartCoroutine(CountdownCoroutine());

        //TODO: play 3, 2, 1, countdown audio
        HUDAudio.clip = countdown;
        HUDAudio.Play();
    }

    IEnumerator CountdownCoroutine()
    {
        for (int image = 0; image < countDownSprites.Length; image++)
        {
            GameObject currentImage = Instantiate(countDownSprites[image]) as GameObject;
            currentImage.transform.SetParent(countdownHolder);
            currentImage.transform.position = countdownHolder.position;
            //TODO: figure out color stuff

            yield return new WaitForSeconds(1f);

            GameObject.Destroy(currentImage);
        }


    }
        // Update is called once per frame
        void Update()
    {
        //Debug.Log(enemyHolder.childCount);
    }
}
