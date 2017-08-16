using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Completed;

namespace Completed
{
    public class GUI_Menu : MonoBehaviour
    {
        public bool active = false;
        public GameMaster GM;
        public Transform panel;                                    //
        public Transform banner;
        public Transform panelLogo;                                //
        public Image logo;                                         //

        // Temp buttons
        public UnityEngine.UI.Selectable currentButton;
        public Button solo;
        public Button coop;
        public Button settings;
        public Button soloPlay;
        public Button soloBack;
        public Button coopPlay;
        public Button coopBack;
        public Button setting1;
        public Button settingsBack;

        // Physical buttons in Menu
        public GameObject menu;
        public GameObject[] blocks = new GameObject[9];
        private GameObject blockSolo;
        private GameObject blockCoop;
        private GameObject blockSettings;
        private GameObject blockSoloPlay;
        private GameObject blockSoloBack;
        private GameObject blockCoopPlay;
        private GameObject blockCoopBack;
        private GameObject blockSetting1;
        private GameObject blockSettingsBack;

        private RectTransform mainHolder;                          // Holds the buttons solo, coop, and settings.
        private Transform soloHolder;                              // Holds the buttons of solo.
        private Transform coopHolder;                              // Holds the buttons of coop.
        private Transform settingsHolder;                          // Holds the buttons of settings.

        private float[] delayRange = new float[2];

        //TODO: when one is called stop the other coroutine rather than wait until the other is done
        private Coroutine raise;
        private Coroutine lower;


        // Use this for initialization
        public void initialDisplay()
        {
            // Display the logo and then the menu in this coroutine.
            StartCoroutine(initialDisplayHelper());

            active = true;
        }

        // Helper so that the menu doesn't begin until the logo finishes.
        private IEnumerator initialDisplayHelper()
        {
            yield return displayLogo();
            displayMenu();
            yield return fadeFromBlack();
        }

        // Displays the logo.
        private IEnumerator displayLogo()
        {
            // Hold black.
            panelLogo.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            //TODO: yield return playLogoAudio();

            // Fade from black.
            yield return fadeLogoFromBlack();

            // Hold the logo and play audio.
            yield return new WaitForSeconds(1f);
            //TODO: audio, maybe the logo pronounced wrong

            // Fade to black.
            yield return fadeLogoToBlack();

            // Hold black background then set inactive.
            yield return new WaitForSeconds(1f);
            panelLogo.gameObject.SetActive(false);
        }

        // Fade the logo from black.
        private IEnumerator fadeLogoFromBlack()
        {
            // Increment alpha until visible.
            float alpha = 0;
            float increment = .03f;
            Image imageAlpha = logo;
            while (alpha < 1)
            {
                alpha += increment;
                imageAlpha.color = new Color(1, 1, 1, alpha);
                yield return new WaitForSeconds(.01f);
            }
            alpha = 1;
            imageAlpha.color = new Color(1, 1, 1, alpha);
        }

        // Fade the logo to black.
        private IEnumerator fadeLogoToBlack()
        {
            // Decrement the alpha until invisible.
            float alpha = 1;
            float increment = .08f;
            Image imageAlpha = logo;
            while (alpha > 0)
            {
                alpha -= increment;
                imageAlpha.color = new Color(1, 1, 1, alpha);
                yield return new WaitForSeconds(.01f);
            }
        }

        // Fade panel from black.
        private IEnumerator fadeToBlack()
        {
            // Increment alpha until visible.
            float alpha = 0;
            float increment = .03f;
            Image imageAlpha = panel.GetComponent<Image>();
            imageAlpha.color = Color.black;
            while (alpha < 1)
            {
                alpha += increment;
                imageAlpha.color = new Color(0, 0, 0, alpha);
                yield return new WaitForSeconds(.01f);
            }
            alpha = 1;
            imageAlpha.color = new Color(0, 0, 0, alpha);
        }

        // Fade panel to black.
        private IEnumerator fadeFromBlack()
        {
            // Decrement the alpha until invisible.
            float alpha = 1;
            float increment = .08f;
            Image imageAlpha = panel.GetComponent<Image>();
            while (alpha > 0)
            {
                alpha -= increment;
                imageAlpha.color = new Color(0, 0, 0, alpha);
                yield return new WaitForSeconds(.01f);
            }
            alpha = 0;
            imageAlpha.color = new Color(0, 0, 0, alpha);
        }


        // Displays the menu.
        private void displayMenu()
        {
            // Set the panel active and start as black.
            panel.GetComponent<Image>().color = Color.black;
            panel.gameObject.SetActive(true);

            // Initialize holders and et cetra.
            initializeHoldersEtc();

            // Select Solo and begin display.
            solo.Select();
            menu.GetComponent<Menu>().beginDisplay();

            //TOOD: fade from black

        }

        // Initialize holders and et cetra before anything else.
        private void initializeHoldersEtc()
        {
            // Main Holder
            mainHolder = new GameObject("Main Holder").AddComponent<RectTransform>();
            mainHolder.SetParent(panel);
            mainHolder.anchorMin = new Vector2(0, 0.5f);
            mainHolder.anchorMax = new Vector2(0, 0.5f);
            mainHolder.position = Vector3.zero;
            solo.transform.SetParent(mainHolder);
            coop.transform.SetParent(mainHolder);
            settings.transform.SetParent(mainHolder);

            // Blocks from menu. Set the MenuButtons "block" as its respective UIBlock.
            blocks = menu.GetComponent<Menu>().initializeAndSendBlocks();
            blockSolo = blocks[0];
            solo.GetComponent<MenuButtons>().block = blockSolo;
            blockCoop = blocks[1];
            coop.GetComponent<MenuButtons>().block = blockCoop;
            blockSettings = blocks[2];
            settings.GetComponent<MenuButtons>().block = blockSettings;
            blockSoloPlay = blocks[3];
            soloPlay.GetComponent<MenuButtons>().block = blockSoloPlay;
            blockSoloBack = blocks[4];
            soloBack.GetComponent<MenuButtons>().block = blockSoloBack;
            blockCoopPlay = blocks[5];
            coopPlay.GetComponent<MenuButtons>().block = blockCoopPlay;
            blockCoopBack = blocks[6];
            coopBack.GetComponent<MenuButtons>().block = blockCoopBack;
            blockSetting1 = blocks[7];
            setting1.GetComponent<MenuButtons>().block = blockSetting1;
            blockSettingsBack = blocks[8];
            settingsBack.GetComponent<MenuButtons>().block = blockSettingsBack;

            // Etc
            delayRange[0] = 0f;
            delayRange[1] = .5f;
        }


        // OnClick for menu buttons.
        //TODO: for settings1 and coopPlay, just run lower door
        public void soloOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected Solo");
                soloPlay.Select();
                menu.GetComponent<Menu>().placeSolo();
                currentButton = soloPlay;
            }
        }
        public void coopOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected Coop");
                coopPlay.Select();
                menu.GetComponent<Menu>().placeCoop();
                currentButton = coopPlay;
            }
        }
        public void settingsOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected Settings");
                setting1.Select();
                menu.GetComponent<Menu>().placeSettings();
                currentButton = setting1;
            }
        }
        public void soloPlayOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected SoloPlay");
                startGame();
                //clearMenu();
            }
        }
        public void soloBackOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected SoloPlay");
                solo.Select();
                menu.GetComponent<Menu>().placeMenu();
                currentButton = solo;
            }
        }
        public void coopPlayOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected CoopPlay");
                startGameCoop();
                //clearMenu();
            }
        }
        public void coopBackOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected CoopBack");
                coop.Select();
                menu.GetComponent<Menu>().placeMenu();
                currentButton = solo;
            }
        }
        public void settings1OnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                //TODO: drop a slider or a tv or something to choose the color of the tank
                Debug.Log("selected Settings1");
                StartCoroutine(temporarySettings1AndCoopPlay());//TODO: should display the setting
            }
        }
        public void settingsBackOnClick()
        {
            if (!menu.GetComponent<Menu>().containerMoving && active)
            {
                menu.GetComponent<Menu>().containerMoving = true;
                Debug.Log("selected SettingBack");
                settings.Select();
                menu.GetComponent<Menu>().placeMenu();
                currentButton = solo;
            }
        }

        // Functions called by GUI_Controller
        public void back()
        {
            if (currentButton == solo || currentButton == coop || currentButton == settings)
            {
                // Do nothing
            }
            else
            {
                if (!menu.GetComponent<Menu>().containerMoving)
                {
                    menu.GetComponent<Menu>().containerMoving = true;
                    Debug.Log("back");
                    solo.Select();
                    menu.GetComponent<Menu>().placeMenu();
                    currentButton = solo;
                }
            }
        }
        public void select()
        {
            currentButton.GetComponent<Button>().onClick.Invoke();
        }
        public void up()
        {
            Debug.Log("this shouldn't be being called");
            currentButton = currentButton.GetComponent<Button>().navigation.selectOnUp;
            currentButton.Select();
        }
        public void down()
        {
            currentButton = currentButton.GetComponent<Button>().navigation.selectOnDown;
            currentButton.Select();
        }

        private IEnumerator temporarySettings1AndCoopPlay()
        {
            yield return menu.GetComponent<Menu>().lowerContainer();
            yield return menu.GetComponent<Menu>().idleContainer();
        }

        // Called by when this camera isn't necessary.
        private void startGame()
        {
            StartCoroutine(startGameCoroutine());
            
            active = false;
        }
        private void startGameCoop()
        {
            StartCoroutine(startGameCoopCoroutine());
            
            active = false;
        }

        // Helper for disableCamera.
        private IEnumerator startGameCoroutine()
        {
            // Fade to black.
            yield return fadeToBlack();

            // Call the GameMaster function to start a game.
            GM.CreateSoloGame();

            // Set menus inactive.
            menu.SetActive(false);
            panel.gameObject.SetActive(false);
        }
        private IEnumerator startGameCoopCoroutine()
        {
            // Fade to black.
            yield return fadeToBlack();

            // Call the GameMaster function to start a game.
            GM.CreateCoopGame();

            // Set menus inactive.
            menu.SetActive(false);
            panel.gameObject.SetActive(false);
        }
        
    }
}
 