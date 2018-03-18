using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonQuit : ButtonPause
{
    private Button yes;
    private Button no;
    private bool selectedQuit;
    private Color subbuttonUnselected = new Color(0, 0, 0, .3f);
    private Color subbuttonSelected = new Color(.8f, .8f, .8f, 1);

    private void Awake()
    {
        // Set the color.
        originalColor = transform.GetChild(1).GetComponent<Text>().color;

        // Set the button transform.
        button = transform;

        // Set the button's text.
        buttonText = transform.GetChild(1).GetComponent<Text>();

        // Set the buttons yes and no.
        yes = transform.GetComponentsInChildren<Button>()[1];
        no = transform.GetComponentsInChildren<Button>()[2];
    }

    // OnClick button for quit.
    public void quit()
    {
        // Select the yes button to begin.
        selectedQuit = true;
        yes.GetComponent<Button>().Select();

        // Make the subbotton colors.
        Debug.Log(yes.transform.GetComponentInChildren<Text>().color);
        Debug.Log(no.transform.GetComponentInChildren<Text>().color);
        yes.transform.GetComponentInChildren<Text>().color = subbuttonSelected;
        no.transform.GetComponentInChildren<Text>().color = subbuttonUnselected;
        Debug.Log(yes.transform.GetComponentInChildren<Text>().color);
        Debug.Log(no.transform.GetComponentInChildren<Text>().color);
    }

    // Occurs when the button is deselected.
    public override void OnDeselect(BaseEventData data)
    {
        if (!selectedQuit)
        {
            base.OnDeselect(data);
        }
    }

    // Occurs when the button is deselected.
    public override void OnSelect(BaseEventData data)
    {
        // If coming from selectedQuit, we don't need to do anything to the button. Just make the subbuttons invisible.
        if (selectedQuit)
        {
            selectedQuit = false;
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
