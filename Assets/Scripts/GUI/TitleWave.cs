using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleWave : MonoBehaviour {

    public Transform colorText;
    public Transform shadowText;

    private Coroutine cr;
    private Vector3 shadowDisplacement = new Vector3(5, -5, 0);
    private float startDelay = 3;
    private float inbetweenDelay = 5;
    private float period = .2f;
    private float delay = .1f;
    private float epsilon = .01f;
    private Vector3 colorOffset = new Vector3(0, 12.5f, 0);
    private Vector3 shadowOffset = new Vector3(7.5f, -7.5f, 0);
    private Color originalColor = new Color(.67f, .67f, .67f, 230);
    private Color highColor = new Color(.9f, .9f, .9f, 230);
    

    private void OnEnable()
    {
        // Reset colors.
        for (int letter = 0; letter < colorText.childCount; letter++)
        {
            colorText.GetChild(letter).GetComponentInChildren<Text>().color = originalColor;
        }

        cr = StartCoroutine(waveLetters());
    }
    private void OnDisable()
    {
        // Reset letter positions.
        for (int letter = 0; letter < colorText.childCount; letter++)
        {
            colorText.GetChild(letter).localPosition = Vector3.zero;
            shadowText.GetChild(letter).localPosition = Vector3.zero;
        }

        StopCoroutine(cr);
    }

    private IEnumerator waveLetters()
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(startDelay));

        while (true)
        {
            for (int letter = 0; letter < colorText.childCount; letter++)
            {
                StartCoroutine(moveLetters(colorText.GetChild(letter), shadowText.GetChild(letter)));
                yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(delay));
            }

            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(inbetweenDelay));
        }
    }

    private IEnumerator moveLetters(Transform c, Transform s)
    {
        float time = 0;
        while (c.localPosition.y >= 0)
        {
            c.localPosition = colorOffset * Mathf.Sin(time / period);
            s.localPosition = shadowOffset * Mathf.Sin(time / period);

            float colorRange = highColor.r - originalColor.r;

            Color newColor = new Color(originalColor.r + colorRange * Mathf.Sin(time / period),
                originalColor.g + colorRange * Mathf.Sin(time / period),
                originalColor.b + colorRange * Mathf.Sin(time / period),
                originalColor.a);
            c.GetComponentInChildren<Text>().color = newColor;

            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(epsilon));
            time += epsilon;
        }

        c.localPosition = Vector3.zero;
        s.localPosition = Vector3.zero;
        c.GetComponentInChildren<Text>().color = originalColor;
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
