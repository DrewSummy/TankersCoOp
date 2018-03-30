using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCountdown : MonoBehaviour
{
    private float pulseTime = 1;
    private float epsilon = .03f;                       // Small time increment.
    public Color c = Color.black;                       // This will get set as the level's accent color.



    public void Pulse()
    {
        StartCoroutine(PulseHelper());
        //StartCoroutine(timeTest());
    }

    private IEnumerator PulseHelper()
    {
        // set object start color to c
        float alphaIncrement = (epsilon / pulseTime);
        int countDown = 3;
        Color mc = gameObject.GetComponent<Renderer>().material.color;

        for (int count = 0; count < countDown; count++)
        {
            // Set color to default.
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", c);
            mc.a = 0;
            float time = 0;

            while (time < pulseTime)
            {
                mc.a += alphaIncrement;
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", mc);
                yield return new WaitForSeconds(epsilon);
                time += epsilon;
            }
            yield return new WaitForSeconds(epsilon);
        }
        
        mc.a = 0;
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", mc);
    }
    private IEnumerator timeTest()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("tick");
        yield return new WaitForSeconds(1);
        Debug.Log("tick");
        yield return new WaitForSeconds(1);
        Debug.Log("tick");
        yield return new WaitForSeconds(1);
        Debug.Log("tick");
    }
}
