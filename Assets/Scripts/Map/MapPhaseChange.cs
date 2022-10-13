using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPhaseChange : MonoBehaviour
{
    [SerializeField] private int[] index;
    [SerializeField] private bool[] disable;
    [SerializeField] private MapManager manager;

    // Start is called before the first frame update
    void Start()
    {
        if (index.Length != disable.Length)
        {
            Debug.Log("Map Phase Change Index and Disable not same length in " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePhase()
    {
        for (int i = 0; i < index.Length; i++)
        {
            if (disable[i])
            {
                manager.DisablePhase(index[i]);
            }
            else
            {
                manager.EnablePhase(index[i]);
            }
        }

    }
}
