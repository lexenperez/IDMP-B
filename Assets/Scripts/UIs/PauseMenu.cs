using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    private bool inControlsMenu = false;

    [SerializeField] private GameObject mainPauseMenuUI;
    [SerializeField] private GameObject subPauseMenuUI;
    [SerializeField] private GameObject controlsMenuUI;

    [SerializeField] private string mainMenuScene;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.gameEnded)
        {
            if (inControlsMenu)
                GoBack();
            else
            {
                if (gameIsPaused)
                    Resume();
                else
                    Pause();
            }
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
    }

    public void ViewControls()
    {
        inControlsMenu = true;
        controlsMenuUI.SetActive(true);
        subPauseMenuUI.SetActive(false);
    }

    public void GoBack()
    {
        inControlsMenu = false;
        controlsMenuUI.SetActive(false);
        subPauseMenuUI.SetActive(true);
    }
}
