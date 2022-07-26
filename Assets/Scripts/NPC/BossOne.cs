using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static BossVars;

public class BossOne : Enemy
{
    /* Controls First Boss animations, projectile handling and health */

    public TextMeshProUGUI phasetxt;

    // Projectile Vars
    public Transform[] projectileSpawnPlacements;
    public GameObject horizontalProjectile;
    public GameObject missile;
    public GameObject missileSfx;
    public Transform missileCeiling;
    public Transform missileFloor;
    public int totalMissiles;
    public GameObject shotgun;
    public ShotgunBullet shotgunBulletVars;
    public Transform[] waypoints;
    public GameObject bulletSpawner;
    public GameObject bulletSpawnerThird;
    public float moveTime = 0;

    // Tween Vars
    public Color phaseOneColor;
    public Color phaseTwoColor;
    public Color phaseThreeColor;
    public Color missileColor;
    public Color shotgunColor;
    public float rotateTime;

    // Phase Vars
    [SerializeField] private float startingGraceTime = 5;
    [SerializeField] private float phaseOneGrace = 6;
    [SerializeField] private float phaseTwoGrace = 5;
    [SerializeField] private float phaseThreeGrace = 5;
    [SerializeField] private float deathGraceTime = 2;

    [SerializeField] private GameObject cam;

    private ParticleSystem ps;
    private int currentPhase = 0;
    private float t = 0;
    private float moveT = 0;
    private bool isMoving = false;
    private Vector3 waypoint;
    private Material material;

    [SerializeField] private AudioClip shotgunSfx;
    [SerializeField] private AudioClip spawnInSfx;
    [SerializeField] private AudioClip phaseChangeSfx;
    [SerializeField] private AudioClip deathSfx;



    // Start is called before the first frame update
    void Start()
    {
        maxHp = hp;
        ps = GetComponent<ParticleSystem>();
        ps.Stop();
        transform.localScale = Vector3.zero;
        material = GetComponent<SpriteRenderer>().material;
        material.EnableKeyword("_EMISSION");
        base.Init();
        phasetxt.text = string.Format("PHASE {0}", currentPhase);

    }

    // Update is called once per frame
    void Update()
    {
        material.SetColor("_EmissionColor", gameObject.GetComponent<SpriteRenderer>().color);
        //if (Keyboard.current[Key.Q].wasPressedThisFrame)
        //{
        //    hp -= 10;
        //    Debug.Log("HP: " + hp);
        //    UpdateHealthBar();
        //}
        // Stop React State when moving
        if (!isMoving)
        {
            t += Time.deltaTime;
            moveT += Time.deltaTime;
        }

        if (thePlayer == null)
        {
            t = -999;
            return;
        }
        

        /* Phase Zero
         * Spawn in animation
         */
        if (currentPhase == 0)
        {
            if (allowSelfHitbox && baseCollider.isActiveAndEnabled)
            {
                baseCollider.enabled = false;
            }

            Phase p = PhaseZero(t, startingGraceTime);
            if (p == Phase.TimeReset)
            {
                t = 0;
            }
        }

        phasetxt.text = string.Format("PHASE {0}", currentPhase);
        /* Phase One
         * Constant Spiral Attack
         * Projectile Attacks on centre
         * Missiles if player is above or under
         */
        if (currentPhase == 1)
        {
            if (allowSelfHitbox)
            {
                baseCollider.enabled = true;
            }
            Phase p = PhaseOne(t, phaseOneGrace);
            if (p == Phase.TimeReset)
            {
                t = 0;
            }
            else if (p == Phase.HPThreshold)
            {
                t = 0;
                rotateTime -= 1;
                currentPhase++;
                phasetxt.text = string.Format("PHASE {0}", currentPhase);
                LTSeq sq = LeanTween.sequence();
                sq.append(1.0f);
                sq.append(LeanTween.color(gameObject, phaseTwoColor, 2));
            }
        }

        /* Phase Two
         * Constant Spiral Attack
         * Missile threshold lower
         * Shotgun Attacks
         */
        if (currentPhase == 2)
        {

            Phase p = PhaseTwo(t, phaseTwoGrace);
            if (p == Phase.TimeReset)
            {
                t = 0;
            }
            else if (p == Phase.HPThreshold)
            {
                t = -1;
                rotateTime -= 1.2f;
                currentPhase++;
                phasetxt.text = string.Format("PHASE {0}", currentPhase);
                LTSeq sq = LeanTween.sequence();
                sq.append(1.0f);
                sq.append(LeanTween.color(gameObject, phaseThreeColor, 2));
            }
        }

        /* Phase Three
         * Constant Spiral Attack
         * Extra Fast Spiral Attack
         * Missile same threshold
         * Shotgun
         */
        if (currentPhase == 3)
        {
            if (!bulletSpawnerThird.activeSelf)
            {
                bulletSpawnerThird.SetActive(true);
            }
            
            Phase p = PhaseThree(t, phaseThreeGrace);
            if (p == Phase.TimeReset)
            {
                t = 0;
            }
            else if (p == Phase.HPThreshold)
            {
                t = 0;
                moveTime = 9999;
                currentPhase++;
                phasetxt.text = string.Format("PHASE {0}", currentPhase);
                CancelAllAttacks();
            }
        }

        /* Phase Four
         * Death
         * Plays animation then destroys
         */
        if (currentPhase == 4)
        {
            // Inefficient but this does stop coroutines that manages to spawn in before the previous cancels
            CancelAllAttacks();
            Phase p = PhaseFour(t, deathGraceTime);
            if (p == Phase.HPThreshold)
            {
                t = -9999;
                //gameManager.GetComponent<GameManager>().FinishFight();
            }
        }

        // Move if needed independent
        if (moveT > moveTime)
        {
            // Find furthest waypoint to player and move there
            float furthestPoint = 0;
            Vector3 bestPoint = Vector3.zero;
            for (int i = 0; i < waypoints.Length; i++)
            {
                float d = Vector3.Distance(thePlayer.transform.position, waypoints[i].position);
                if (d > furthestPoint)
                {
                    furthestPoint = d;
                    bestPoint = waypoints[i].position;
                }
            }
            if (bestPoint != Vector3.zero)
            {
                waypoint = bestPoint;
                isMoving = true;
                t = 0;
                moveT = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 v = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypoint) < 0.01f)
            {
                isMoving = false;
                moveT = 0;
            }
        }
    }

    private Phase PhaseZero(float t, float graceTime)
    {
        if (t > graceTime)
        {
            startingGraceTime = 9999;
            LeanTween.scale(gameObject, new Vector3(8, 8, 1), 5);
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);
            sq.append(() => ps.Play());
            sq.append(LeanTween.color(gameObject, Color.red, 1));
            sq.append(LeanTween.color(gameObject, Color.blue, 1));
            sq.append(LeanTween.color(gameObject, Color.red, 1));
            sq.append(LeanTween.color(gameObject, Color.blue, 1));
            sq.append(LeanTween.color(gameObject, phaseOneColor, 1));
            sq.append(() => BaseRotationTween());
            sq.append(() => ps.Emit(100));
            sq.append(() => ps.Stop());
            sq.append(() => cam.GetComponent<CameraShake>().ScreenShake(2.0f));
            sq.append(() => audioSource.PlayOneShot(spawnInSfx));


            sq.append(() => currentPhase++);
            return Phase.TimeReset;
        }
        return Phase.Nothing;
    }

    private Phase PhaseOne(float t, float graceTime)
    {
        if (hp / maxHp <= 0.75)
        {
            cam.GetComponent<CameraShake>().ScreenShake(2.0f);
            audioSource.PlayOneShot(phaseChangeSfx);
            return Phase.HPThreshold;
        }

        if (t > graceTime)
        {
            if (!bulletSpawner.activeSelf)
            {
                bulletSpawner.SetActive(true);
            }
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);
            sq.append(() => ProjectileAttack());
            
            if (thePlayer.transform.position.y <= missileFloor.transform.position.y || thePlayer.transform.position.y >= missileCeiling.transform.position.y)
            {
                sq.append(LeanTween.color(gameObject, missileColor, 1));
                sq.append(() => SpawnMissiles(totalMissiles, thePlayer.transform));
                sq.append(LeanTween.color(gameObject, phaseOneColor, 1));
            }

            return Phase.TimeReset;
        }


        return Phase.Nothing;
    }

    private Phase PhaseTwo(float t, float graceTime)
    {
        if (hp / maxHp <= 0.35f)
        {
            cam.GetComponent<CameraShake>().ScreenShake(2.0f);
            audioSource.PlayOneShot(phaseChangeSfx);
            return Phase.HPThreshold;
        }

        float heightCeiling = missileCeiling.transform.position.y - 1.0f;
        float heightFloor = missileFloor.transform.position.y + 1.0f;
        if (t > graceTime)
        {
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);

            if (thePlayer.transform.position.y <= heightFloor || thePlayer.transform.position.y >= heightCeiling)
            {
                sq.append(LeanTween.color(gameObject, missileColor, 1));
                sq.append(() => SpawnMissiles(totalMissiles, thePlayer.transform));
                sq.append(LeanTween.color(gameObject, phaseTwoColor, 1));
            }

            sq.append(LeanTween.color(gameObject, shotgunColor, 0.3f));
            sq.append(() => MultipleShotgun());
            sq.append(LeanTween.color(gameObject, phaseTwoColor, 1));

            return Phase.TimeReset;
        }
        return Phase.Nothing;
    }

    private Phase PhaseThree(float t, float graceTime)
    {
        if (hp / maxHp <= 0.0f)
        {
            cam.GetComponent<CameraShake>().ScreenShake(2.0f);
            audioSource.PlayOneShot(phaseChangeSfx);
            return Phase.HPThreshold;
        }

        float heightCeiling = missileCeiling.transform.position.y - 1.0f;
        float heightFloor = missileFloor.transform.position.y + 1.0f;
        if (t > graceTime)
        {
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);

            if (thePlayer.transform.position.y <= heightFloor || thePlayer.transform.position.y >= heightFloor)
            {
                sq.append(LeanTween.color(gameObject, missileColor, 1));
                sq.append(() => SpawnMissiles(5, thePlayer.transform));
                sq.append(LeanTween.color(gameObject, phaseThreeColor, 1));
            }

            sq.append(LeanTween.color(gameObject, shotgunColor, 0.3f));
            sq.append(() => MultipleShotgun());
            sq.append(LeanTween.color(gameObject, phaseThreeColor, 1));

            return Phase.TimeReset;
        }
        return Phase.Nothing;
    }

    private Phase PhaseFour(float t, float graceTime)
    {

        if (t > graceTime)
        {
            cam.GetComponent<CameraShake>().ScreenShake(10.0f);
            audioSource.PlayOneShot(deathSfx);
            LeanTween.cancel(gameObject);
            BaseRotationTween();
            LTSeq sq = LeanTween.sequence();
            Vector3 scale = gameObject.transform.localScale;
            Color low = phaseThreeColor * 0.5f;
            low.a = 1.0f;
            Color high = phaseThreeColor * -0.25f;
            high.a = 1.0f;
            sq.append(() => ps.Play());
            sq.append(0.5f);
            sq.append(LeanTween.scale(gameObject, scale * 0.5f, 1));
            sq.append(() => ps.Emit(100));
            sq.append(LeanTween.scale(gameObject, scale, 1));
            sq.append(LeanTween.scale(gameObject, scale * 0.3f, 1));
            sq.append(() => ps.Emit(100));
            sq.append(LeanTween.scale(gameObject, scale, 1));
            sq.append(LeanTween.scale(gameObject, scale * 0.1f, 1));
            sq.append(() => ps.Emit(100));
            sq.append(LeanTween.scale(gameObject, scale, 0));
            sq.append(() => ps.Emit(100));
            sq.append(() => ps.Stop());
            sq.append(() => Death());
            LTSeq sc = LeanTween.sequence();
            sc.append(LeanTween.color(gameObject, low, 0.5f));
            sc.append(LeanTween.color(gameObject, high, 0.5f));
            sc.append(LeanTween.color(gameObject, low, 0.5f));
            sc.append(LeanTween.color(gameObject, high, 0.5f));
            return Phase.HPThreshold;
        }
        return Phase.Nothing;
    }
    
    private void Death()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void ProjectileAttack()
    {
        Vector2 playerPos = thePlayer.transform.position;
        Vector2 myPosition = transform.position;

        if (playerPos.x < myPosition.x)
        {
            FullLeftProjectileAttack();
        }
        else if (myPosition.x < playerPos.x)
        {
            FullRightProjectileAttack();
        }
        else
        {
            FullLeftProjectileAttack();
            FullRightProjectileAttack();
        }
    }

    private void FullLeftProjectileAttack()
    {
        horizontalProjectile.GetComponent<Projectile>().startingVelocity = new Vector2(-250f, 0.0f);
        for (int i = 0; i < 6; i++)
        {
            GameObject go = SpawnProjectile(i, horizontalProjectile);
            go.SetActive(true);
        }
    }

    private void FullRightProjectileAttack()
    {
        horizontalProjectile.GetComponent<Projectile>().startingVelocity = new Vector2(250f, 0.0f);
        for (int i = 0; i < 6; i++)
        {
            GameObject go = SpawnProjectile(i, horizontalProjectile);
            go.SetActive(true);
        }
    }

    private GameObject SpawnProjectile(int projectileIndex, GameObject prefab)
    {
        GameObject go = Instantiate(prefab);
        go.transform.position = projectileSpawnPlacements[projectileIndex].position;
        return go;

    }

    private void SpawnMissiles(int total, Transform target)
    {
        missile.GetComponent<Missile>().target = thePlayer.transform;
        for (int i = 0; i < total; i++)
        {
            GameObject go = Instantiate(missile);
            go.transform.position = transform.position;
            go.SetActive(true);
        }
        Instantiate(missileSfx).GetComponent<MissileSfx>().timeToActivate = missile.GetComponent<Missile>().timeToActivate;
    }

    public void MultipleShotgun()
    {
        float angle = MathFuncs.AngleTowardsObject(thePlayer, transform);
        ShotgunBullet temp = shotgunBulletVars.Copy();
        temp.rotation = angle;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(temp, shotgun, transform));
        StartCoroutine(DelayedSfx(shotgunSfx, temp.spawnInterval, temp.repeats));
    }

    public void BaseRotationTween()
    {
        LeanTween.rotateAround(gameObject, Vector3.forward, 360.0f, rotateTime)
            .setRepeat(-1);
    }

    private void CancelAllProjectiles()
    {
        foreach (GameObject pj in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            if (pj.GetComponent<LifeTimer>() != null)
            {
                pj.GetComponent<LifeTimer>().Expire();
            }
            else Destroy(pj);
        }
    }
    private IEnumerator DelayedSfx(AudioClip clip, float delay, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(delay);
        }
        
    }

    private void CancelAllAttacks()
    {
        if (bulletSpawner.activeSelf || bulletSpawnerThird.activeSelf || baseCollider.enabled)
        {
            bulletSpawner.SetActive(false);
            bulletSpawnerThird.SetActive(false);
            baseCollider.enabled = false;
        }
        StopAllCoroutines();
        CancelAllProjectiles();
    }
}