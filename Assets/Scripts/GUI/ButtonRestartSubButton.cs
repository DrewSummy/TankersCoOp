using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonRestartSubButton : ButtonPause
{

    private Color subbuttonUnselected = new Color(0, 0, 0, 80);
    private Color subbuttonSelected = new Color(255, 255, 255, 150);

    private void Awake()
    {
        // Set the color.
        originalColor = transform.GetComponentInChildren<Text>().color;

        // Set the button transform.
        button = transform;

        // Set the button's text.
        buttonText = transform.GetComponentInChildren<Text>();
    }


    // Occurs when the button is deselected.
    public override void OnDeselect(BaseEventData data)
    {
        // Make the button greyed.
        buttonText.color = subbuttonUnselected;
    }

    // Occurs when the button is deselected.
    public override void OnSelect(BaseEventData data)
    {
        // Make the button highleted.
        buttonText.color = subbuttonSelected;
    }
}