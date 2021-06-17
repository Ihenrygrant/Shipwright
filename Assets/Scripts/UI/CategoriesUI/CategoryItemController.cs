using UnityEngine;

public class CategoryItemController : MonoBehaviour {
    public GameObject panel;

    void Awake()
    {
        Transform ItemGrids = transform.parent.parent.Find("ItemGrids");

        panel = ItemGrids.Find(transform.name).gameObject;
    }

    public bool activatePanel()
    {
        if (panel == null) return false;
        panel.SetActive(true);
        return true;
    }

    public void deactivatePanel()
    {
        if (panel == null) return;
        panel.SetActive(false);
    }
}
