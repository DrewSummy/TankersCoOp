using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCube : MonoBehaviour
{
    protected float time;
    protected Vector3 startPos;
    protected float amplitude = .5f;

    public float freq = Mathf.PI;
    public float vOffset = 0;
    public float rOffset = 0;
    public float speed = 150;
    public bool on = true;



    // Use this for initialization
    void Start()
    {
        startPos = transform.localPosition;
        //transform.LookAt(Quaternion.Euler(45, 45, 0) * Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            time += Time.deltaTime;
            float magnitude = -amplitude * Mathf.Sin(time * freq + vOffset);
            Vector3 pos = startPos + Vector3.up * magnitude;
            transform.localPosition = pos;
            
            transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
        }
    }
}
