using System.Collections;
using System.Collections.Generic;
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

    public CircleBullet cb;
    public SpiralBullet sb;
    public ShotgunBullet shb;
    public int totalMissiles;
    public static Vector3 testV;
    public Transform[] waypoints;
    enum State
    {
        Idle,
        Attack,
        Move,
        Death
    };
    public float lowerReactTime;
    public float higherReactTime;
    public float playerDistanceThreshold;
    private float reactTime = 99;
    private float t = 0;
    private int repeats = 0;
    private bool starth = true;
    private bool isMoving = false;
    private Vector3 waypoint;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();

        // Probs set values of projectiles here
    }

    // Update is called once per frame
    void Update()
    {
        //if (Keyboard.current[Key.Q].wasPressedThisFrame)
        //{
        //    float furthestPoint = 0;
        //    Vector3 bestPoint = Vector3.zero;
        //    for (int i = 0; i < waypoints.Length; i++)
        //    {
        //        float d = Vector3.Distance(thePlayer.transform.position, waypoints[i].position);
        //        if (d > furthestPoint)
        //        {
        //            furthestPoint = d;
        //            bestPoint = waypoints[i].position;
        //        }
        //    }
        //    if (bestPoint != Vector3.zero)
        //    {
        //        waypoint = bestPoint;
        //        isMoving = true;
        //    }
        //}
        //t += Time.deltaTime;
        if (repeats > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {
                Debug.Log("waiting for anim to finish");
                repeats = AlternateMeleeAttack(repeats, starth);
                starth = !starth;
            }
        }

        if (t >= reactTime)
        {
            t = 0;
            reactTime = Random.Range(lowerReactTime, higherReactTime);
            
            // If distance to player is on threshold
            // Do melee
            if (Vector2.Distance(thePlayer.transform.position, transform.position) > playerDistanceThreshold)
            {
                repeats = 2;
            }
            
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

            int projAttack = Random.Range(0, 2);
            if (projAttack == 0)
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

            // Then
            // Move if needed
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
            }
        }

    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Debug.Log("moving");
            Vector3 v = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
        }
    }

    private void MeleeAttack(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    private void FullLeftProjectileAttack()
    {
        horizontalProjectile.GetComponent<Projectile>().startingVelocity = new Vector2(-250f, 0.0f);
        //GameObject temp = Instantiate(horizontalProjectile);
        //temp.SetActive(false);
        //StartCoroutine(spawn(temp));
        for (int i = 0; i < 3; i++)
        {
            GameObject go = SpawnProjectile(i, horizontalProjectile);
            //(2.0f);
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
            GameObject go = Instantiate(missile, transform.position, Quaternion.identity, transform);
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

    // P1
    public void SpiralWithShotgun()
    {
        StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
        StartCoroutine(BulletHellFuncs.SpiralBullet(sb, bulletLog, transform));
        
    }

    // P2
    public void TwoSpirals()
    {
        StartCoroutine(BulletHellFuncs.CircularBullet(cb, bullet, transform));
        StartCoroutine(BulletHellFuncs.CircularBullet(cb, bullet, transform));
    }

    // P3
    public void MultipleShotgun()
    {
        float temp = AngleTowardsPlayer();
        shb.rotation = temp;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
        ShotgunBullet temp1 = shb.Copy();
        temp1.rotation = temp - 25.0f;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(temp1, shotgun, transform));
        //shb.rotation = temp - 50.0f;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
        ShotgunBullet temp2 = shb.Copy();
        temp2.rotation = temp + 25.0f;
        StartCoroutine(BulletHellFuncs.ShotgunBullet(temp2, shotgun, transform));
        //shb.rotation = temp + 50;
        //StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
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
//if (Keyboard.current[Key.Q].wasPressedThisFrame)
//{
//    AlternateMeleeAttack(20, true);
//    //StartCoroutine("hitbox");

//}
//if (Keyboard.current[Key.W].wasPressedThisFrame)
//{
//    Vector2 playerDirection = transform.InverseTransformPoint(thePlayer.transform.position);
//    if (playerDirection.x < 0)
//    {
//        FullLeftProjectileAttack();
//    }
//    else if (playerDirection.x > 0)
//    {
//        FullRightProjectileAttack();
//    }
//}

//if (Keyboard.current[Key.E].wasPressedThisFrame)
//{
//    FullLeftProjectileAttack();
//}

//if (Keyboard.current[Key.R].wasPressedThisFrame)
//{
//    FullRightProjectileAttack();
//}

//if (Keyboard.current[Key.T].wasPressedThisFrame)
//{
//    SpawnMissiles(totalMissiles, thePlayer.transform);
//}

//if (Keyboard.current[Key.Y].wasPressedThisFrame)
//{
//    MultipleShotgun();
//}

//if (Keyboard.current[Key.U].wasPressedThisFrame)
//{
//    TwoSpirals();
//}

//if (Keyboard.current[Key.I].wasPressedThisFrame)
//{
//    SpiralWithShotgun();
//}