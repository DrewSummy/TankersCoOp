using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonRestart : ButtonPause
{
    private Button yes;
    private Button no;
    private bool selectedRestart;
    private Color subbuttonUnselected = new Color(0, 0, 0, 150);
    private Color subbuttonSelected = Color.white;

    private void Awake()
    {
        // Set the originalPos and color.
        originalPos = transform.position;
        originalColor = transform.GetChild(1).GetComponent<Text>().color;

        // Set the offset position.
        offsetPos = originalPos + selectOffset;

        // Set the button transform.
        button = transform;

        // Set the button's text.
        buttonText = transform.GetChild(1).GetComponent<Text>();

        // Set the buttons yes and no.
        yes = transform.GetComponentsInChildren<Button>()[1];
        no = transform.GetComponentsInChildren<Button>()[2];
    }

    // OnClick button for restart.
    public void restart()
    {
        // Select the yes button to begin.
        selectedRestart = true;
        yes.GetComponent<Button>().Select();

        // Make the subbotton colors.
        yes.transform.GetComponentInChildren<Text>().color = subbuttonSelected;
        no.transform.GetComponentInChildren<Text>().color = subbuttonUnselected;
    }

    // Occurs when the button is deselected.
    public override void OnDeselect(BaseEventData data)
    {
        if (!selectedRestart)
        {
            base.OnDeselect(data);
        }
    }

    // Occurs when the button is deselected.
    public override void OnSelect(BaseEventData data)
    {
        // If coming from selectedQuit, we don't need to do anything to the button. Just make the subbuttons invisible.
        if (selectedRestart)
        {
            selectedRestart = false;
            no.transform.GetComponentInChildren<Text>().color = new Color(0, 0, 0, 0);
            yes.transform.GetComponentInChildren<Text>().color = new Color(0, 0, 0, 0);
        }
        // ...or use the base function and add animation.
        else
        {
            base.OnSelect(data);
        }
    }
}
