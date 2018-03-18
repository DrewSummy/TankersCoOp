using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.UI;
using Completed;

public class GUI_Controller : MonoBehaviour
{
    private Coroutine currentCoroutine;

    // Button/Joystick Names
    private string jsLeftVerticalName = "Left Stick Vertical";
    private string dpVerticalName = "DPAD Vertical";
    private string startName = "Start";
    private string AButtonName = "Button A";
    private string BButtonName = "Button B";

    // Button/Joystick Values
    private float jsLeftVerticalValue;
    private float dpVerticalValue;
    private bool startValue;
    private bool AButtonValue;
    private bool BButtonValue;

    // Miscellaneous
    public GUI_Menu menuGUI;                                            // GUI
    public GUI_Pause pauseGUI; 
    private float deadzone = .3f;
    private float repeatDelay = .5f;
    private bool canUp = true;
    private bool canDown = true;

    

    // Store the value of the input axes while exculding based on deadzones.
    private void TakeControllerInputs()
    {
        PlayerID PID = TeamUtility.IO.PlayerID.One;

        // Left Joystick
        jsLeftVerticalValue = -InputManager.GetAxis(jsLeftVerticalName, PID);
        if (Mathf.Abs(jsLeftVerticalValue) < deadzone)
        {
            jsLeftVerticalValue = 0;
        }

        // D-pad
        dpVerticalValue = InputManager.GetAxis(dpVerticalName, PID);
        if (Mathf.Abs(dpVerticalValue) < deadzone)
        {
            dpVerticalValue = 0;
        }

        // Start
        startValue = InputManager.GetButtonDown(startName, PID);

        // A
        AButtonValue = InputManager.GetButtonDown(AButtonName, PID);

        // B
        BButtonValue = InputManager.GetButtonDown(BButtonName, PID);
    }


    void Update()
    {
        TakeControllerInputs();

        select();
        deselect();
        vertical();
    }

    private void select()
    {
        if (startValue || AButtonValue)
        {
            if (menuGUI.active)
            {
                menuGUI.select();
            }
            if (pauseGUI.paused)
            {
                pauseGUI.select();
            }
        }
    }

    private void deselect()
    {
        if (BButtonValue)
        {
            if (menuGUI.active)
            {
                menuGUI.back();
            }
            if (pauseGUI.paused)
            {
                pauseGUI.back();
            }
        }
    }

    private void vertical()
    {
        float v = dpVerticalValue;
        if (v == 0)
        {
            v = jsLeftVerticalValue;
        }

        if (v > 0)
        {
            if (canUp)
            {
                Coroutine oldRoutine = currentCoroutine;
                if (oldRoutine != null)
                {
                    StopCoroutine(oldRoutine);
                }
                currentCoroutine = StartCoroutine(delayUp());

                if (menuGUI.active)
                {
                    menuGUI.up();
                }
                if (pauseGUI.paused)
                {
                    pauseGUI.up();
                }
            }
        }
        else if (v < 0)
        {
            if (canDown)
            {
                Coroutine oldRoutine = currentCoroutine;
                if (oldRoutine != null)
                {
                    StopCoroutine(oldRoutine);
                }
                currentCoroutine = StartCoroutine(delayDown());

                if (menuGUI.active)
                {
                    menuGUI.down();
                }
                if (pauseGUI.paused)
                {
                    pauseGUI.down();
                }
            }
        }
        else
        {
            canUp = true;
            canDown = true;
        }
    }

    private IEnumerator delayUp()
    {
        canUp = false;
        yield return new WaitForSeconds(repeatDelay);
        canUp = true;
    }
    private IEnumerator delayDown()
    {
        canDown = false;
        yield return new WaitForSeconds(repeatDelay);
        canDown = true;
    }
}
