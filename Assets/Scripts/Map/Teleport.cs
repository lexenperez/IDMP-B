using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{

    [SerializeField] private Transform teleporter;
    [SerializeField] private string[] teleportees;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tags in teleportees)
        {
            if (collision.CompareTag(tags))
            {
                collision.transform.position = teleporter.position;
                // This is dumb
                if (GetComponent<MapPhaseChange>())
                {
                    GetComponent<MapPhaseChange>().ChangePhase();
                }
            }
        }
    }
}
