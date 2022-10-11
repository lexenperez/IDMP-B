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
    [SerializeField] private int boss1;
    [SerializeField] private int boss2;
    [SerializeField] private int boss3;
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

    public void StartBoss1()
    {
        
        SceneManager.LoadScene(boss1);
    }    
    
    public void StartBoss2()
    {
        
        SceneManager.LoadScene(boss2);
    }

    public void StartBoss3()
    {
        SceneManager.LoadScene(boss3);
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
