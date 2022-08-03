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

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetSceneByName("Pause").isLoaded)
            {
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
    }
}
