using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GridLayoutSelectionController : MonoBehaviour {
    GridLayoutGroup itemPortGLG;

    public GameObject buildingMarker;

    Vector2 currentSelected = new Vector2(0, 0);
    int maxColumns = 0, maxRows = 0;

    private UnityAction someListener;

    void Awake()
    {
        itemPortGLG = gameObject.GetComponent<GridLayoutGroup>();
        someListener = new UnityAction(ResolutionUpdateNotification);
    }

    void OnEnable()
    {
        ResolutionHandler.StartListening("ResolutionUpdateNotification", someListener);
    }

    void OnDisable()
    {
        ResolutionHandler.StopListening("ResolutionUpdateNotification", someListener);
        ClearSelection();
    }

    public void NotifyInactiveCursor()
    {
        ClearSelection();
    }

    void ResolutionUpdateNotification()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemPortGLG.gameObject.GetComponent<RectTransform>());

        GetColumnAndRow(itemPortGLG, out maxRows, out maxColumns);
        currentSelected = new Vector2(0, 0);

        foreach (RectTransform child in itemPortGLG.transform)
        {
            child.GetComponent<Outline>().enabled = false;
        }

        itemPortGLG.transform.GetChild(0).GetComponent<Outline>().enabled = true;
    }

    void ClearSelection()
    {

        foreach (RectTransform child in itemPortGLG.transform)
        {
            child.GetComponent<Outline>().enabled = false;
        }

        currentSelected = new Vector2(0, 0);
        Transform item = itemPortGLG.transform.GetChild((int)(currentSelected.y + (maxColumns * currentSelected.x)));
        item.GetComponent<Outline>().enabled = true;
    }

    // Use this for initialization
    void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemPortGLG.gameObject.GetComponent<RectTransform>());
        GetColumnAndRow(itemPortGLG, out maxRows, out maxColumns);
    }

    void updateSelection(BuildingUIInterface.cursorDirection selection)
    {
        Transform previtem = itemPortGLG.transform.GetChild((int)(currentSelected.y + (maxColumns * currentSelected.x)));
        previtem.GetComponent<Outline>().enabled = false;

        if (selection == BuildingUIInterface.cursorDirection.left)
        {
            if (currentSelected.y == 0)
            {
                currentSelected.y = maxColumns - 1;
                int indexedSelection = (int)(currentSelected.y + (maxColumns * currentSelected.x));
                if (itemPortGLG.transform.childCount <= indexedSelection) currentSelected.y = itemPortGLG.transform.childCount % maxColumns - 1;
            }
            else
            {
                currentSelected.y--;
            }
        }
        else if (selection == BuildingUIInterface.cursorDirection.right)
        {
            if (currentSelected.y == maxColumns - 1)
            {
                currentSelected.y = 0;
            }
            else
            {
                currentSelected.y++;
                int indexedSelection = (int)(currentSelected.y + (maxColumns * currentSelected.x));
                if (itemPortGLG.transform.childCount <= indexedSelection) currentSelected.y = 0;
            }
        }
        else if (selection == BuildingUIInterface.cursorDirection.up)
        {
            if (currentSelected.x == 0)
            {
                transform.parent.SendMessage("changeActiveCursorUI", BuildingUIInterface.cursorDirection.up);

                //int indexedSelection = (int)(currentSelected.y + (maxColumns * currentSelected.x));
                //if (itemPortGLG.transform.childCount <= indexedSelection)
                //{
                //    transform.parent.SendMessage("changeActiveCursorUI", BuildingUIInterface.cursorDirection.down);
                //    //currentSelected.x = itemPortGLG.transform.childCount % maxColumns - 1;
                //}
                //else
                //{
                //    currentSelected.x = maxRows - 1;
                //}
            }
            else
            {
                currentSelected.x--;
            }
        }
        else if (selection == BuildingUIInterface.cursorDirection.down)
        {
            if (currentSelected.x == maxRows - 1)
            {
                currentSelected.x = 0;
            }
            else
            {
                currentSelected.x++;

                int indexedSelection = (int)(currentSelected.y + (maxColumns * currentSelected.x));
                if (itemPortGLG.transform.childCount <= indexedSelection)
                {
                    currentSelected.x--;
                    transform.parent.SendMessage("changeActiveCursorUI", BuildingUIInterface.cursorDirection.down);
                }
                else
                {

                }
            }
        }

        Transform item = itemPortGLG.transform.GetChild((int)(currentSelected.y + (maxColumns * currentSelected.x)));
        item.GetComponent<Outline>().enabled = true;



        //Material material = item.GetComponent<BlockInfo>().blockRef.GetComponent<Renderer>().sharedMaterial;
        //buildingMarker.GetComponent<MeshRenderer>().material = material;

        PlayerInfo.blockSelection = item.GetComponent<BlockInfo>().blockRef;
        //Debug.Log(currentSelected + " rows " + maxRows + " columns " + maxColumns);
    }

    void GetColumnAndRow(GridLayoutGroup glg, out int column, out int row)
    {
        column = 0;
        row = 0;

        if (glg.transform.childCount == 0)
            return;

        //Column and row are now 1
        column = 1;
        row = 1;

        //Get the first child GameObject of the GridLayoutGroup
        RectTransform firstChildObj = glg.transform.
            GetChild(0).GetComponent<RectTransform>();

        Vector2 firstChildPos = firstChildObj.anchoredPosition;
        bool stopCountingRow = false;

        //Loop through the rest of the child object
        for (int i = 1; i < glg.transform.childCount; i++)
        {
            //Get the next child
            RectTransform currentChildObj = glg.transform.
           GetChild(i).GetComponent<RectTransform>();

            Vector2 currentChildPos = currentChildObj.anchoredPosition;

            //if first child.x == otherchild.x, it is a column, ele it's a row
            if (firstChildPos.x == currentChildPos.x)
            {
                column++;
                //Stop couting row once we find column
                stopCountingRow = true;
            }
            else
            {
                if (!stopCountingRow)
                    row++;
            }
        }
    }
}
