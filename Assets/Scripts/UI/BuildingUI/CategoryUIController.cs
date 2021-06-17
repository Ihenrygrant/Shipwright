using UnityEngine;
using UnityEngine.UI;

public class CategoryUIController : MonoBehaviour {

    Text categoryTitle;

    bool activeCursor = false;

    HorizontalLayoutGroup HLG;
    int currentSelected = 0;
    int maxColumns = 0;

    void Start()
    {
        HLG = gameObject.GetComponent<HorizontalLayoutGroup>();

        activeCursor = true;

        currentSelected = 0;
        maxColumns = HLG.transform.childCount;

        categoryTitle = transform.parent.Find("Category Title").GetComponent<Text>();
        categoryTitle.text = HLG.transform.GetChild(currentSelected).name;
    }


    public void NotifyInactiveCursor()
    {

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



            transform.parent.SendMessage("setUIState", currentSelected);
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

            transform.parent.SendMessage("setUIState", currentSelected);
        }
        else if (selection == BuildingUIInterface.cursorDirection.up)
        {

        }
        else if (selection == BuildingUIInterface.cursorDirection.down)
        {
            transform.parent.SendMessage("changeActiveCursorUI", BuildingUIInterface.cursorDirection.down);
        }

        Transform item = HLG.transform.GetChild(currentSelected);
        item.GetComponent<Outline>().enabled = true;

        categoryTitle.text = item.name;
    }
}
