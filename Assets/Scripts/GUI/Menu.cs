using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    public GameObject Camera;
    public GameObject block;
    public GameObject container;
    public GameObject deadBlockHolder;
    
    private GameObject menu;
    private Transform blockHolder;
    private GameObject[] blocks = new GameObject[9];
    private int conHeight1 = 20;
    private int conHeight2 = 0;
    private int conHeight3 = 5;
    private int conHeight4 = 2;
    private float blockHeight = 4.1f;
    private float[] offsetRange = new float[2];
    private float containerSpeedFast = 12f;
    private float containerSpeedSlow = 4f;
    private bool idle = false;
    private float idleRadius = .085f;
    public bool containerMoving = false;                    // Used to make sure that inputs are not taken while the container is unavailable.

    //TODO: add clicking audio when raising container




    // To send the blocks to the GUI menu.
    public void beginDisplay()
    {
        placeMenu();
    }

    // To send the blocks to the GUI menu.
    public GameObject[] initializeAndSendBlocks()
    {
        initialization();
        placeContainers();
        placeBlocks();
        return blocks;
    }

    // Initialize.
    private void initialization()
    {
        // Set holder.
        blockHolder = new GameObject("Blocks").transform;
        blockHolder.SetParent(transform);

        // Set variables.
        offsetRange[0] = -2;
        offsetRange[1] = 2;
    }
	
    // Place containers.
    private void placeContainers()
    {
        // Menu
        menu = Instantiate(container) as GameObject;
        menu.transform.position = conHeight1 * Vector3.up;
        menu.name = "Container Menu";
        menu.transform.SetParent(transform);
    }

    // Place blocks.
    private void placeBlocks()
    {
        for (int i = 0; i < 9; i++)
        {
            blocks[i] = Instantiate(block) as GameObject;
            blocks[i].transform.SetParent(blockHolder);
            blocks[i].transform.position = deadBlockHolder.transform.position
                + Vector3.up * i * blockHeight;
            blocks[i].transform.rotation = Quaternion.identity;
            blocks[i].GetComponent<Rigidbody>().useGravity = false;
            blocks[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            blocks[i].GetComponent<UIBlock>().menuText = menu.transform.GetChild(1).GetComponent<TextMesh>();
        }

        // Menu
        blocks[0].GetComponent<UIBlock>().setText("SOLO");
        blocks[0].GetComponent<UIBlock>().unHighlight();
        blocks[0].name = "Block Solo";
        blocks[0].transform.rotation = Quaternion.identity;
        blocks[1].GetComponent<UIBlock>().setText("CO-OP");
        blocks[1].GetComponent<UIBlock>().unHighlight();
        blocks[1].name = "Block Co-op";
        blocks[1].transform.rotation = Quaternion.identity;
        blocks[2].GetComponent<UIBlock>().setText("SETTINGS");
        blocks[2].GetComponent<UIBlock>().unHighlight();
        blocks[2].name = "Block Settings";
        blocks[2].transform.rotation = Quaternion.identity;

        // Solo
        blocks[3].GetComponent<UIBlock>().setText("PLAY");
        blocks[3].GetComponent<UIBlock>().unHighlight();
        blocks[3].name = "Block Solo Play";
        blocks[3].transform.rotation = Quaternion.identity;
        blocks[4].GetComponent<UIBlock>().setText("BACK");
        blocks[4].GetComponent<UIBlock>().unHighlight();
        blocks[4].name = "Block Solo Back";
        blocks[4].transform.rotation = Quaternion.identity;

        // Coop
        blocks[5].GetComponent<UIBlock>().setText("PLAY");
        blocks[5].GetComponent<UIBlock>().unHighlight();
        blocks[5].name = "Block Coop Play";
        blocks[5].transform.rotation = Quaternion.identity;
        blocks[6].GetComponent<UIBlock>().setText("BACK");
        blocks[6].GetComponent<UIBlock>().unHighlight();
        blocks[6].name = "Block Coop Back";
        blocks[6].transform.rotation = Quaternion.identity;

        // Settings
        blocks[7].GetComponent<UIBlock>().setText("SETTING 1");
        blocks[7].GetComponent<UIBlock>().unHighlight();
        blocks[7].name = "Block Setting 1";
        blocks[7].transform.rotation = Quaternion.identity;
        blocks[8].GetComponent<UIBlock>().setText("BACK");
        blocks[8].GetComponent<UIBlock>().unHighlight();
        blocks[8].name = "Block Settings Back";
        blocks[8].transform.rotation = Quaternion.identity;
    }

    // Called when the respective mode needs to be displayed.
    public void placeMenu()
    {
        menu.transform.GetComponentInChildren<TextMesh>().text = "MENU";
        StartCoroutine(replaceMenu());
    }
    public void placeSolo()
    {
        menu.transform.GetComponentInChildren<TextMesh>().text = "SOLO";
        StartCoroutine(replaceSolo());
    }
    public void placeCoop()
    {
        menu.transform.GetComponentInChildren<TextMesh>().text = "CO-OP";
        StartCoroutine(replaceCoop());
    }
    public void placeSettings()
    {
        menu.transform.GetComponentInChildren<TextMesh>().text = "SETTINGS";
        StartCoroutine(replaceSettings());
    }

    //TODO: interrupt placing modes by checking if it is the correct selection

    // Cancel the current mode and place a new one.
    private IEnumerator replaceMenu()
    {
        // Raise, inactivate blocks, place blocks, lower.
        yield return raiseContainer();

        inactivateBlocks();

        // Place the blocks (0, 2).
        Vector3 dropLocation = menu.transform.GetChild(2).transform.position;
        for (int i = 0; i < 3; i++)
        {
            blocks[i].SetActive(true);
            blocks[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            blocks[i].GetComponent<Rigidbody>().useGravity = true;
            blocks[i].transform.position = dropLocation - i * blockHeight * Vector3.up
            + Random.Range(offsetRange[0], offsetRange[1]) * Vector3.right + Random.Range(offsetRange[0], offsetRange[1]) * Vector3.forward;
            blocks[i].transform.rotation = Quaternion.identity;
            blocks[i].transform.Rotate(new Vector3(0, Random.Range(0, 90), 0));
        }

        yield return lowerContainer();
        yield return idleContainer();
    }
    private IEnumerator replaceSolo()
    {
        // Raise, inactivate blocks, place blocks, lower.
        yield return raiseContainer();

        inactivateBlocks();
        
        // Place the blocks (3, 4).
        Vector3 dropLocation = menu.transform.GetChild(2).transform.position;
        for (int i = 3; i < 5; i++)
        {
            blocks[i].SetActive(true);
            blocks[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            blocks[i].GetComponent<Rigidbody>().useGravity = true;
            blocks[i].transform.position = dropLocation - (i - 3) * blockHeight * Vector3.up
            + Random.Range(offsetRange[0], offsetRange[1]) * Vector3.right + Random.Range(offsetRange[0], offsetRange[1]) * Vector3.forward;
            blocks[i].transform.rotation = Quaternion.identity;
            blocks[i].transform.Rotate(new Vector3(0, Random.Range(0, 90),0));
        }
        yield return lowerContainer();
        yield return idleContainer();
    }
    private IEnumerator replaceCoop()
    {
        // Raise, inactivate blocks, place blocks, lower.
        yield return raiseContainer();

        inactivateBlocks();

        // Place the blocks (5, 6).
        Vector3 dropLocation = menu.transform.GetChild(2).transform.position;
        for (int i = 5; i < 7; i++)
        {
            blocks[i].SetActive(true);
            blocks[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            blocks[i].GetComponent<Rigidbody>().useGravity = true;
            blocks[i].transform.position = dropLocation - (i - 5) * blockHeight * Vector3.up;
            //+ Random.Range(offsetRange[0], offsetRange[1]) * Vector3.right + Random.Range(offsetRange[0], offsetRange[1]) * Vector3.forward;
            blocks[i].transform.rotation = Quaternion.identity;
            //blocks[i].transform.Rotate(new Vector3(0, Random.Range(0, 90), 0));
        }

        yield return lowerContainer();
        yield return idleContainer();
    }
    private IEnumerator replaceSettings()
    {
        // Raise, inactivate blocks, place blocks, lower.
        yield return raiseContainer();

        inactivateBlocks();

        // Place the blocks (7, 8).
        Vector3 dropLocation = menu.transform.GetChild(2).transform.position;
        for (int i = 7; i < 9; i++)
        {
            blocks[i].SetActive(true);
            blocks[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            blocks[i].GetComponent<Rigidbody>().useGravity = true;
            blocks[i].transform.position = dropLocation - (i - 7) * blockHeight * Vector3.up
            + Random.Range(offsetRange[0], offsetRange[1]) * Vector3.right + Random.Range(offsetRange[0], offsetRange[1]) * Vector3.forward;
            blocks[i].transform.rotation = Quaternion.identity;
            blocks[i].transform.Rotate(new Vector3(0, Random.Range(0, 90), 0));
        }

        yield return lowerContainer();
        yield return idleContainer();
    }

    // Animation to bring the container into display.
    public IEnumerator lowerContainer()
    {
        idle = false;
        while (menu.transform.position.y > conHeight3)
        {
            Debug.Log("moving");
            menu.GetComponent<Rigidbody>().velocity = containerSpeedFast * Vector3.down;
            //menu.GetComponent<Rigidbody>().AddForce(Vector3.down * 350);
            yield return new WaitForSeconds(.01f);
        }
        while (menu.transform.position.y > conHeight2)
        {
            Debug.Log("moving");
            menu.GetComponent<Rigidbody>().velocity = containerSpeedSlow * Vector3.down;
            //menu.GetComponent<Rigidbody>().AddForce(Vector3.up * 100);
            yield return new WaitForSeconds(.01f);
        }
        menu.GetComponent<Rigidbody>().velocity = Vector3.zero;
        idle = true;
        Debug.Log("done");
        // Reset containerMoving to false so that it can move again.
        containerMoving = false;
    }

    // Animation to hold the container in display.
    public IEnumerator idleContainer()
    {

        menu.transform.position = Vector3.up * conHeight2;
        menu.GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return new WaitForSeconds(.01f);

        float time = 0;
        while (idle)
        {
            menu.GetComponent<Rigidbody>().position = idleRadius * Mathf.Sin(2 * time) * Vector3.up + Vector3.up * conHeight2;
            time += .01f;

            yield return new WaitForSeconds(.01f);
        }
    }

    // Called when reaching a new menu.
    private void cancel()
    {
        inactivateBlocks();
        StartCoroutine(raiseContainer());
    }

    // Animation to lift the container out of display.
    private IEnumerator raiseContainer()
    {
        idle = false;


        while (menu.transform.position.y < conHeight4)
        {
            menu.GetComponent<Rigidbody>().velocity = containerSpeedSlow * Vector3.up;
            yield return new WaitForSeconds(.01f);
        }
        while (menu.transform.position.y < conHeight1)
        {
            menu.GetComponent<Rigidbody>().velocity = containerSpeedFast * Vector3.up;
            yield return new WaitForSeconds(.01f);
        }
        menu.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    // Helper for raiseContainer.
    private void inactivateBlocks()
    {
        for(int block = 0; block < blocks.Length; block++)
        {
            blocks[block].SetActive(false);
            blocks[block].transform.position = deadBlockHolder.transform.position
                + Vector3.up * block * blockHeight;
            blocks[block].transform.rotation = Quaternion.identity;
            blocks[block].GetComponent<Rigidbody>().useGravity = false;
            blocks[block].GetComponent<Rigidbody>().velocity = Vector3.zero;
            blocks[block].GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    


    private IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
    }
}