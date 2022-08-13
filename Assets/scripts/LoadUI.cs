using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadUI : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
    }

    //TODO -- NEED TO UPDATE TO NEW INPUT METHOD WITH INPUT MANAGER
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetSceneByName("Pause").isLoaded)
            {
                Time.timeScale = 1;
                SceneManager.UnloadSceneAsync("Pause");
                SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            }
            else
            {
                Time.timeScale = 0;
                SceneManager.UnloadSceneAsync("UI");
                SceneManager.LoadSceneAsync("Pause", LoadSceneMode.Additive);
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //SceneManager.LoadScene("Scene 2");
        }
    }
}
