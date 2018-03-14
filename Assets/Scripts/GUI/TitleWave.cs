using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleWave : MonoBehaviour {

    private Text title;
    private Text titleShadow;

    // Use this for initialization
    void Start ()
    {
        title = GetComponentsInChildren<Text>()[0];
        titleShadow = GetComponentsInChildren<Text>()[1];
        Debug.Log(title);
	}
	
	// Update is called once per frame
	void Update ()
    {
        // while true
		    // wait 1 sec
            // iterate through each letter
                // raise color and lower shadow
	}

    private IEnumerator waveLetters()
    {
        for (int letter = 0; letter < title.text.Length; letter++)
        {
            Debug.Log(title.text[letter]);
            yield return new WaitForSeconds(.5f);
        }
        Debug.Log("done");
    }
}
