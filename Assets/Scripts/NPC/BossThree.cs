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

    [Header("Phase Two Variables")]
    [SerializeField] private Transform thirdTeleportLocation;
    [SerializeField] private LayerMask playerMask;

    [Header("Phase Three Variables")]
    [SerializeField] private Transform thirdStartLocation;
    [SerializeField] private Transform thirdEndLocation;
    [SerializeField] private GameObject thirdTurretSpawner;
    [SerializeField] private GameObject[] thirdCannonballs;
    [SerializeField] private GameObject[] thirdLavas;
    [SerializeField] private float cannonballSpawnTime;
    [SerializeField] private float lavaTime;
    [SerializeField] private float thirdStartTime;
    [SerializeField] private GameObject thirdBulletSpawnerOne;
    [SerializeField] private GameObject thirdBulletSpawnerTwo;
    [SerializeField] private float bulletRotationMod;
    private float thirdTotalTime = 0;
    private bool thirdCannonballsOn = false;
    private bool startPhaseThree = false;
    private bool initialMove = false;
    private float speedToEnd = 5;


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
        for(int i = 0; i < thirdCannonballs.Length; i++)
        {
            thirdCannonballs[i].SetActive(false);
        }
        thirdTurretSpawner.SetActive(false);


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
        if (HelperFuncs.TriggerHit2D(spawnTrigger, thePlayer))
        {
            transform.LeanMove(phaseOnePosition.position, 7);
            spawnTrigger.SetActive(false);
        }

        // Start Phase One fight
        if (HelperFuncs.TriggerHit2D(phaseOneTrigger, thePlayer))
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
            // Turn on cannonball spawning
            if (!cannonballsOn)
            {
                cannonballsOn = true;
                StartCoroutine(PhaseOneCannonballs(cannonballTime));
            }
            return Phase.TimeReset;
        }

        if (hp / maxHp <= 0.75f)
        {
            cannonballsOn = false;
            teleport.SetActive(true);
            spikes.SetActive(false);
            platforms.SetActive(false);
            bulletSpawner.SetActive(false);
            turretSpawner.SetActive(false);
            StopCoroutine(PhaseOneCannonballs(cannonballTime));
            HelperFuncs.DestroyObjectsOfTag(cannonballs[0].tag);
            HelperFuncs.DestroyObjectsOfTag(bulletSpawner.GetComponent<BulletSpawner>().bullet.tag);
            return Phase.HPThreshold;
        }

        return Phase.Nothing;
    }

    Phase PhaseTwo(float t)
    {
        // In this case just check if the player teleported to the final area
        if (Physics2D.OverlapCircle(thirdTeleportLocation.position, 3.0f, playerMask))
        {
            transform.position = thirdStartLocation.position;
            return Phase.HPThreshold;
        }
        
        return Phase.Nothing;
    }

    Phase PhaseThree(float t)
    {
        if (!initialMove)
        {
            LTSeq sq = LeanTween.sequence();
            sq.append(LeanTween.move(gameObject, thirdEndLocation.position, speedToEnd));

            sq.append(speedToEnd);
            sq.append(() => startPhaseThree = true);
            sq.append(() =>
            {
                thirdTurretSpawner.SetActive(true);
                thirdBulletSpawnerOne.SetActive(true);
                //thirdBulletSpawnerTwo.SetActive(true);
            });
            thirdTotalTime = thirdStartTime;

            initialMove = true;
        }

        if (startPhaseThree && t >= thirdTotalTime)
        {
            // Cannonballs on even/odds
            // Timer for lava movement
            // Spawn turrets
            LTSeq sq = LeanTween.sequence();
            sq.append(cannonballSpawnTime);
            sq.append(() => PhaseThreeCannonballs());
            sq.append(() => PhaseThreeSwapBulletHell());
            //sq.append(lavaTime);
            //sq.append(() => PhaseThreeLava());
            thirdTotalTime = thirdStartTime + cannonballSpawnTime + lavaTime;
            return Phase.TimeReset;

        }

        if (hp / maxHp <= 0.0f)
        {
            thirdTurretSpawner.SetActive(true);
            thirdBulletSpawnerOne.SetActive(false);
            thirdBulletSpawnerTwo.SetActive(false);
            foreach(GameObject lava in thirdLavas)
            {
                lava.SetActive(false);
            }
            HelperFuncs.DestroyObjectsOfTag(cannonballs[0].tag);
            HelperFuncs.DestroyObjectsOfTag(bulletSpawner.GetComponent<BulletSpawner>().bullet.tag);
            return Phase.HPThreshold;
        }

        return Phase.Nothing;
    }

    Phase PhaseFour(float t)
    {
        //cam.GetComponent<CameraShake>().ScreenShake(10.0f);
        //audioSource.PlayOneShot(deathSfx);
        //LeanTween.cancel(gameObject);
        //BaseRotationTween();
        //LTSeq sq = LeanTween.sequence();
        //Vector3 scale = gameObject.transform.localScale;
        //Color low = phaseThreeColor * 0.5f;
        //low.a = 1.0f;
        //Color high = phaseThreeColor * -0.25f;
        //high.a = 1.0f;
        //sq.append(() => ps.Play());
        //sq.append(0.5f);
        //sq.append(LeanTween.scale(gameObject, scale * 0.5f, 1));
        //sq.append(() => ps.Emit(100));
        //sq.append(LeanTween.scale(gameObject, scale, 1));
        //sq.append(LeanTween.scale(gameObject, scale * 0.3f, 1));
        //sq.append(() => ps.Emit(100));
        //sq.append(LeanTween.scale(gameObject, scale, 1));
        //sq.append(LeanTween.scale(gameObject, scale * 0.1f, 1));
        //sq.append(() => ps.Emit(100));
        //sq.append(LeanTween.scale(gameObject, scale, 0));
        //sq.append(() => ps.Emit(100));
        //sq.append(() => ps.Stop());
        //sq.append(() => Death());
        //LTSeq sc = LeanTween.sequence();
        //sc.append(LeanTween.color(gameObject, low, 0.5f));
        //sc.append(LeanTween.color(gameObject, high, 0.5f));
        //sc.append(LeanTween.color(gameObject, low, 0.5f));
        //sc.append(LeanTween.color(gameObject, high, 0.5f));
        //return Phase.HPThreshold;
        Death();
        return Phase.Nothing;
    }

    private void Death()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }


    private void PhaseThreeCannonballs()
    {
        int index = Random.Range(0, thirdCannonballs.Length);
        Instantiate(thirdCannonballs[ballIndices[index]], thirdCannonballs[ballIndices[index]].transform.position, Quaternion.identity, null).SetActive(true);
    }

    private void PhaseThreeSwapBulletHell()
    {
        thirdBulletSpawnerOne.GetComponent<BulletSpawner>().modRotation(bulletRotationMod);
        thirdBulletSpawnerTwo.GetComponent<BulletSpawner>().modRotation(bulletRotationMod);
        
        if (thirdBulletSpawnerOne.activeSelf)
        {
            thirdBulletSpawnerOne.SetActive(false);
            thirdBulletSpawnerTwo.SetActive(true);
        }
        else
        {
            thirdBulletSpawnerOne.SetActive(true);
            thirdBulletSpawnerTwo.SetActive(false);
        }
    }

    private void PhaseThreeLava()
    {
        int index = Random.Range(0, thirdLavas.Length);
        RisingTrap trap = thirdLavas[index].GetComponent<RisingTrap>();
        trap.RefreshParameters();
        trap.ResumeTween();
        // TODO find when lava has returned and x time has passed when that is finished
    }

    private IEnumerator PhaseOneCannonballs(float wait)
    {
        while(cannonballsOn)
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
