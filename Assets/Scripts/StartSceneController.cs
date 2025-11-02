using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartSceneController : MonoBehaviour
{
    void Start()
    {

    }

    public void OnStartClicked() => SceneManager.LoadScene("GameScene");
}
