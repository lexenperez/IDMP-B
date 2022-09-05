using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    private bool inControlsMenu = false;

    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject controlsMenuUI;

    [SerializeField] private string mainMenuScene;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    private void Pause()
    {
        pauseMenuCanvas.SetActive(true);
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
        pauseMenuUI.SetActive(false);
    }

    public void GoBack()
    {
        inControlsMenu = false;
        controlsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}
