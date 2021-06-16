using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CreativeItemPanelController : MonoBehaviour {

    public GameObject BlockPanel;


	// Use this for initialization
	void Start () {
        GameObject panel = Resources.Load("Panel") as GameObject;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Atlas16x16");

        //SpriteAtlas sprites = Resources.Load("Atlas16x16") as SpriteAtlas;

        int blockRef = 0;
        foreach (KeyValuePair<string, Dictionary<string, Vector2[]>> entry in TextureMapController.BlockUVDictionary)
        {
            GameObject blockPanel = Instantiate(panel, BlockPanel.transform);

            //List<string> keyList = new List<string>(entry.Value.Keys);
            //if (sprites == null) return;
            //blockPanel.GetComponent<Image>().sprite = sprites.GetSprite(keyList[2]);

            int temp = blockRef;
            Button button =  blockPanel.AddComponent<Button>();
            button.onClick.AddListener(() => SetBlockSelection(temp));
            blockRef++;


            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].name == entry.Key)
                {
                    blockPanel.GetComponent<Image>().sprite = sprites[i];

                
                    break;
                }
            }



        }
    }

    private void OnDisable()
    {
        PlayerInfo.SetPlayerState(PlayerStates.Flying);
    }

    void SetBlockSelection(int blockRef)
    {
        Debug.Log(blockRef);
        PlayerInfo.blockSelection = blockRef;
    }
}
