using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsSceneController : MonoBehaviour
{
    public Button backButton;
    public string backSceneName = "EndScene";

    void Start()
    {
        if (backButton) backButton.onClick.AddListener(() => SceneManager.LoadScene(backSceneName));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(backSceneName);
    }
}
