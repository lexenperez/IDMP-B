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
            SceneManager.LoadScene("Scene 2");
        }
    }
}
