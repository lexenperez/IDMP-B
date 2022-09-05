using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private string mainMenuScene;

    public void Restart()
    {
        // TODO: Restart Game
        Debug.Log("TODO - Restart Game");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
