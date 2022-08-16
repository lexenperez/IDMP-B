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

    private float reactTime = 5;
    private float t = 0;
    private float moveT = 0;
    //private int repeats = 0;
    //private bool starth = true;
    private bool isMoving = false;
    private Vector3 waypoint;



    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        Tween();

        // Probs set values of projectiles here
    }

    public void Tween()
    {
        LeanTween.cancel(gameObject);
        LeanTween.rotateAround(gameObject, Vector3.forward, 360.0f, tweenTime).setRepeat(-1);
    }
    // Update is called once per frame
    void Update()
    {
        
        if (Keyboard.current[Key.Q].wasPressedThisFrame)
        {
            SpawnMissiles(3, thePlayer.transform);
        }

        // Stop React State when moving
        if (!isMoving)
        {
            t += Time.deltaTime;
            moveT += Time.deltaTime;
        }
       
       

        // Start React State
        if (t >= reactTime)
        {
            t = 0;
            reactTime = Random.Range(lowerReactTime, higherReactTime);

            // Melee a bit bugged so leave for now
            //if (repeats > 0)
            //{
            //    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            //    {
            //        Debug.Log("waiting for anim to finish");
            //        repeats = AlternateMeleeAttack(repeats, starth);
            //        starth = !starth;
            //    }
            //}

            // If distance to player is on threshold
            // Do melee
            //if (Vector2.Distance(thePlayer.transform.position, transform.position) > playerDistanceThreshold)
            //{
            //    repeats = 2;
            //    int coinflip = Random.Range(0, 2);
            //    switch(coinflip)
            //    {
            //        case 0:
            //            starth = false;
            //            break;
            //        case 1:
            //            starth = true;
            //            break;
            //        default:
            //            break;
            //    }
            //}

            // Then
            // Randomise total patterns and pick a pattern
            // Execute it
            // Could store patterns in a pattern class
            int patternToExecute = Random.Range(0, BossOneConstants.totalBossPatterns);
            switch(patternToExecute)
            {
                case 0:
                    SpiralWithShotgun();
                    break;
                case 1:
                    TwoSpirals();
                    break;
                case 2:
                    MultipleShotgun();
                    break;
                default:
                    break;
            }

            bool projAttack = MathFuncs.Chance(projectileChance);
            if (projAttack)
            {
                Vector2 playerDirection = transform.InverseTransformPoint(thePlayer.transform.position);
                if (playerDirection.x < 0)
                {
                    FullLeftProjectileAttack();
                }
                else if (playerDirection.x > 0)
                {
                    FullRightProjectileAttack();
                }
            }


            bool missileAttack = MathFuncs.Chance(missileChance);
            if (missileAttack)
            {
                FireMissiles();
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

    private void MeleeAttack(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    private void FullLeftProjectileAttack()
    {
        horizontalProjectile.GetComponent<Projectile>().startingVelocity = new Vector2(-250f, 0.0f);
        for (int i = 0; i < 3; i++)
        {
            GameObject go = SpawnProjectile(i, horizontalProjectile);
            go.SetActive(true);
        }
    }

    private void FullRightProjectileAttack()
    {
        horizontalProjectile.GetComponent<Projectile>().startingVelocity = new Vector2(250f, 0.0f);
        for (int i = 3; i < 6; i++)
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