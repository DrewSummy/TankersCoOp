using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;      // Tells Random to use the Unity Engine random number generator.
using Completed;


public class BackgroundCubeNormal : BackgroundCube
{

    // Use this for initialization
    void Start()
    {
        startPos = transform.localPosition;
        vOffset = Random.Range(0, 2 * Mathf.PI);
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
        }
    }
}
