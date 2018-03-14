using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

namespace Completed
{
    public class GUI_HUD : MonoBehaviour
    {
        public GameObject gameOverPanel;
        public GameObject Restart;
        public GameObject Menu;
        public GameObject projectile;               // The GameObject with the projectile sprite.
        public GameObject projectileEmpty;          // The GameObject with the empty projectile sprite.
        public GameObject tank;                     // The GameObject with the enemy sprite.
        public GameObject blackBanner;              // The GameObject with the banner sprite.
        public Material[] enemiesColor;             //TODO: use this for specific sprite colors instead of the tanks colors
        public GameObject[] countDownSprites;       // The GameObjects with the countdown sprites.
        public AudioSource HUDAudio;                // Reference to the audio source used to play HUD sounds.
        public AudioClip countdown;                 // Reference to the audio clip used to play the HUD audio source.
        public bool player1Alive;
        public bool player2Alive;

        public GameObject projectilePanel;
        public GameObject enemyPanel;

        private int P1ProjectileMax;                // The maximum number of player 1's projectiles at a time.
        private int P1ProjectileCount;              // The current amount of player 1's projectiles available to fire.
        private int P2ProjectileMax;                // The maximum number of player 1's projectiles at a time.
        private int P2ProjectileCount;              // The current amount of player 1's projectiles available to fire.
        public RectTransform projectileHolder1;     // Transform for holding projectiles and projectileEmptys.
        public RectTransform projectileHolder2;     // Transform for holding projectiles and projectileEmptys.
        //private Transform enemyHolder;              // Transform for holding tanks.
        public Transform enemyHolder;              // Temp
        public Transform countdownHolder;          // Transform for holding countDownSprites.
        public Transform bannerHolder;
        private GameObject P1;                      // Reference to the player 1 game object.
        private GameObject P2;                      // Reference to the player 2 game object.
        private float iconDistance = 15;

        // Coroutine
        Coroutine countDown;




        private void callAwake()
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
            // TODO: this might be being done too soon in awake player1Alive = P1.GetComponent<TankPlayer>().alive;
            // TODO: this might need to actually grab coop from gameMaster. player2Alive = P2.GetComponent<TankPlayer>().alive;
        }

        // Use this for initialization
        private void callStart()
        {
            // Create the enemy holder.
            //enemyHolder = new GameObject("EnemyHolder").AddComponent<RectTransform>();
            //enemyHolder.SetParent(enemyPanel.transform);
            //enemyHolder.position = enemyPanel.transform.position + new Vector3(50, -40, 0);

            // Create the countdown holder.
            //countdownHolder = new GameObject("CountdownHolder").transform;
            //countdownHolder.SetParent(this.transform);
            //countdownHolder.position = this.transform.position;

            // Create the banner holder.
            //bannerHolder = new GameObject("BannerHolder").transform;
            //bannerHolder.SetParent(this.transform);
            //bannerHolder.position = this.transform.position;

            // Load relevant sprites.
            /*projectile = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Image_Projectile") as GameObject;
            projectileEmpty = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Image_ProjectileEmpty") as GameObject;
            //tank = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Image_TankBig") as GameObject;
            blackBanner = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Image_Banner") as GameObject;*/

            GameObject[] sprites = new GameObject[4];
            sprites[0] = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Deprecated/Image_3") as GameObject;
            sprites[1] = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Deprecated/Image_2") as GameObject;
            sprites[2] = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Deprecated/Image_1") as GameObject;
            sprites[3] = Resources.Load("Prefab/UIPrefab/GUI/HUDImages/Deprecated/Image_Start") as GameObject;
            countDownSprites = sprites;

            // Load audio objects.
            HUDAudio = GetComponent<AudioSource>();
            countdown = Resources.Load("150207__killkhan__countdown-1") as AudioClip;
        }

        // Enable and disable the other panels.
        private void OnEnable()
        {
            projectileHolder1.gameObject.SetActive(true);
            if (P2)
            {
                projectileHolder2.gameObject.SetActive(true);
            }
        }
        private void OnDisable()
        {
            projectileHolder1.gameObject.SetActive(false);
            projectileHolder2.gameObject.SetActive(false);
        }

        // Called when game starts.
        public void enableHUD()
        {
            callAwake();
            callStart();

            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (tank.GetComponent<TankPlayer>().m_PlayerNumber == 1)
                {
                    PlaceP1Projectiles();
                }
                else
                {
                    PlaceP2Projectiles();
                }
            }
        }

        private void PlaceP1Projectiles()
        {
            // Update player 1's projectiles.
            P1ProjectileMax = P1.GetComponent<TankPlayer>().getProjectileAmount();
            P1ProjectileCount = P1.GetComponent<TankPlayer>().getProjectileCount();

            int nonEmptyProj = P1ProjectileCount;

            for (int bullet = 0; bullet < P1ProjectileMax; bullet++)
            {
                Transform p = projectileHolder1.GetChild(bullet);

                if (bullet < nonEmptyProj)
                {
                    p.GetChild(0).gameObject.SetActive(false);
                    p.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    p.GetChild(0).gameObject.SetActive(true);
                    p.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
        private void PlaceP2Projectiles()
        {
            // Update player 1's projectiles.
            P2ProjectileMax = P2.GetComponent<TankPlayer>().getProjectileAmount();
            P2ProjectileCount = P2.GetComponent<TankPlayer>().getProjectileCount();

            int nonEmptyProj = P2ProjectileCount;

            for (int bullet = 0; bullet < P2ProjectileMax; bullet++)
            {
                Transform p = projectileHolder2.GetChild(bullet);

                if (bullet < nonEmptyProj)
                {
                    p.GetChild(0).gameObject.SetActive(false);
                    p.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    p.GetChild(0).gameObject.SetActive(true);
                    p.GetChild(1).gameObject.SetActive(false);
                }
            }
        }

        public void UpdateProjectiles()
        {

            foreach (GameObject tank in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (tank.GetComponent<TankPlayer>().m_PlayerNumber == 1)
                {
                    UpdateP1Projectiles();
                }
                else
                {
                    UpdateP2Projectiles();
                }
            }
        }
        private void UpdateP1Projectiles()
        {
            //foreach (Transform projectile in projectileHolder1.transform)
            /*foreach (Transform projectile in projectileHolder1.transform)
            {
                GameObject.Destroy(projectile.gameObject);
            }*/

            PlaceP1Projectiles();
        }
        private void UpdateP2Projectiles()
        {
            //foreach (Transform projectile in projectileHolder2.transform)
            /*foreach (Transform projectile in projectileHolder2.transform)
            {
                GameObject.Destroy(projectile.gameObject);
            }*/

            //PlaceP2Projectiles();
            PlaceP2Projectiles();
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
                    tankImage.transform.SetSiblingIndex(0);
                    tankImage.transform.position = enemyHolder.position + new Vector3(currentChild * iconDistance, 0, 0);
                    // The first child is the shadow.
                    tankImage.GetComponentsInChildren<Image>()[1].color = enemy.GetComponentInChildren<TankEnemy>().tankColor.color;
                    currentChild++;
                }
            }
        }

        public void UpdateEnemies(Transform eH)
        {
            // Remove the objects.
            //foreach (Transform enemyImage in enemyHolder.transform)
            foreach (Transform enemyImage in enemyHolder.transform)
            {
                GameObject.Destroy(enemyImage.gameObject);
            }

            PlaceEnemies(eH);
        }

        public void PlayCountDown()
        {
            //countDown = StartCoroutine(CountdownCoroutine());

            //TODO: play 3, 2, 1, countdown audio
            HUDAudio.clip = countdown;
            HUDAudio.Play();
        }

        private IEnumerator CountdownCoroutine()
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
        
        void losingScene()
        {
            //TODO:
            // eliminate all other sprites
            // introduce options; ie restart
            // display score
            // make public and have this be called when both player die

            GameObject topBanner = Instantiate(blackBanner) as GameObject;
            topBanner.transform.SetParent(bannerHolder);
            topBanner.transform.position = bannerHolder.position + new Vector3(0, 175, 0);

            GameObject bottomBanner = Instantiate(blackBanner) as GameObject;
            bottomBanner.transform.SetParent(bannerHolder);
            bottomBanner.transform.position = bannerHolder.position + new Vector3(0, -175, 0);
        }

        private void GameOver()
        {
            gameOverPanel.SetActive(true);

            Restart.GetComponent<Button>().Select();
            Menu.GetComponent<Button>().Select();
            //TODO: create an idle state between games
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(enemyHolder.childCount);
        }

        public void resetHUD()
        {
            //StopCoroutine(countDown);
            
            /*oreach (Transform child in countdownHolder.transform)
            {
                GameObject.Destroy(child.gameObject);
            }*/


            //Destroy(countdownHolder.gameObject);
            //Destroy(enemyHolder.gameObject);
        }
    }
}