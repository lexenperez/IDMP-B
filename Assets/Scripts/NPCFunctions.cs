using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunctions : MonoBehaviour
{

    // General functions for NPCS 

    // Creates a projectile at a given location
    public void CreateObject(GameObject obj, Vector2 position)
    {

        Instantiate(obj, position, Quaternion.identity);
    }
}
