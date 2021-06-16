using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Depcrecated
/// </summary>
public class BlockSelectionUIController : MonoBehaviour {

    public RectTransform selectionPanel;
    ushort selectionIndex = 0;
    ushort selectionSize = 0;


	// Use this for initialization
	void Start () {
        selectionSize = (ushort)(selectionPanel.childCount-1);
        //PlayerInfo.blockSelection = selectionPanel.transform.GetChild(0).GetComponent<BlockInfo>().blockRef;
    }
	
	// Update is called once per frame
	void Update () {
        SelectionKeys();
	}

    void SelectionKeys()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectionIndex != selectionSize)
            {
                selectionIndex++;
                UpdateSelection();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectionIndex != 0)
            {
                selectionIndex--;
                UpdateSelection();
            }
        }
    }

    void UpdateSelection()
    {
        ushort iterator = 0;
        foreach(RectTransform child in selectionPanel)
        {
            if(iterator == selectionIndex)
            {
                //PlayerInfo.blockSelection = child.GetComponent<BlockInfo>().blockRef;
            }else{

            }
            iterator++;
        }
    }
}
