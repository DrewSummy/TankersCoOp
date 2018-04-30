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
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        yield return new WaitForSeconds(.25f);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        yield return new WaitForSeconds(.75f);

        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        yield return new WaitForSeconds(.25f);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        yield return new WaitForSeconds(.75f);

        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        StartCoroutine(turnOff());
    }
    private IEnumerator turnOff()
    {
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.clear);
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
