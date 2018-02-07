using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;

public class BackgroundMaster : MonoBehaviour {

    public CameraControl cam;

    // Use this for initialization
    void Start()
    {
        cam.gameObject.SetActive(true);
        cam.Initialize(transform);
    }
}
