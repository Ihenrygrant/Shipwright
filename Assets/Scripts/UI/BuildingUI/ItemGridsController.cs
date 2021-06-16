using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGridsController : MonoBehaviour {

    private int activeIndex = 0;

    void Start()
    {
        foreach (RectTransform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        transform.GetChild(activeIndex).gameObject.SetActive(true);
    }

    void NotifyInactiveCursor()
    {
        transform.GetChild(this.activeIndex).SendMessage("NotifyInactiveCursor");
    }

    void changeActiveCursorUI(BuildingUIInterface.cursorDirection direction)
    {
        transform.parent.SendMessage("changeActiveCursorUI", direction);
    }


    void updateSelection(BuildingUIInterface.cursorDirection selection)
    {
        transform.GetChild(activeIndex).gameObject.SendMessage("updateSelection", selection); //TODO send to activepage
    }

}
