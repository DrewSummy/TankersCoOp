using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rise : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100f, 0));
	}
	
	// Update is called once per frame
	void Update ()
    {
        //gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, -10f, 0));
    }
}
