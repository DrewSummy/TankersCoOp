using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonRestartSubButton : ButtonPause
{

    private Color subbuttonUnselected = new Color(0, 0, 0, 150);
    private Color subbuttonSelected = Color.white;

    private void Awake()
    {
        // Set the originalPos and color.
        originalPos = transform.position;
        originalColor = transform.GetComponentInChildren<Text>().color;

        // Set the offset position.
        offsetPos = originalPos + selectOffset;

        // Set the button transform.
        button = transform;

        // Set the button's text.
        buttonText = transform.GetComponentInChildren<Text>();
    }

    protected override void moveButton()
    {
        return;
    }
    protected override void moveButtonBack()
    {
        return;
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