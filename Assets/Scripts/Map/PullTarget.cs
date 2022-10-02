using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullTarget : MonoBehaviour
{
    [SerializeField] private string[] tagsToPull;
    [SerializeField] private float pullForce;
    [SerializeField] private float distanceThreshold;

    private GameObject[] objsToPull;
    // Start is called before the first frame update
    void Start()
    {
        objsToPull = GetObjectsToPull();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject obj in objsToPull)
        {
            if (obj)
            {
                if (Vector3.Distance(obj.transform.position, transform.position) <= distanceThreshold)
                {
                    obj.transform.position = Vector2.MoveTowards(obj.transform.position, transform.position, Time.deltaTime * pullForce);
                }
            }
        }
    }

    public GameObject[] GetObjectsToPull()
    {
        List<GameObject> allObjs = new List<GameObject>();
        foreach (string tag in tagsToPull)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            allObjs.AddRange(objs);
        }
        return allObjs.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceThreshold);
    }
}
