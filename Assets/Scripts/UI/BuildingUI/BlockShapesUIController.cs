using UnityEngine;
using UnityEngine.UI;

public class BlockShapesUIController : MonoBehaviour {
    HorizontalLayoutGroup HLG;
    int currentSelected = 0;
    int maxColumns = 0;

    void Start()
    {
        HLG = gameObject.GetComponent<HorizontalLayoutGroup>();

        currentSelected = 0;
        maxColumns = HLG.transform.childCount;
    }

    void updateSelection(BuildingUIInterface.cursorDirection selection)
    {
        Transform previtem = HLG.transform.GetChild(currentSelected);
        previtem.GetComponent<Outline>().enabled = false;

        if (selection == BuildingUIInterface.cursorDirection.left)
        {
            if (currentSelected == 0)
            {
                currentSelected = maxColumns - 1;
            }
            else
            {
                currentSelected--;
            }
        }
        else if (selection == BuildingUIInterface.cursorDirection.right)
        {
            if (currentSelected == maxColumns - 1)
            {
                currentSelected = 0;
            }
            else
            {
                currentSelected++;
            }

        }
        else if (selection == BuildingUIInterface.cursorDirection.up)
        {
            transform.parent.SendMessage("changeActiveCursorUI", BuildingUIInterface.cursorDirection.up);
        }
        else if (selection == BuildingUIInterface.cursorDirection.down)
        {
            transform.parent.SendMessage("changeActiveCursorUI", BuildingUIInterface.cursorDirection.down);
        }

        Transform item = HLG.transform.GetChild(currentSelected);
        item.GetComponent<Outline>().enabled = true;

        //Debug.Log(currentSelected + " rows " + maxRows + " columns " + maxColumns);
    }

}
