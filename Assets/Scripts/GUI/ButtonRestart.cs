using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonRestart : ButtonPause
{
    private Button yes;
    private Button no;
    private bool selectedRestart;
    private Color subbuttonUnselected = Color.grey;

    private void Awake()
    {
        // Set the originalPos and color.
        originalPos = transform.position;
        originalColor = transform.GetChild(0).GetChild(3).GetComponent<Text>().color;

        // Set the offset position.
        offsetPos = originalPos + selectOffset;

        // Set the button transform.
        button = transform;

        // Set the button's text.
        buttonText = transform.GetChild(0).GetChild(3).GetComponent<Text>();

        // Set the buttons yes and no.
        no = transform.GetChild(0).GetChild(4).GetComponent<Button>();
        yes = transform.GetChild(0).GetChild(5).GetComponent<Button>();
    }

    // OnClick button for quit.
    public void restart()
    {
        // Select the yes button to begin.
        selectedRestart = true;
        yes.GetComponent<Button>().Select();

        // Make the no button greyed.
        no.transform.GetChild(0).GetComponent<Text>().color = subbuttonUnselected;
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
            no.transform.GetChild(0).GetComponent<Text>().color = new Color(0, 0, 0, 0);
            yes.transform.GetChild(0).GetComponent<Text>().color = new Color(0, 0, 0, 0);
        }
        // ...or use the base function and add animation.
        else
        {
            base.OnSelect(data);
        }
    }
    
    //TODO: OBSOLETE: these buttons: probably call a funtion in GameMaster
    /*public void restartYes()
    {
        Debug.Log("another game");
    }
    public void restartNo()
    {
        Debug.Log("same game");
        GetComponent<Button>().Select();
    }
    public void cancelSubButton()
    {
        GetComponent<Button>().Select();
    }*/
}
