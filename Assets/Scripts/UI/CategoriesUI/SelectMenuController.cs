using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMenuController : MonoBehaviour {

    public GameObject backPanel;

    public List<RectTransform> items = new List<RectTransform>();
    RectTransform rectTransform;
    

    public float itemWidth = 100.0f;
    public float paddingWidth = 5.0f;

    //public float curPosition = 0;
    public Vector3 curPosition = new Vector3(0,0,0);
    public int curSelection = 0;

    public float speed = 0.1f;

    public bool isFading = false;

    void Start () {

            itemWidth = transform.GetChild(0).GetChild(0).transform.GetComponent<RectTransform>().rect.width;
            paddingWidth = transform.GetChild(0).GetComponent<HorizontalLayoutGroup>().spacing;

            foreach (RectTransform child in transform.GetChild(0))
            {
                items.Add(child);
            }

        items[curSelection].GetComponent<Image>().color = Color.gray;

        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        curPosition.x = transform.GetComponent<RectTransform>().rect.width / 3;

        transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(transform.GetComponent<RectTransform>().rect.width / 3,-75,0);
    }

    void OnEnable()
    {
        curPosition.y = 0;
    }

    // Update is called once per frame
    void Update () {
        if (!isFading)
        {
            keys();
        }
        Slerp();
    }

    void disable()
    {
        isFading = false;
        gameObject.SetActive(false);
    }

    void keys()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (curSelection != 0)
            {
                items[curSelection].GetComponent<Image>().color = Color.white;
                curSelection--;
                items[curSelection].GetComponent<Image>().color = Color.gray;

                curPosition.x += itemWidth + paddingWidth;
                //rectTransform.localPosition = new Vector3(curPosition, 0, 0);
            }

        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (curSelection != items.Count - 1)
            {
                items[curSelection].GetComponent<Image>().color = Color.white;
                curSelection++;
                items[curSelection].GetComponent<Image>().color = Color.gray;

                curPosition.x -= itemWidth + paddingWidth;
                //rectTransform.localPosition = new Vector3(curPosition, 0, 0);
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (items[curSelection].GetComponent<CategoryItemController>().activatePanel())
            {
                isFading = true;
                curPosition += new Vector3(0, rectTransform.rect.height, 0);
                Invoke("disable", 0.5f);
            }
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (backPanel == null) return;

            curPosition = new Vector3(curPosition.x, -rectTransform.rect.height, 0);
            Invoke("disable", 0.5f);
            backPanel.SetActive(true);
        }


    }

    // Time to move from sunrise to sunset position, in seconds.
    public float journeyTime = 10.0f;

    // The time at which the animation started.
    private float startTime;

    void Slerp()
    {

        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, curPosition, speed);

        //rectTransform.localPosition = Vector3.Slerp(curPosition, 0, 0);
    }

}
