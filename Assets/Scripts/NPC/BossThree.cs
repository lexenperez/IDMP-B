using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossVars;

public class BossThree : Enemy
{
    [SerializeField] private GameObject spawnTrigger;
    [SerializeField] private GameObject phaseOneTrigger;
    [SerializeField] private Transform phaseOnePosition;


    [SerializeField] private float bulletSpawnTimeChange;
    [SerializeField] private GameObject[] cannonballs;
    [SerializeField] private GameObject bulletSpawner;

    private List<int> ballIndices;
    private float t;
    // Start is called before the first frame update
    void Start()
    {
        //bs = GetComponent<BulletSpawner>();
        base.Init();
        ballIndices = new List<int>();
        for (int i = 0; i < cannonballs.Length; i++)
        {
            cannonballs[i].SetActive(false);
            ballIndices.Add(i);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (TriggerHit(spawnTrigger, thePlayer))
        {
            transform.LeanMove(phaseOnePosition.position, 7);
            spawnTrigger.SetActive(false);
        }

        if (TriggerHit(phaseOneTrigger, thePlayer))
        {
            phaseOneTrigger.SetActive(false);
            bulletSpawner.SetActive(true);
        }

        if (t > bulletSpawnTimeChange)
        {
            if (bulletSpawner.activeSelf)
            {
                bulletSpawner.GetComponent<BulletSpawner>().vars.rotation += 15.0f;
                SpawnCannonballs();
            }


            t = 0;
        }
    }


    private bool TriggerHit(GameObject trigger, GameObject triggeree)
    {
        return trigger.GetComponent<Collider2D>().IsTouching(triggeree.GetComponent<Collider2D>());
    }

    private void SpawnCannonballs()
    {
        List<int> indices = new List<int>(ballIndices);
        // Randomly pick 2
        for (int i = 0; i < indices.Count; i++)
        {
            int index = Random.Range(0, indices.Count);
            Instantiate(cannonballs[ballIndices[index]], transform.position, Quaternion.identity, null).SetActive(true);
            indices.Remove(index);
        }
    }

}
