using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour {
	void Awake () {
        transform.GetComponent<Button>().onClick.AddListener(doExitGame);
	}

    void doExitGame()
    {
        Application.Quit();
    }
}
