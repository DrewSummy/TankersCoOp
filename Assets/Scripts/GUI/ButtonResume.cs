using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonResume : ButtonPause
{
    private void Awake()
    {
        // Set the original position and color.
        originalPos = transform.position;
        originalColor = transform.GetChild(1).GetComponent<Text>().color;

        // Set the offset position.
        offsetPos = originalPos + selectOffset;

        // Set the button transform.
        button = transform;//.GetChild(0);

        // Set the button's text.
        buttonText = transform.GetChild(1).GetComponent<Text>();
    }
}