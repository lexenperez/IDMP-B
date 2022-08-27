using UnityEngine;
using UnityEngine.InputSystem;

public class BossOne : Enemy
{

    // Individual scripts should contain its state machine and handle all calls for activating hitboxes, animations and spawning
    // For this boss, it doesnt move
    // Idle - Boss stands still
    // Attack - Do Randomised pattern of attacks (figure out patterns later)
    // Move - Boss moves to a certain location (animation for walking or teleporting)
    // Death - Boss dies (animation trigger)

    // Patterns should be a function that calls the attacks in order
    // Check how to check whether the duration for the previous attack is finished
    // https://answers.unity.com/questions/362629/how-can-i-check-if-an-animation-is-being-played-or.html
    // One way while in that pattern state, refresh in update whether we can do next attack

    // Call Animations here

    // Store projectiles in case need to change values when spawning them

    // Maybe seperate this into classes for easier serialisation
    public GameObject horizontalProjectile;
    public GameObject missile;
    public int totalMissiles;
    public GameObject bullet;
    public GameObject bulletLog;
    public GameObject shotgun;

    public CircleBullet circleBulletVars;
    public SpiralBullet spiralBulletVars;
    public ShotgunBullet shotgunBulletVars;
    public Transform[] waypoints;

    public float lowerReactTime;
    public float higherReactTime;
    public float playerDistanceThreshold;
    public float moveTime = 0;
    public float projectileChance = 0.25f;
    public float missileChance = 0.15f;

    public Color phaseOneColor;
    public Color phaseTwoColor;
    public Color phaseThreeColor;
    public Color missileColor;
    public Color shotgunColor;

    public float tweenTime;
    private int currentPhase = 0;

    // Phase One Modifiers
    public GameObject bulletSpawner;
    //public CircleBullet PhaseOneCircleVariables;

    public GameObject bulletSpawnerThird;

    //public GameObject gameManager;

    private float reactTime = 5;
    private float t = 0;
    private float moveT = 0;
    //private int repeats = 0;
    //private bool starth = true;
    private bool isMoving = false;
    private Vector3 waypoint;
    private Material material;

    enum Phase
    {
        TimeReset,
        HPThreshold,
        Nothing
    }


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
        material = GetComponent<SpriteRenderer>().material;
        material.EnableKeyword("_EMISSION");
        
        base.Init();
        //totalHP = this.hp;
        //Tween();

        // Probs set values of projectiles here
    }

    public void Tween()
    {
        //LeanTween.cancel(gameObject);
        LeanTween.rotateAround(gameObject, Vector3.forward, 360.0f, tweenTime)
            .setRepeat(-1);
    }
    // Update is called once per frame
    void Update()
    {
        material.SetColor("_EmissionColor", gameObject.GetComponent<SpriteRenderer>().color);
        if (Keyboard.current[Key.Q].wasPressedThisFrame)
        {
            hp -= 10;
            Debug.Log("HP: "+ hp);
            UpdateHealthBar();
        }
        // Stop React State when moving
        if (!isMoving)
        {
            t += Time.deltaTime;
            moveT += Time.deltaTime;
        }

        /* Phase Zero
         * Spawn in animation
         */
        if (currentPhase == 0)
        {
            Phase p = PhaseZero(t);

            if (p == Phase.TimeReset)
            {
                t = 0;
                currentPhase++;
                //gameManager.GetComponent<GameManager>().StartCurrentTime();
            }
        }

        /* Phase One
         * Constant Spiral Attack
         * Projectile Attacks on centre
         * Missiles if player is above or under
         */
        if (currentPhase == 1)
        {
            Phase p = PhaseOne(t);
            if (p == Phase.TimeReset)
            {
                t = 0;
            }
            else if (p == Phase.HPThreshold)
            {
                t = -999;
                tweenTime -= 1;
                currentPhase++;
                LTSeq sq = LeanTween.sequence();
                sq.append(1.0f);
                sq.append(LeanTween.color(gameObject, phaseTwoColor, 2));
                sq.append(() =>
                {
                    t = 0;
                });
            }
        }

        /* Phase Two
         * Constant Spiral Attack
         * Missile threshold lower
         * Shotgun Attacks
         */
        if (currentPhase == 2)
        {

            Phase p = PhaseTwo(t);
            if (p == Phase.TimeReset)
            {
                t = 0;
            }
            else if (p == Phase.HPThreshold)
            {
                t = -999;
                currentPhase++;
                LTSeq sq = LeanTween.sequence();
                sq.append(1.0f);
                sq.append(LeanTween.color(gameObject, phaseThreeColor, 2));
                sq.append(() =>
                {
                    t = 0;
                    bulletSpawnerThird.SetActive(true);
                });
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

            Phase p = PhaseThree(t);
            if (p == Phase.TimeReset)
            {
                t = 0;
            }
            else if (p == Phase.HPThreshold)
            {
                bulletSpawner.SetActive(false);
                bulletSpawnerThird.SetActive(false);
                t = 0;
                currentPhase++;
            }
        }

        /* Phase Four
         * Death
         * Plays animation then destroys
         */
        if (currentPhase == 4)
        {
            Phase p = PhaseFour(t);
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

    private Phase PhaseZero(float t)
    {
        if (t > 5)
        {
            LeanTween.scale(gameObject, new Vector3(8, 8, 1), 5);
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);
            sq.append(LeanTween.color(gameObject, Color.red, 1));
            sq.append(LeanTween.color(gameObject, Color.blue, 1));
            sq.append(LeanTween.color(gameObject, Color.red, 1));
            sq.append(LeanTween.color(gameObject, Color.blue, 1));
            sq.append(LeanTween.color(gameObject, phaseOneColor, 1));
            sq.append(() => Tween());
            return Phase.TimeReset;
        }
        return Phase.Nothing;
    }

    private Phase PhaseOne(float t)
    {

        float heightThreshold = 7.0f;
        if (hp / maxHp <= 0.75)
        {
            bulletSpawner.SetActive(false);
            return Phase.HPThreshold;
        }

        if (t > 6)
        {
            if (!bulletSpawner.activeSelf)
            {
                bulletSpawner.SetActive(true);
            }
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);
            sq.append(() => ProjectileAttack());
            
            if (thePlayer.transform.position.y <= -heightThreshold || thePlayer.transform.position.y >= heightThreshold)
            {
                sq.append(LeanTween.color(gameObject, missileColor, 1));
                sq.append(() => SpawnMissiles(totalMissiles, thePlayer.transform));
                sq.append(LeanTween.color(gameObject, phaseOneColor, 1));
            }

            return Phase.TimeReset;
        }


        return Phase.Nothing;
    }

    private Phase PhaseTwo(float t)
    {
        if (hp / maxHp <= 0.5f)
        {
            return Phase.HPThreshold;
        }

        float heightThreshold = 6.0f;
        if (t > 5)
        {
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);

            if (thePlayer.transform.position.y <= -heightThreshold || thePlayer.transform.position.y >= heightThreshold)
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

    private Phase PhaseThree(float t)
    {
        if (hp / maxHp <= 0.0f)
        {
            return Phase.HPThreshold;
        }

        float heightThreshold = 6.0f;
        if (t > 5)
        {
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);

            if (thePlayer.transform.position.y <= -heightThreshold || thePlayer.transform.position.y >= heightThreshold)
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

    private Phase PhaseFour(float t)
    {

        if (t > 2)
        {
            LeanTween.cancel(gameObject);
            Tween();
            LTSeq sq = LeanTween.sequence();
            Vector3 scale = gameObject.transform.localScale;
            Color low = phaseThreeColor * 0.25f;
            low.a = 1.0f;
            Color high = phaseThreeColor * -0.25f;
            high.a = 1.0f;
            sq.append(0.5f);
            sq.append(LeanTween.scale(gameObject, scale * 0.5f, 1));
            sq.append(LeanTween.scale(gameObject, scale, 1));
            sq.append(LeanTween.scale(gameObject, scale * 0.3f, 1));
            sq.append(LeanTween.scale(gameObject, scale, 1));
            sq.append(LeanTween.scale(gameObject, scale * 0.1f, 1));
            sq.append(LeanTween.scale(gameObject, scale, 0));
            sq.append(() => Destroy(gameObject));
            LTSeq sc = LeanTween.sequence();
            sc.append(LeanTween.color(gameObject, low, 0.5f));
            sc.append(LeanTween.color(gameObject, high, 0.5f));
            sc.append(LeanTween.color(gameObject, low, 0.5f));
            sc.append(LeanTween.color(gameObject, high, 0.5f));
            return Phase.HPThreshold;
        }
        return Phase.Nothing;
    }

    private void MeleeAttack(string trigger)
    {
        animator.SetTrigger(trigger);
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

    // Probs do similar to projectiles and find certain points to spawn at, or just spawn at parent center
    private void SpawnMissiles(int total, Transform target)
    {
        missile.GetComponent<Missile>().target = thePlayer.transform;
        for (int i = 0; i < total; i++)
        {
            GameObject go = Instantiate(missile);
            go.transform.position = transform.position;
            go.SetActive(true);
        }
    }

    // Melee Attack up down alternatively
    private int AlternateMeleeAttack(int repeats, bool startHigh)
    {
        if (startHigh)
        {
            MeleeAttack(BossOneConstants.highAttack);
        }
        else
        {
            MeleeAttack(BossOneConstants.lowAttack);
        }
        return repeats - 1;
    }

    private void FireMissiles()
    {
        SpawnMissiles(totalMissiles, thePlayer.transform);
    }

    // EDIT THESE TO FIT GAMEPLAY
    // P1
    public void SpiralWithShotgun()
    {
        ShotgunBullet temp = shotgunBulletVars.Copy();
        temp.repeats = 3;
        temp.spawnInterval = 3.0f;
        temp.rotation = AngleTowardsPlayer();
        temp.totalLeft = 5;
        temp.totalRight = 5;
        temp.distanceBetweenBullets = 15;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(temp, shotgun, transform));
        StartCoroutine(BulletHellFuncs.CircularBullet(circleBulletVars, bullet, transform));
        LeanTween.color(gameObject, Color.red, 2.0f);

    }

    // P2
    public void TwoSpirals()
    {
        StartCoroutine(BulletHellFuncs.CircularBullet(circleBulletVars, bullet, transform));
        StartCoroutine(BulletHellFuncs.CircularBullet(circleBulletVars, bullet, transform));
        LeanTween.color(gameObject, Color.blue, 2.0f);
    }

    // P3
    public void MultipleShotgun()
    {
        float angle = AngleTowardsPlayer();
        ShotgunBullet temp = shotgunBulletVars.Copy();
        temp.rotation = angle;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(temp, shotgun, transform));
        //ShotgunBullet temp1 = shotgunBulletVars.Copy();
        //temp1.rotation = angle - 25.0f;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(temp1, shotgun, transform));
        ////shb.rotation = temp - 50.0f;
        ////StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
        //ShotgunBullet temp2 = shotgunBulletVars.Copy();
        //temp2.rotation = angle + 25.0f;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(temp2, shotgun, transform));
        //shb.rotation = temp + 50;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
    }

    private float AngleTowardsPlayer()
    {
        Vector3 dir = (thePlayer.transform.position);
        return Mathf.Atan2(dir.y - transform.position.y, dir.x - transform.position.x) * Mathf.Rad2Deg;
    }
}
public class BossOneConstants
{
    public const string highAttack = "HighAttack";
    public const string lowAttack = "LowAttack";
    public const int totalBossPatterns = 3;
}