using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossVars;

public class BossThree : Enemy
{
    [Header("Phase One Variables")]
    [SerializeField] private GameObject spawnTrigger;
    [SerializeField] private GameObject phaseOneTrigger;
    [SerializeField] private Transform phaseOnePosition;
    [SerializeField] private float bulletSpawnTimeChange;
    [SerializeField] private float cannonballTime;
    [SerializeField] private GameObject[] cannonballs;
    [SerializeField] private GameObject bulletSpawner;
    [SerializeField] private GameObject spikes;
    [SerializeField] private GameObject platforms;
    [SerializeField] private GameObject teleport;
    [SerializeField] private GameObject turretSpawner;
    private bool cannonballsOn = false;


    
    private List<int> ballIndices;
    private float t;
    private float currentPhase = 0;
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
        teleport.SetActive(false);
        turretSpawner.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        Phase p = Phase.Nothing;

        switch(currentPhase)
        {
            // Spawn in / Turret Platform
            case 0:
                p = PhaseZero(t);
                break;
            // First fight, platforms and turrets
            case 1:
                p = PhaseOne(t);
                break;
            // Platforming Section
            case 2:
                p = PhaseTwo(t);
                break;
            // Final fight
            case 3:
                p = PhaseThree(t);
                break;
            // Death
            case 4:
                p = PhaseFour(t);
                break;
            default:
                break;
        }

        if (p == Phase.TimeReset)
        {
            t = 0;
        }
        else if (p == Phase.HPThreshold)
        {
            currentPhase++;
        }

    }

    Phase PhaseZero(float t)
    {
        // Start trigger
        if (TriggerHit(spawnTrigger, thePlayer))
        {
            transform.LeanMove(phaseOnePosition.position, 7);
            spawnTrigger.SetActive(false);
        }

        // Start Phase One fight
        if (TriggerHit(phaseOneTrigger, thePlayer))
        {
            phaseOneTrigger.SetActive(false);
            bulletSpawner.SetActive(true);
            turretSpawner.SetActive(true);
            currentPhase++;
            
        }
        return Phase.Nothing;
    }

    Phase PhaseOne(float t)
    {
        // Change bullet angle
        if (t > bulletSpawnTimeChange)
        {
            if (bulletSpawner.activeSelf)
            {
                bulletSpawner.GetComponent<BulletSpawner>().vars.rotation += 15.0f;
            }
            if (!cannonballsOn)
            {
                StartCoroutine(PhaseOneCannonballs(cannonballTime));
                cannonballsOn = true;
            }
            return Phase.TimeReset;
        }

        if (hp / maxHp <= 0.75f)
        {

            teleport.SetActive(true);
            spikes.SetActive(false);
            platforms.SetActive(false);
            bulletSpawner.SetActive(false);
            turretSpawner.SetActive(false);
            StopCoroutine(PhaseOneCannonballs(0));
            return Phase.HPThreshold;
        }

        return Phase.Nothing;
    }

    Phase PhaseTwo(float t)
    {
        return Phase.Nothing;
    }

    Phase PhaseThree(float t)
    {
        return Phase.Nothing;
    }

    Phase PhaseFour(float t)
    {
        return Phase.Nothing;
    }


    private bool TriggerHit(GameObject trigger, GameObject triggeree)
    {
        return trigger.GetComponent<Collider2D>().IsTouching(triggeree.GetComponent<Collider2D>());
    }

    private IEnumerator PhaseOneCannonballs(float wait)
    {
        while(true)
        {
            SpawnCannonballs();
            yield return new WaitForSeconds(wait);
        }
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
