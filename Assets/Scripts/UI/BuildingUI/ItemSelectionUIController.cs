using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class BuildingUIInterface
{
    public enum cursorDirection
    {
        up,
        down,
        left,
        right
    }
}

public class ItemSelection{
    public int blockSelection = 0;
}

public class ItemSelectionUIController : MonoBehaviour {

    int uiIndex = 0;
    int uiState = 0;

    List<RectTransform> buildingLGs = new List<RectTransform>();

    RectTransform rect;

    public List<RectTransform> I0 = new List<RectTransform>();
    public List<RectTransform> I1 = new List<RectTransform>();
    public List<RectTransform> I2 = new List<RectTransform>();
    public List<RectTransform> I3 = new List<RectTransform>();
    public List<RectTransform> I4 = new List<RectTransform>();
    public List<List<RectTransform>> BuildingUIStates = new List<List<RectTransform>>();

    void Start()
    {
        BuildingUIStates.Add(I0);
        BuildingUIStates.Add(I1);
        BuildingUIStates.Add(I2);
        BuildingUIStates.Add(I3);
        BuildingUIStates.Add(I4);

        Transform Category = transform.Find("Category");
        Transform BlockShapes = transform.Find("BlockShapes");
        Transform ItemGrids = transform.Find("ItemGrids");
        Transform PageControl = transform.Find("PageControl");
        //Transform Blocks = ItemGrids.Find("Blocks");

        I0.Add(Category.GetComponent<RectTransform>());
        I0.Add(BlockShapes.GetComponent<RectTransform>());
        I0.Add(ItemGrids.GetComponent<RectTransform>());
        I0.Add(PageControl.GetComponent<RectTransform>());

        foreach (RectTransform child in Category)
        {
            I1.Add(child.GetComponent<RectTransform>());
        }



        foreach (RectTransform child in transform)
        {
            buildingLGs.Add(child);
        }

        BuildingUIStates.Add(new List<RectTransform>() { rect, rect});

    }

    void Update()
    {
        selectionKeys();
    }

    void selectionKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            BuildingUIStates[uiState][uiIndex].SendMessage("updateSelection", BuildingUIInterface.cursorDirection.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            BuildingUIStates[uiState][uiIndex].SendMessage("updateSelection", BuildingUIInterface.cursorDirection.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            BuildingUIStates[uiState][uiIndex].SendMessage("updateSelection", BuildingUIInterface.cursorDirection.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BuildingUIStates[uiState][uiIndex].SendMessage("updateSelection", BuildingUIInterface.cursorDirection.down);
        }
    }

    void UpdateActiveCursor(BuildingUIInterface.cursorDirection direction)
    {
        //foreach (RectTransform child in buildingLGs)
        //{
        //    child.gameObject.SendMessage("NotifyInactiveCursor");
        //}
    }

    void setUIState(int uiState)
    {
        clearUI();
        this.uiState = uiState;
        activateUI();
    }

    public void changeActiveCursorUI(BuildingUIInterface.cursorDirection direction)
    {
        if(direction == BuildingUIInterface.cursorDirection.up)
        {
            if(uiIndex == 0)
            {

            }
            else
            {
                UpdateActiveCursor(BuildingUIInterface.cursorDirection.up);
                BuildingUIStates[uiState][uiIndex].gameObject.GetComponent<Outline>().enabled = false;
                uiIndex--;
                BuildingUIStates[uiState][uiIndex].gameObject.GetComponent<Outline>().enabled = true;
            }
        }
        else if(direction == BuildingUIInterface.cursorDirection.down)
        {
            if (uiIndex == buildingLGs.Count-1)
            {

            }
            else
            {
                UpdateActiveCursor(BuildingUIInterface.cursorDirection.down);
                BuildingUIStates[uiState][uiIndex].gameObject.GetComponent<Outline>().enabled = false;
                uiIndex++;
                BuildingUIStates[uiState][uiIndex].gameObject.GetComponent<Outline>().enabled = true;
            }
        }
    }



    void activateUI()
    {
        for (int i = 1; i < BuildingUIStates[uiState].Count; i++)
        {
            BuildingUIStates[uiState][i].gameObject.SetActive(true);
        }
    }

    void clearUI()
    {
        for (int i = 1; i < BuildingUIStates[uiState].Count; i++)
        {
            BuildingUIStates[uiState][i].gameObject.SetActive(false);
        }
    }


}
