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
    public GameObject bullet;
    public GameObject bulletLog;
    public GameObject shotgun;

    public CircleBullet circleBulletVars;
    public SpiralBullet spiralBulletVars;
    public ShotgunBullet shotgunBulletVars;
    public int totalMissiles;
    public Transform[] waypoints;

    public float lowerReactTime;
    public float higherReactTime;
    public float playerDistanceThreshold;
    public float moveTime = 0;
    public float projectileChance = 0.25f;
    public float missileChance = 0.15f;

    public float tweenTime;
    private int currentPhase = 0;

    // Phase One Modifiers
    public float projectileInterval;
    public GameObject bulletSpawner;
    //public CircleBullet PhaseOneCircleVariables;

    private float reactTime = 5;
    private float t = 0;
    private float moveT = 0;
    //private int repeats = 0;
    //private bool starth = true;
    private bool isMoving = false;
    private Vector3 waypoint;
    private Color bossColor = new Color(0.7051759f, 0.5385814f, 0.8584906f);
    private float totalHP;

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
        base.Init();
        totalHP = this.hp;
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
                t = 0;
                currentPhase++;
            }
        }

        //if(PhaseOne(t)) t =0;

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
            sq.append(LeanTween.color(gameObject, bossColor, 1));
            sq.append(() => Tween());
            return Phase.TimeReset;
        }
        return Phase.Nothing;
    }

    private Phase PhaseOne(float t)
    {

        float heightThreshold = -7.0f;
        if (t > 6)
        {
            if (!bulletSpawner.activeSelf)
            {
                bulletSpawner.SetActive(true);
            }
            LTSeq sq = LeanTween.sequence();
            sq.append(1.0f);
            sq.append(() => ProjectileAttack());
            
            if (thePlayer.transform.position.y <= heightThreshold)
            {
                sq.append(LeanTween.color(gameObject, Color.blue, 1));
                sq.append(() => SpawnMissiles(5, thePlayer.transform));
                sq.append(LeanTween.color(gameObject, bossColor, 1));
            }

            return Phase.TimeReset;
        }

        if (hp / totalHP <= 0.75)
        {
            bulletSpawner.SetActive(false);
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
        //ShotgunBullet temp = shotgunBulletVars.Copy();
        //temp.rotation = angle;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(temp, shotgun, transform));
        ShotgunBullet temp1 = shotgunBulletVars.Copy();
        temp1.rotation = angle - 25.0f;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(temp1, shotgun, transform));
        //shb.rotation = temp - 50.0f;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
        ShotgunBullet temp2 = shotgunBulletVars.Copy();
        temp2.rotation = angle + 25.0f;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(temp2, shotgun, transform));
        //shb.rotation = temp + 50;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
        LeanTween.color(gameObject, Color.green, 2.0f);
    }

    private float AngleTowardsPlayer()
    {
        Vector3 dir = transform.InverseTransformPoint(thePlayer.transform.position);
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
public class BossOneConstants
{
    public const string highAttack = "HighAttack";
    public const string lowAttack = "LowAttack";
    public const int totalBossPatterns = 3;
}