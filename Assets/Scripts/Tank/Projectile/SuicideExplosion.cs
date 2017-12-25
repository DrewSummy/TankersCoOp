using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideExplosion : MonoBehaviour {

    private Transform sphere;
    private Vector3 epsS = new Vector3(.2f, .2f, .2f);
    private Color epsC = new Color(.01f, .0f, .0f, -.005f);
    private float epsO = .01f;

    private Vector3 startSize = new Vector3(0, 0, 0);
    private Vector3 endSize = new Vector3(20, 20, 20);
    private Color startC = Color.red;

    // Use this for initialization
    void Start ()
    {
        sphere = transform;
        sphere.transform.localScale = startSize;
        sphere.GetComponent<Renderer>().material.color = startC;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Vector3.Magnitude(sphere.transform.localScale) < Vector3.Magnitude(endSize))
        {
            sphere.transform.localScale += epsS;
            sphere.GetComponent<Renderer>().material.color += epsC;
        }
        else
        {
            sphere.GetComponent<Renderer>().material.color += new Color(0, 0, 0, -.05f);
            if (sphere.GetComponent<Renderer>().material.color.a < 0)
            {
                Destroy(gameObject);
            }
        }
	}
}
