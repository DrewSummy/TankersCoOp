using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleWave : MonoBehaviour {

    public Transform colorText;
    public Transform shadowText;

    private float period = .25f;
    private float delay = .1f;
    private float epsilon = .03f;
    private Vector3 colorOffset = new Vector3(0, .25f, 0);
    private Vector3 shadowOffset = new Vector3(.4f, -.15f, 0);

    // Use this for initialization
    void Start ()
    {
        //Time.timeScale = 0;
	}

    private void OnEnable()
    {
        StartCoroutine(waveLetters());
    }

    private IEnumerator waveLetters()
    {
        while (true)
        {
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(3));

            for (int letter = 0; letter < colorText.childCount; letter++)
            {
                StartCoroutine(moveLetters(colorText.GetChild(letter), shadowText.GetChild(letter)));
                yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(delay));
            }
        }
    }

    private IEnumerator moveLetters(Transform c, Transform s)
    {
        Vector3 cPos = c.localPosition;
        Vector3 sPos = s.localPosition;

        float time = 0;
        while (c.localPosition.y >= 0)
        {
            c.localPosition = c.localPosition + colorOffset * Mathf.Cos(time / period);
            s.localPosition = s.localPosition + shadowOffset * Mathf.Cos(time / period);
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(epsilon));
            time += epsilon;
        }

        c.localPosition = cPos;
        s.localPosition = sPos;
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
