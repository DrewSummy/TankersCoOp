using UnityEngine;
using System.Collections;

public class UIBlock : MonoBehaviour
{

    public TextMesh menuText;

    private string blockText;
    private TextMesh[] textU;
    private TextMesh[] textH;
    private GameObject unHighlighted;
    private GameObject highlighted;

    //TODO: add audio on collision

    void Awake()
    {
        // Set the private variables.
        unHighlighted = transform.GetChild(1).gameObject;
        highlighted = transform.GetChild(2).gameObject;
        textU = unHighlighted.GetComponentsInChildren<TextMesh>();
        textH = highlighted.GetComponentsInChildren<TextMesh>();
    }


    public void setText(string str)
    {
        blockText = str;
        foreach (TextMesh t in textU)
        {
            t.text = str;
        }

        foreach (TextMesh t in textH)
        {
            t.text = str;
        }
    }

    public void highlight()
    {
        //TODO: add highlighted bars along corners
        highlighted.SetActive(true);
        unHighlighted.SetActive(false);

        menuText.text = blockText;
    }

    public void unHighlight()
    {
        highlighted.SetActive(false);
        unHighlighted.SetActive(true);
    }
}
