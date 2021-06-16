using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour {
    void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(doLoadScene);
    }

    void doLoadScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
