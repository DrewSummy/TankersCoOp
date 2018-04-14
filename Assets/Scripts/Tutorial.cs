using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public GameObject[] slides;
    private Coroutine cr;
    private float holdTime = 1;

	// Use this for initialization
	void Start ()
    {
        cr = StartCoroutine(rotateImages());
	}
	
	// Update is called once per frame
	private IEnumerator rotateImages()
    {
        while (true)
        {
            for (int i = 0; i < slides.Length; i++)
            {
                slides[i].SetActive(true);
                yield return new WaitForSeconds(holdTime);
                slides[i].SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(cr);
    }
}
