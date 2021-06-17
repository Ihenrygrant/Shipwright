using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GridLayoutSizeController : MonoBehaviour {

    GridLayoutGroup gridLayout;

    public static float maxItemSize = 50.0f;

    Vector2[] itemSizes = { new Vector2(maxItemSize, maxItemSize), new Vector2(maxItemSize*(2/3), maxItemSize*(2/3)), new Vector2(maxItemSize * (1 / 3), maxItemSize * (1 / 3))};

	// Use this for initialization
	void Start () {
        gridLayout = GetComponent<GridLayoutGroup>();
        updateItemSize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateItemSize()
    {
        gridLayout.cellSize = itemSizes[0];
    }
}
