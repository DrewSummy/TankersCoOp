using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseRoom : MonoBehaviour {
    
    public Transform[] doorIndicators;
    public Transform background;


    public void SetDoors(bool[] d)
    {
        // Set the door indicators using FindCurrentNEWS.
        int doorCount = doorIndicators.Length;

        for (int i = 0; i < doorCount; ++i)
        {
            doorIndicators[i].gameObject.SetActive(d[i]);
        }
    }

    public void SetSize(float length)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            //t.GetComponent<RectTransform>().rect.width = length;
            t.GetComponent<RectTransform>().sizeDelta = new Vector2(length, length);
        }
    }
}
