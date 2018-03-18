using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseRoomOccupied : PauseRoom {

    public Color accent;
    private Color defaultColor = new Color(150f / 255f, 150f / 255f, 150f / 255f, 150f / 255f);
    private Color difference;

    private float delay = .5f;
    private float epsilon = .01f;
    private float period = .15f;
    private Coroutine cr;


    private void OnEnable()
    {
        cr = StartCoroutine(Shine());
    }
    private void OnDisable()
    {
        StopCoroutine(cr);
    }

    private void Start()
    {
        difference = (accent - defaultColor);
    }

    private IEnumerator Shine()
    {
        while (true)
        {
            background.GetComponent<Image>().color = defaultColor;
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(delay));

            for (int i = 0; i < 2; i++)
            {
                float time = 0;

                background.GetComponent<Image>().color = accent;
                while (time < period)
                {

                    Color newColor = new Color(defaultColor.r + difference.r * Mathf.Sin(time / period),
                        defaultColor.g + difference.g * Mathf.Sin(time / period),
                        defaultColor.b + difference.b * Mathf.Sin(time / period),
                        defaultColor.a);

                    newColor = defaultColor + difference * Mathf.Sin(time / period);
                    background.GetComponent<Image>().color = newColor;



                    yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(epsilon));
                    time += epsilon;
                }
            }
        }
    }

    // Class for coroutines when timescale = 0.
    public static class CoroutineUtilities
    {
        public static IEnumerator WaitForRealTime(float delay)
        {
            while (true)
            {
                float pauseEndTime = Time.realtimeSinceStartup + delay;
                while (Time.realtimeSinceStartup < pauseEndTime)
                {
                    yield return 0;
                }
                break;
            }
        }
    }
}
