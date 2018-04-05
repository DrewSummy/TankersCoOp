using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFade : MonoBehaviour {

    private float liveTime = 3;
    private float time = 0;
    private float epsilon = .025f;
    private Light L;
    private float intesity;

    // Use this for initialization
    void Start ()
    {
        L = GetComponent<Light>();
        intesity = L.intensity;
	}

    // Update is called once per frame
    void Update()
    {
        intesity -= epsilon;
        L.intensity = intesity;
        if (intesity < 0)
        {
            Destroy(gameObject);
        }
    }
}
