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

    // Start is called before the first frame update
    void Start()
    {
        if (!horizontalProjectile)
        {
            horizontalProjectile = Resources.Load("Prefabs/Projectile") as GameObject;
        }
        if (!missile)
        {
            missile = Resources.Load("Prefabs/Missile") as GameObject;
        }
        base.Init();

        // Probs set values of projectiles here
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current[Key.Q].wasPressedThisFrame)
        {
            MeleeAttack(BossOneConstants.lowAttack);
            //StartCoroutine("hitbox");
            
        }
        if (Keyboard.current[Key.W].wasPressedThisFrame)
        {
           MeleeAttack(BossOneConstants.highAttack);
        }

        if (Keyboard.current[Key.E].wasPressedThisFrame)
        {
            FullLeftProjectileAttack();
        }

        if (Keyboard.current[Key.R].wasPressedThisFrame)
        {
            FullRightProjectileAttack();
        }

        if (Keyboard.current[Key.T].wasPressedThisFrame)
        {
            SpawnMissiles(totalMissiles, thePlayer.transform);
        }

        if (Keyboard.current[Key.Y].wasPressedThisFrame)
        {
            StartCoroutine(BulletHellFuncs.CircularBullet(cb, bullet, transform));
        }

        if (Keyboard.current[Key.U].wasPressedThisFrame)
        {
            StartCoroutine(BulletHellFuncs.SpiralBullet(sb, bulletLog, transform));
        }

        if (Keyboard.current[Key.I].wasPressedThisFrame)
        {
            StartCoroutine(BulletHellFuncs.ShotgunBullet(shb, shotgun, transform));
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

    // Use this for delayed patterns
    // i.e bullethell
    //IEnumerator spawn(GameObject temp)
    //{
    //    for (int i = 0; i < 3; i++)
    //    {
    //        GameObject go = SpawnProjectile(i, temp);
    //        //(2.0f);
    //        go.SetActive(true);
    //        yield return new WaitForSeconds(2.0f);
    //    }
    //    Destroy(temp);
    //}

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


}

public class BossOneConstants
{
    public const string highAttack = "HighAttack";
    public const string lowAttack = "LowAttack";
}
