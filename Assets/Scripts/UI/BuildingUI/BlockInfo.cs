using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BlockInfo : MonoBehaviour
{
    public int blockRef;
    
    void Awake()
    {
        Button button = gameObject.AddComponent<Button>();

        button.onClick.AddListener(SetBlockSelection);
    }

    void SetBlockSelection()
    {
        PlayerInfo.blockSelection = blockRef;
    }
}