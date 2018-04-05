using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Completed;

namespace Completed
{
    public class GUI_GameOver : GUI_Pause
    {
        private float timeSlowSpeed = .35f;
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
                //StartCoroutine(returnToNormalTimescale());

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
    }
}