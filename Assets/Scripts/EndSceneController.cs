using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    [Header("Labels")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI finalLevelText;

    [Header("Buttons")]
    public Button restartButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Scene Names")]
    public string startSceneName = "StartScene";
    public string creditsSceneName = "CreditsScene";

    void Start()
    {
        if (finalScoreText) finalScoreText.text = $"Final Score: {SceneLoader.FinalScore}";
        if (highScoreText) highScoreText.text = $"High Score:  {SceneLoader.HighScore}";
        if (finalLevelText) finalLevelText.text = $"Level Reached: {SceneLoader.FinalLevel}";

        if (restartButton) restartButton.onClick.AddListener(() => SceneManager.LoadScene(startSceneName));
        if (creditsButton) creditsButton.onClick.AddListener(() => SceneManager.LoadScene(creditsSceneName));

#if UNITY_WEBGL
        if (quitButton) quitButton.gameObject.SetActive(false);
#else
        if (quitButton)
        {
            quitButton.gameObject.SetActive(true);
            quitButton.onClick.AddListener(QuitApp);
        }
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) SceneManager.LoadScene(startSceneName);
        if (Input.GetKeyDown(KeyCode.C)) SceneManager.LoadScene(creditsSceneName);

#if !UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Escape)) QuitApp();
#endif
    }

    void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
