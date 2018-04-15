using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCamera : MonoBehaviour {

    public Transform panel;

    private void OnEnable()
    {
        panel.GetComponent<Image>().color = Color.black;
    }
    private void OnDisable()
    {
        panel.GetComponent<Image>().color = Color.black;
    }
}
