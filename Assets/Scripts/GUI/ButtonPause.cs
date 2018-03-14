using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

abstract public class ButtonPause : EventTrigger
{
    public GameObject image;                                      // Image for when the button isn't selected.

    protected Transform button;                                   // The transform of the button.
    public Text buttonText;                                    // The text of the button.
    protected Vector3 selectOffset = new Vector3(50, 0, 0);         // The offset of the button when selected.
    protected Vector3 originalPos;                                // The original position of the button.
    protected Vector3 offsetPos;                                  // The position of the button when selected.
    protected Vector3 originalSize;
    protected float enlargeSize = 1.1f;
    protected Color originalColor;                                // The original color of the text.
    private Color colorRange = new Color(.2f, .2f, .2f, 0);       // The range the color changes when selected..

    private bool activateText = false;


    // Occurs when the button is selected.
    public override void OnSelect(BaseEventData eventData)
    {
        // Move and activate the button.
        //moveButton();
        enlargeButton();
        StartCoroutine(activeButton());
    }

    // Occurs when the button is deselected.
    public override void OnDeselect(BaseEventData data)
    {
        // Move the button back to its original position and color.
        buttonText.color = originalColor;
        //moveButtonBack();
        unenlargeButton();

        // Deactivate the button.
        activateText = false;
    }

    // Helper function for OnSelect().
    private IEnumerator activeButton()
    {
        activateText = true;
        Color colorInc = new Color(.02f, .02f, .02f, 0);
        Color minColor = originalColor - colorRange / 2;
        Color maxColor = originalColor + colorRange / 2;

        // Flash the text.
        //buttonText.color = maxColor;
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(.05f));

        // Make the text's color go between minColor and maxColor.
        while (activateText)
        {
            // Change the color and wait.
            buttonText.color += colorInc;
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(.03f));

            // If the color exceeds the range, reset the color at the bottom of the range.
            if (buttonText.color.r > maxColor.r)
            {
                buttonText.color = minColor;
            }
        }
    }

    // Helper function for OnSelect().
    protected virtual void moveButton()
    {
        // Move the button to the offset position.
        button.position = offsetPos;
    }

    // Helper function for OnDeselect().
    protected virtual void moveButtonBack()
    {
        // Move the button back to the original position.
        button.position = originalPos;
    }

    // Helper function for OnSelect().
    protected virtual void enlargeButton()
    {
        button.localScale = button.localScale * enlargeSize;
    }

    // Helper function for OnDeselect().
    protected virtual void unenlargeButton()
    {
        button.localScale = button.localScale / enlargeSize;
    }

    // Class for coroutines when timescale = 0.
    public static class CoroutineUtilities
    {
        public static IEnumerator WaitForRealTime(float delay)
        {
            while (true)
            {
                float pauseEndTime = Time.realtimeSinceStartup + delay;
                while (Time.realtimeSinceStartup < pauseEndTime)
                {
                    yield return 0;
                }
                break;
            }
        }
    }
}