using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrownShine : MonoBehaviour {


    public Image lead;
    public Image follow;
    public Image reverseLead;
    public Image reverseFollow;

    private Coroutine cr;
    private float startDelay = 3;
    private float delay = .05f;
    private float epsilon = .01f;
    private float fillRation = 6;

    private void OnEnable()
    {
        // Reset sliders.
        ResetFill();

        cr = StartCoroutine(Shine());
    }
    private void OnDisable()
    {
        StopCoroutine(cr);
    }
    private void ResetFill()
    {
        lead.fillAmount = 0;
        follow.fillAmount = 0;
        reverseLead.fillAmount = 1;
        reverseFollow.fillAmount = 1;
    }

    private IEnumerator Shine()
    {
        while (true)
        {
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(startDelay));

            StartCoroutine(Slide(lead, reverseLead));
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(delay));
            yield return StartCoroutine(Slide(follow, reverseFollow));

            ResetFill();
        }
    }

    private IEnumerator Slide(Image i, Image iR)
    {
        while (i.fillAmount < 1)
        {
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(epsilon));
            i.fillAmount += epsilon * fillRation;
            iR.fillAmount = 1 - i.fillAmount;
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
