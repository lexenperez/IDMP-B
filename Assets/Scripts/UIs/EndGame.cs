using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private string mainMenuScene;

    public void Restart()
    {
        SceneManager.LoadScene(GameManager.currScene.name);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
