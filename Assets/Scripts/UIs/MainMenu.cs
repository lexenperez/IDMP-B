using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject records;
    [SerializeField] private string gameScene;
    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void ViewRecords()
    {
        records.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ViewControls()
    {
        controls.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void GoBack()
    {
        if (controls.activeSelf)
            controls.SetActive(false);

        else if (records.activeSelf)
            records.SetActive(false);

        mainMenu.SetActive(true);
    }
}
