using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFuncs : MonoBehaviour
{
    // Common helper functions
    public static void DestroyObjectsOfTag(string tag)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag))
        {
            Destroy(obj);
        }

    }

    public static bool TriggerHit2D(GameObject trigger, GameObject triggeree)
    {
        if (trigger.GetComponent<Collider2D>() && trigger.GetComponent<Collider2D>())
        {
            return trigger.GetComponent<Collider2D>().IsTouching(triggeree.GetComponent<Collider2D>());
        }

        else return false;
       
    }

}
