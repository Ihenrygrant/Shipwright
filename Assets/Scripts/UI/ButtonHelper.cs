using UnityEngine;
using UnityEngine.UI;

public class ButtonHelper : MonoBehaviour {

    public KeyCode key;
	
	void Update () {
        if (Input.GetKeyUp(key))
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
