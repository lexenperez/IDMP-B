using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailUpdater : MonoBehaviour
{

    [SerializeField] TrailRenderer trilRenderer;
    Vector3 previousPos;

    private void Start()
    {
        previousPos = trilRenderer.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if ((previousPos - trilRenderer.transform.position).magnitude > 2)
        {
            trilRenderer.Clear();
        }
        previousPos = trilRenderer.transform.position;
    }
}
