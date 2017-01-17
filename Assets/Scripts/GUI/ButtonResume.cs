using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonResume : ButtonPause
{
    private void Awake()
    {
        // Set the original position and color.
        originalPos = this.transform.position;
        originalColor = this.transform.GetChild(0).GetChild(3).GetComponent<Text>().color;

        // Set the offset position.
        offsetPos = originalPos + selectOffset;

        // Set the button transform.
        button = this.transform.GetChild(0);

        // Set the button's text.
        buttonText = this.transform.GetChild(0).GetChild(3).GetComponent<Text>();
    }
}