using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonResume : ButtonPause
{
    private void Awake()
    {
        // Set the and color.
        originalColor = transform.GetChild(1).GetComponent<Text>().color;

        // Set the button transform.
        button = transform;

        // Set the button's text.
        buttonText = transform.GetChild(1).GetComponent<Text>();
    }
}