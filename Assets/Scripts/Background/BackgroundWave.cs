using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundWave : MonoBehaviour
{
    private bool debug = true;                     // No animations that slow the game down.

    public Color colorMain = Color.blue;
    public Color colorAccent = Color.white;
    public GameObject cube;
    public GameObject cubeActive;
    public GameObject plane;
    public Transform normalHolder;
    public Transform chaosHolder;

    private List<GameObject> normalCubes = new List<GameObject>();
    private List<GameObject> chaosCubes = new List<GameObject>();
    private Coroutine currentAction;
    public bool goingToFight;
    private float xLength = 200;
    private float yLength = 150;
    private float delta = 2.5f;
    private float planeHeightUp = 4;
    private float planeHeightDown = -1;
    private float platformSpeed = .25f;
    private float colorVar = .5f;
    private float accentProb = .005f;

    public bool up = false;
    public bool down = false;


    void Start()
    {
        if (!debug)
        {
            chaosHolder.gameObject.SetActive(true);
            normalHolder.gameObject.SetActive(false);
            plane.SetActive(true);

            for (int i = 0; i < normalHolder.childCount; i++)
            {
                normalCubes.Add(normalHolder.GetChild(i).gameObject);
            }
            for (int i = 0; i < chaosHolder.childCount; i++)
            {
                chaosCubes.Add(chaosHolder.GetChild(i).gameObject);
            }
        }


        //PlaceObjects();
        Debug.Log("start");
    }

    private void Update()
    {
        //Debug.Log(currentAction);
        if (up)
        {
            //StartCoroutine(raisePlane());
            activateMatch(true);
            up = false;
        }
        if (down)
        {
            //StartCoroutine(lowerPlane());
            activateMatch(false);
            down = false;
        }
    }

    public void Initialize(Color setColorMain, Color setColorAccent)
    {
        //chaosHolder.gameObject.SetActive(true);
        //normalHolder.gameObject.SetActive(false);
        if (!debug)
        {
            for (int i = 0; i < normalHolder.childCount; i++)
            {
                normalCubes.Add(normalHolder.GetChild(i).gameObject);
            }
            for (int i = 0; i < chaosHolder.childCount; i++)
            {
                chaosCubes.Add(chaosHolder.GetChild(i).gameObject);
            }

            //PlaceObjects();
            assignColors(setColorMain, setColorAccent);
        }
    }

    private void PlaceObjects()
    {
        bool odd = true;

        for (float x = -xLength / 2; x < xLength / 2; x += delta)
        {
            // Stagger the odd rows.
            Vector3 stagger;
            if (odd)
            {
                stagger = new Vector3(0, 0, delta / 2);
                odd = false;
            }
            else
            {
                stagger = Vector3.zero;
                odd = true;
            }

            for (float y = -yLength / 2; y < yLength / 2; y += delta)
            {
                // Add to the normal transform.
                Vector3 pos = new Vector3(x, 0, y) + transform.localPosition + stagger;
                GameObject n = Instantiate(cube, pos, Quaternion.identity, normalHolder);
                n.transform.position = pos;
                n.transform.eulerAngles = new Vector3(45, 45, 45);
                normalCubes.Add(n);

                // Add to the chaos transform.
                GameObject c = Instantiate(cubeActive, pos, Quaternion.identity, chaosHolder);
                c.transform.position = pos;
                chaosCubes.Add(c);
            }
        }
    }

    public void assignColors(Color setColorMain, Color setColorAccent)
    {
        colorMain = setColorMain;
        colorAccent = setColorAccent;
        // Set the plane's color.
        plane.GetComponent<Renderer>().material.color = colorAccent;

        // Set the cubes' colors with a chance of being accented.
        for (int cube = 0; cube < normalCubes.Count; cube++)
        {
            Color temp = new Color();
            if (Random.Range(0f, 1f) < accentProb)
            {
                temp = colorAccent;
            }
            else
            {
                temp = new Color(
                    colorMain.r + Random.Range(-colorVar, colorVar),
                    colorMain.g + Random.Range(-colorVar, colorVar),
                    colorMain.b + Random.Range(-colorVar, colorVar));
            }

            normalCubes[cube].GetComponent<Renderer>().material.color = temp;
            chaosCubes[cube].GetComponent<Renderer>().material.color = temp;
        }
    }
    
    public void activateMatch(bool b)
    {
        if (!debug)
        {
            goingToFight = b;
            if (currentAction != null)
            {
                StopCoroutine(currentAction);
            }
            Debug.Log("1");
            if (b)
            {
                currentAction = StartCoroutine(activate());
            }
            else
            {
                currentAction = StartCoroutine(deactivate());
            }
        }
    }

    private IEnumerator activate()
    {
        yield return raisePlane();
        chaosHolder.gameObject.SetActive(true);
        normalHolder.gameObject.SetActive(false);
        yield return lowerPlane();
    }
    private IEnumerator deactivate()
    {
        yield return raisePlane();
        chaosHolder.gameObject.SetActive(false);
        normalHolder.gameObject.SetActive(true);
        yield return lowerPlane();
    }
    private IEnumerator raisePlane()
    {
        Debug.Log("plane raising");
        float time = 0;
        while (plane.transform.localPosition.y < planeHeightUp)
        {
            //plane.transform.Translate(Vector3.up * platformSpeed);
            plane.transform.localPosition = plane.transform.localPosition + time * new Vector3(0, platformSpeed, 0);
            yield return new WaitForSeconds(.01f);
            time += .01f;
        }
        Debug.Log("plane raised");
    }
    private IEnumerator lowerPlane()
    {
        Debug.Log("plane lowering");
        float time = 0;
        while (plane.transform.localPosition.y > planeHeightDown)
        {
            //plane.transform.Translate(Vector3.down * platformSpeed);
            plane.transform.localPosition = plane.transform.localPosition - time *  new Vector3(0, platformSpeed, 0);
            yield return new WaitForSeconds(.01f);
            time += .01f;
        }
        Debug.Log("plane lowered");
    }
}
