using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{
    [SerializeField] private GameObject turret; 
    public float spawnTime;
    public float spawnInterval;
    public Transform[] positions;
    public float radius;
    public float speed;
    public float spawnRadius;
    [SerializeField] private LayerMask turretMask;
    private bool currentlySpawning = false;
    private float t = 0;
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (currentlySpawning)
        {
            // Spawn using interval time then start at 0 again
            if (t >= spawnInterval)
            {
                if (!Physics2D.OverlapCircle(positions[index].position, 1.0f, turretMask))
                {
                    
                    float x = Random.Range(0f, 360f);
                    float y = Random.Range(0f, 360f);
                    Vector3 position = MathFuncs.GetVectorOnCircle(x, y, spawnRadius);
                    GameObject tr = Instantiate(turret, transform.position + position, Quaternion.identity, null);
                    tr.AddComponent<MoveTowards>().positionToGo = positions[index].position;
                    tr.GetComponent<MoveTowards>().speed = speed;
                }
                index++;
                t = 0;
            }
            if (index >= positions.Length)
            {
                index = 0;
                currentlySpawning = false;
            }
        }
        // Check time for spawn time
        else if (t >= spawnTime)
        {
            currentlySpawning = true;
            t = 0;
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        for(int i = 0;i < positions.Length;i++)
        {
            Gizmos.DrawWireSphere(positions[i].position, 1);
        }
    }
}
