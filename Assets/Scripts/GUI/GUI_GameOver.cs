using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Completed;

namespace Completed
{
    public class GUI_GameOver : GUI_Pause
    {
        private float timeSlowSpeed = .35f;
        private float fixedDeltaOriginalTime = .02f;
        private float timeScaleDelay = 2f;
        private float epsilon = .02f;
        private float panelAlpha = .34f;
        

        public override void Pause()
        {
            if (enabled)
            {
                PlaceMenu();
                panel.SetActive(true);
                StartCoroutine(panelFadeIn());
                Time.timeScale = timeSlowSpeed;
                Time.fixedDeltaTime = timeSlowSpeed;

                // Set the resume button as selected. For some reason there has to be a different button selected previously.
                Quit.GetComponent<Button>().Select();
                Restart.GetComponent<Button>().Select();

                // Display map and hide the GUI_MiniMap's map.
                guiMiniMap.Hide();

                paused = true;

                // Enable the controller.
                controllerGUI.enabled = true;
            }
        }
        public override void Unpause()
        {
            if (enabled)
            {
                currentButton = Restart.GetComponent<Button>();

                Time.timeScale = 1f;
                Time.fixedDeltaTime = fixedDeltaOriginalTime;
                RemoveKills();
                panel.SetActive(false);

                // Reveal the GUI_MiniMap's map.
                guiMiniMap.Reveal();

                // Disable the controller.
                controllerGUI.enabled = false;

                paused = false;
            }
        }

        private IEnumerator panelFadeIn()
        {
            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            float currentA = 0;

            while (panel.GetComponent<Image>().color.a < panelAlpha)
            {
                panel.GetComponent<Image>().color = new Color(0, 0, 0, currentA);
                currentA += epsilon;
                yield return new WaitForSeconds(epsilon);
            }
        }

        private IEnumerator returnToNormalTimescale()
        {
            //slowly progress to Time.timeScale = 1; over timeScaleDelay.
            float time = 0;

            while (time < timeScaleDelay)
            {
                time += epsilon;
                yield return new WaitForSeconds(epsilon);
            }
        }

        // End and restart game.
        protected override void endGame()
        {
            currentButton = Restart.GetComponent<Button>();
            Unpause();
            GM.endGame();
        }
        protected override void restartGame()
        {
            RemoveMap();
            guiMiniMap.clearMap();
            currentButton = Restart.GetComponent<Button>();
            Unpause();
            GM.restart();
        }

        // Functions called by GUI_Controller
        public override void back()
        {
            if (currentButton == RestartYes.GetComponent<Button>() ||
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
        public override void select()
        {
            if (currentButton.GetComponent<Button>() == Restart.GetComponent<Button>())
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
                currentButton = Restart.GetComponent<Button>();
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
                currentButton = Restart.GetComponent<Button>();
                currentButton.Select();
                endGame();
            }
            else if (currentButton.GetComponent<Button>() == QuitNo.GetComponent<Button>())
            {
                currentButton = Quit.GetComponent<Button>();
            }

            currentButton.Select();
        }
    }
}