using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] phases;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisablePhase(int index)
    {
        if (index < phases.Length)
        {
            phases[index].SetActive(false);
        }
    }

    public void EnablePhase(int index)
    {
        if (index < phases.Length)
        {
            phases[index].SetActive(true);
        }
    }
}
