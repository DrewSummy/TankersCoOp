using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private float speed = .005f;
    
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, speed);
    }
}