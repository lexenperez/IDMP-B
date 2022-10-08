using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [SerializeField] private GameObject mainPauseMenuUI;
    [SerializeField] private GameObject subPauseMenuUI;
    [SerializeField] private GameObject optionsMenuUI;

    [SerializeField] private string mainMenuScene;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.gameEnded)
        {
            //if (inControlsMenu)
            //    GoBack();
            //else
            //{
            if (!optionsMenuUI.activeSelf)
                if (gameIsPaused)
                    Resume();
                else
                    Pause();
            //}
        }
    }

    public void Resume()
    {
        mainPauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    private void Pause()
    {
        mainPauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        gameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void ViewOptions()
    {
        optionsMenuUI.SetActive(true);
        subPauseMenuUI.SetActive(false);
    }

    public void GoBack()
    {
        optionsMenuUI.SetActive(false);
        subPauseMenuUI.SetActive(true);
    }
}
