using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TestSwapScene : MonoBehaviour
{
    public string[] scenes;
    private int currScene = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Scene");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current[Key.RightArrow].wasPressedThisFrame)
        {
            currScene++;
            if (currScene == scenes.Length)
            {
                currScene = 0;
            }
            SceneManager.LoadScene(scenes[currScene]);

        }
    }
}
