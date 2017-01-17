using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class MenuButtons : EventTrigger
{


    public GameObject block;                // The block that corresponds to this button.


    public override void OnSelect(BaseEventData eventData)
    {
        block.GetComponent<UIBlock>().highlight();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        block.GetComponent<UIBlock>().unHighlight();
    }
}