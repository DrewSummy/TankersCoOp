using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

namespace Completed
{
    public class GUI_HUD : MonoBehaviour
    {
        //TODO: figure out aspect ratio stuff http://answers.unity3d.com/questions/1065133/black-bars-on-top-bottom-in-aspect-ratio.html
        //TODO: images make color darker

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
        private RectTransform projectileHolder;         // Transform for holding projectiles and projectileEmptys.
        private Transform enemyHolder;              // Transform for holding tanks.
        private Transform countdownHolder;          // Transform for holding countDownSprites.
        private Transform bannerHolder;
        private GameObject P1;                      // Reference to the player 1 game object.

        //TODO: choose colors based on player tank color



        private void callAwake()
        {
            P1 = GameObject.FindGameObjectWithTag("Player");
            // TODO: this might be being done too soon in awake player1Alive = P1.GetComponent<TankPlayer>().alive;
            // TODO: this might need to actually grab coop from gameMaster. player2Alive = P2.GetComponent<TankPlayer>().alive;
        }

        // Use this for initialization
        private void callStart()
        {
            P1 = GameObject.FindGameObjectWithTag("Player");

            // Create the projectile holder.
            projectileHolder = new GameObject("ProjectileHolder").AddComponent<RectTransform>();
            projectileHolder.SetParent(projectilePanel.transform);
            projectileHolder.position = projectilePanel.transform.position + new Vector3(60, 80, 0);

            // Create the enemy holder.
            enemyHolder = new GameObject("EnemyHolder").AddComponent<RectTransform>();
            enemyHolder.SetParent(enemyPanel.transform);
            enemyHolder.position = enemyPanel.transform.position + new Vector3(50, -40, 0);

            // Create the countdown holder.
            countdownHolder = new GameObject("CountdownHolder").transform;
            countdownHolder.SetParent(this.transform);
            countdownHolder.position = this.transform.position;

            // Create the banner holder.
            bannerHolder = new GameObject("BannerHolder").transform;
            bannerHolder.SetParent(this.transform);
            bannerHolder.position = this.transform.position;

            // Load relevant sprites.
            projectile = Resources.Load("HUDImages/Image_Projectile") as GameObject;
            projectileEmpty = Resources.Load("HUDImages/Image_ProjectileEmpty") as GameObject;
            tank = Resources.Load("HUDImages/Image_TankBig") as GameObject;
            blackBanner = Resources.Load("HUDImages/Image_Banner") as GameObject;

            GameObject[] sprites = new GameObject[4];
            sprites[0] = Resources.Load("HUDImages/Image_3") as GameObject;
            sprites[1] = Resources.Load("HUDImages/Image_2") as GameObject;
            sprites[2] = Resources.Load("HUDImages/Image_1") as GameObject;
            sprites[3] = Resources.Load("HUDImages/Image_Start") as GameObject;
            countDownSprites = sprites;

            // Load audio objects.
            HUDAudio = GetComponent<AudioSource>();
            countdown = Resources.Load("150207__killkhan__countdown-1") as AudioClip;
        }

        // Called when game starts.
        public void enableHUD()
        {
            callAwake();
            callStart();

            PlaceP1Projectiles();
            //PlaceP2Projectiles();
        }

        private void PlaceP1Projectiles()
        {
            // Update player 1's projectiles.
            P1ProjectileMax = P1.GetComponent<TankPlayer>().getProjectileAmount();
            P1ProjectileCount = P1.GetComponent<TankPlayer>().getProjectileCount();

            //TODO: the same for player 2

            int nonEmptyProj = P1ProjectileCount;// P1ProjectileMax - P1ProjectileCount;

            for (int bullet = 0; bullet < P1ProjectileMax; bullet++)
            {
                if (bullet < nonEmptyProj)
                {
                    GameObject projectileImage = Instantiate(projectile) as GameObject;
                    projectileImage.transform.SetParent(projectileHolder);
                    projectileImage.transform.position = projectileHolder.position + new Vector3(bullet * 25, 0, 0);
                    // The first child is the shadow.
                    Color c = Color.white;
                    projectileImage.GetComponentsInChildren<Image>()[1].color = c;
                }
                else
                {
                    GameObject projectileImage = Instantiate(projectileEmpty) as GameObject;
                    projectileImage.transform.SetParent(projectileHolder);
                    projectileImage.transform.position = projectileHolder.position + new Vector3(bullet * 25, 0, 0);
                    // The first child is the shadow. Make the alpha value of the color 110.
                    Color c = new Color(Color.white.r, Color.white.g, Color.white.b, 110f / 255f);
                    projectileImage.GetComponentsInChildren<Image>()[1].color = c;
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
                    // The first child is the shadow.
                    tankImage.GetComponentsInChildren<Image>()[1].color = enemy.GetComponentInChildren<TankEnemy>().tankColor.color;
                    currentChild++;
                }
            }
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
    }
}