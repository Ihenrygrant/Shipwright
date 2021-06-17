using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour {
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
