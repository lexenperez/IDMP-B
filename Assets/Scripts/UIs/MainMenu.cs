using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject records;
    [SerializeField] private GameObject recordTxt;
    [SerializeField] private string gameScene;
    [SerializeField] private GameObject gameManager;
    private string gmTag;
    private void Start()
    {
        // Setup tag at the start since you need a GM
        gmTag = gameManager.tag;
    }

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
        GameObject gm = GameObject.FindGameObjectWithTag(gmTag);
        if (gm)
        {
            gm.GetComponent<GameManager>().recordsObj = recordTxt;
            gm.GetComponent<GameManager>().SetRecords();
        }
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
