using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ManageBoss2 : MonoBehaviour
{
    [SerializeField] Boss2 boss1, boss2;
    [SerializeField] TrailRenderer boss1trail, boss2trail;
    private GameObject player;
    [SerializeField] bool phase1 = true;
    private bool phase0;
    private float timer = 0, grace;
    private float initial_grace = 6.5f;
    private Vector3 home1 = new Vector3(-30, 7, 0), home2 = new Vector3(-30, 10, 0);
    private int NoOfBosses = 2;
    private bool busy = false;
    private int choice, lastchoice = 0;
    [SerializeField] ParticleSystem ps1, ps2;

    //atack pattern choices
    private int attack1 = 25, attack2 = 10, attack3 = 10, attack4 = 10;
    private int total, result;
    private bool needToUpdate = false;

    //charge attack
    [SerializeField] bool charge;
    [SerializeField] float totalChargeTime;
    [SerializeField] float attackSpeed;

    //double attack
    [SerializeField] bool doublecharge;
    [SerializeField] float totalDoubleTime;

    //Beyblade
    [SerializeField] bool beyblade;
    [SerializeField] float totalBeybladeTime;
    [SerializeField] GameObject centre;
    [SerializeField] float spinspeed;
    private bool spawnOnce = true;

    //circle attack
    [SerializeField] bool circle;
    [SerializeField] float totalCircleTime;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bullet2;
    [SerializeField] GameObject laser;
    private float cooldown;
    private Vector3 v = new Vector3(-13, 0, 0), o;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ps1.Stop();
        ps2.Stop();
        grace = initial_grace;
        boss1.Collider.enabled = false;
        boss1.Head.SetActive(false);
        boss2.Collider.enabled = false;
        boss2.Head.SetActive(false);
        phase0 = true;
        busy = true;
    }

    #region FSM
    private void FixedUpdate()
    {

        if (boss1 == null && boss2 == null)
            return;

        if (boss1.thePlayer == null || boss2.thePlayer == null)
        {
            timer = -999;
            return;
        }
        if (boss1.hp <= 0 && boss2.hp <= 0)
        {
            //everything should be destroyed at this point because boss1 and 2 are the same thing.
            if (boss1 != null)
                Destroy(boss1.gameObject);
            timer = -999;
        }
        else if (boss1.hp <= 0)
        {
            Phase2(boss2, boss2trail, boss1);
        }
        else if (boss2.hp <= 0)
        {
            Phase2(boss1, boss1trail, boss2);
        }

        timer += Time.deltaTime;
        if (!busy)
        {
            //small grace window between attacks
            if (timer < grace)
            {
                //do nothing
            }
            else
            {   
                timer = 0;
                //pick attack
                PickAttack();
                switch (choice)
                {
                    case 1:
                        charge = true;
                        break;
                    case 2:
                        doublecharge = true;
                        break;
                    case 3:
                        beyblade = true;
                        break;
                    case 4:
                        circle = true;
                        break;
                }
            }

        }
        //excecute chosen attack
        else
        {
            if (phase0)
            {
                BossEntrance();
            }
            if (charge)
            {
                ChargeAttack();
                if (needToUpdate)
                {
                    UpdateValues(1);
                }
            }
            if (doublecharge)
            {
                DoubleCharge();
                if (needToUpdate)
                {
                    UpdateValues(2);
                }
            }
            if (beyblade)
            {
                BeyBlade();
                if (needToUpdate)
                {
                    UpdateValues(3);
                }
            }
            if (circle)
            {
                Circle();
                if (needToUpdate)
                {
                    UpdateValues(4);
                }
            }
        }
    }
    #endregion

    #region Attacks
    private void ChargeAttack()
    {
        //ShootOutButt();
        if (timer < totalChargeTime)
        {
            //has to be in reverse order in order to not repeat actions
            //only accelerate for 0.1 sec
            if (timer > (totalChargeTime / 5) * 4.1f)
            {
                ShootOutButt();
            }
            //attack 2
            else if (timer > (totalChargeTime / 5) * 4)
            {
                ShootOutButt();
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(1 * attackSpeed, 0, 0);
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
            }
            //position 2
            else if (timer > (totalChargeTime / 5) * 3)
            {
                ShootOutButt();
                boss2.transform.eulerAngles = new Vector3(0, 0, 180);
                boss2trail.Clear();
                boss2.transform.position = new Vector3(-15, player.transform.position.y, 0);
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            //only accelerate for 0.1 sec
            else if (timer > (totalChargeTime / 5) * 2.1f)
            {
                ShootOutButt();
            }
            //attack
            else if (timer > (totalChargeTime / 5) * 2)
            {
                ShootOutButt();
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(-1 * attackSpeed, 0, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
            }
            //position
            else if (timer > totalChargeTime / 5)
            {
                ShootOutButt();
                boss1trail.Clear();
                boss1.transform.position = new Vector3(15, player.transform.position.y, 0);
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
        }
        else
        {
            ResetAll();
        }
    }

    private void DoubleCharge()
    {
        if (timer < totalDoubleTime)
        {
            //only accelerate for 0.1 sec
            if (timer > (totalDoubleTime / 5) * 2.1f)
            {
                ShootOutButt();
            }
            //attack
            else if (timer > (totalDoubleTime / 5) * 2)
            {
                ShootOutButt();
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(-1 * attackSpeed, 0, 0);
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(1 * attackSpeed, 0, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
            }
            //position
            else if (timer > totalDoubleTime / 5)
            {
                ShootOutButt();
                boss1trail.Clear();
                boss1.transform.position = new Vector3(15, player.transform.position.y, 0);
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.eulerAngles = new Vector3(0, 0, 180);
                ShootOutButt();
                boss2trail.Clear();
                boss2.transform.position = new Vector3(-15, player.transform.position.y, 0);
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
        }
        else
        {
            ResetAll();
        }
    }

    private void BeyBlade()
    {
        //spawn blade
        if (timer < 1)
        {
            if (spawnOnce)
            {
                centre.SetActive(true);
                centre.GetComponent<circle>().Pause();
                spawnOnce = false;
                boss2trail.Clear();
                boss1trail.Clear();
                if (!boss1.AudioTest())
                {
                    boss1.SpinUp();
                }
                if (!boss2.AudioTest())
                {
                    boss2.SpinUp();
                }
            }
            boss1.transform.position = centre.transform.position + new Vector3(-3.8f, 0, 0);
            boss1.transform.parent = centre.transform;
            boss2.transform.eulerAngles = new Vector3(0, 0, 180);
            boss2.transform.position = centre.transform.position + new Vector3(3.8f, 0, 0);
            boss2.transform.parent = centre.transform;
        }
        else if (timer < totalBeybladeTime)
        {
            centre.GetComponent<circle>().Resume();
            if (cooldown > 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    GameObject currentBullet = Instantiate(bullet2);
                    currentBullet.transform.position = centre.transform.position;
                    currentBullet.GetComponent<Rigidbody2D>().velocity = new Vector3(Mathf.Cos(Mathf.PI*i/3), Mathf.Sin(Mathf.PI*i/3), 0) * 15;
                }
                cooldown = 0;
            }
            cooldown += Time.deltaTime;
            if (!boss1.AudioTest())
            {
                boss1.Beyblade();
            }
            if (!boss2.AudioTest())
            {
                boss2.Beyblade();
            }
        }
        else
        {
            ResetAll();
        }
    }

    private void Circle()
    {
        if (timer < 1)
        {
            boss1trail.Clear();
            boss1.transform.position = new Vector3(-13, 0, 0);
            boss1.transform.eulerAngles = new Vector3(0, 0, 90);
            boss2trail.Clear();
            boss2.transform.position = new Vector3(13, 0, 0);
            boss2.transform.eulerAngles = new Vector3(0, 0, -90);
            if (!boss1.AudioTest())
            {
                boss1.Circle();
            }
            if (!boss2.AudioTest())
            {
                boss2.Circle();
            }
        }
        else if (timer < totalCircleTime)
        {

            v = Quaternion.AngleAxis(360 / totalCircleTime * 5 * Time.deltaTime, Vector3.forward) * v;
            o = new Vector3(0, Mathf.Sin(2 * Mathf.PI / totalCircleTime * 5 * (timer - 1)), 0);
            boss1.transform.position = new Vector3(0, 0, 0) + v + o * 6;
            boss1.transform.eulerAngles = new Vector3(0, 0, 90 + 360 * (timer - 1) / NoOfBosses);
            boss2.transform.position = new Vector3(0, 0, 0) - v - o * 6;
            boss2.transform.eulerAngles = new Vector3(0, 0, 270 + 360 * (timer - 1) / NoOfBosses);
            //shoot projectiles at player while moving
            if (cooldown > 0.4f)
            {
                if ((float)Mathf.Floor(timer) % 2 == 0)
                {
                    GameObject currentBullet = Instantiate(bullet);
                    currentBullet.transform.position = boss1.transform.position;
                    currentBullet.GetComponent<Rigidbody2D>().velocity =
                        (player.transform.position - currentBullet.transform.position).normalized * 8;
                    cooldown = 0;
                }
                else
                {
                    GameObject currentBullet = Instantiate(bullet);
                    currentBullet.transform.position = boss2.transform.position;
                    currentBullet.GetComponent<Rigidbody2D>().velocity =
                        (player.transform.position - currentBullet.transform.position).normalized * 8;
                    cooldown = 0;
                }
            }
            cooldown += Time.deltaTime;
        }
        else
        {
            ResetAll();
        }

    }

    private void ShootOutButt()
    {
        if (cooldown > 0.2f)
        {
            GameObject currentlaser = Instantiate(laser);
            currentlaser.transform.position = boss1.transform.position + boss1.transform.right * 2.5f - new Vector3(0, 0, 0.05f);
            currentlaser = Instantiate(laser);
            currentlaser.transform.position = boss2.transform.position + boss2.transform.right * 2.5f - new Vector3(0, 0, 0.05f);
            cooldown = 0;
        }

        cooldown += Time.deltaTime;
    }
    #endregion

    #region Util

    private int PickAttack()
    {
        if (phase1)
        {
            total = attack1 + attack2 + attack3 + attack4;
        }
        else
        {
            total = attack1 + attack3 + attack4;
        }
        while (choice == lastchoice)
        {
            result = Random.Range(1, total + 1);
            if (result <= attack1)
            {
                choice = 1;
            }
            else if (result <= attack1 + attack3)
            {
                choice = 3;
            }
            else if (result <= attack1 + attack3 + attack4)
            {
                choice = 4;
            }
            else
            {
                choice = 2;
            }
        }
        lastchoice = choice;
        busy = true;
        needToUpdate = true;
        return choice;
    }

    private void ResetAll()
    {
        spawnOnce = true;
        centre.SetActive(false);
        centre.GetComponent<circle>().Reset();
        doublecharge = false;
        charge = false;
        circle = false;
        beyblade = false;
        boss1.transform.position = home1;
        boss1.transform.parent = null;
        boss1trail.Clear();
        boss1.transform.eulerAngles = new Vector3(0, 0, 0);
        boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        boss2.transform.position = home2;
        boss2.transform.parent = null;
        boss2.transform.eulerAngles = new Vector3(0, 0, 0);
        boss2trail.Clear();
        boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        timer = 0;
        v = new Vector3(-13, 0, 0);
        busy = false;
        phase0 = false;
        grace = 1.5f;
        boss1.Collider.enabled = true;
        boss1.Head.SetActive(true);
        boss2.Collider.enabled = true;
        boss2.Head.SetActive(true);
        boss1.StopAudio();
        boss2.StopAudio();
    }

    private void Phase2(Boss2 survivor,TrailRenderer survivortrail, Boss2 dead)
    {
        ResetAll();
        NoOfBosses = 1;
        dead.DestroyHead();
        Destroy(dead.gameObject);
        boss2 = survivor;
        boss1 = survivor;
        boss1trail = survivortrail;
        boss2trail = survivortrail;
        //speed up charge and double charge
        totalChargeTime = 3.5f;
        totalDoubleTime = 3;
        //speed up beyblade attack and circle attack
        centre.GetComponent<circle>().speed = 2;
        totalCircleTime = 5;
        //make changes only once
        phase1 = false;
        grace = 1;
    }

    private void BossEntrance()
    {
        //dash top and bottom
        if (timer < 6.5)
        {
            if (timer > (initial_grace / 24) * 21)
            {
                boss1.transform.eulerAngles = new Vector3(0, 0, 0);
                boss1.transform.position = new Vector3(15, 0, 0);
                boss1trail.Clear();
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.eulerAngles = new Vector3(0, 0, 180);
                boss2.transform.position = new Vector3(-15, 0, 0);
                boss2trail.Clear();
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                ps1.Play();
                if (!boss1.AudioTest())
                {
                    boss1.Explode();
                }
                ps2.Play();
                if (!boss2.AudioTest())
                {
                    boss2.Explode();
                }
            }
            else if (timer > (initial_grace / 24) * 20)
            {
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(-50, -50, 0);
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
            }
            else if (timer > (initial_grace / 24) * 19)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(50, 50, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
                boss2.transform.position = new Vector3(-6, 16, 0);
                boss2trail.Clear();
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.eulerAngles = new Vector3(0, 0, 45);

            }
            else if (timer > (initial_grace / 24) * 18)
            {
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(50, -50, 0);
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
                boss1.transform.position = new Vector3(6, -16, 0);
                boss1trail.Clear();
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero; 
                boss1.transform.eulerAngles = new Vector3(0, 0, 225);
            }
            else if (timer > (initial_grace / 24) * 17)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(-50, 50, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
                boss2.transform.position = new Vector3(6, 16, 0);
                boss2trail.Clear();
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.eulerAngles = new Vector3(0, 0, -225);
            }
            else if (timer > (initial_grace / 24) * 16)
            {
                boss1.transform.position = new Vector3(-6, -16, 0);
                boss1trail.Clear();
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss1.transform.eulerAngles = new Vector3(0, 0, -45);
            }
            else if (timer > (initial_grace / 24) * 14)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 100, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -100, 0);
            }
            else if (timer > (initial_grace / 24) * 12)
            {
                boss1.transform.position = new Vector3(-16, -16, 0);
                boss1trail.Clear();
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.position = new Vector3(16, 16, 0);
                boss2trail.Clear();
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            else if (timer > (initial_grace / 24) * 10)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 100, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -100, 0);
            }
            else if (timer > (initial_grace / 24) * 8)
            {
                boss1.transform.position = new Vector3(16, -16, 0);
                boss1trail.Clear();
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss1.transform.eulerAngles = new Vector3(0, 0, -90);
                boss2.transform.position = new Vector3(-16, 16, 0);
                boss2trail.Clear();
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.eulerAngles = new Vector3(0, 0, 90);
            }
            else if (timer > (initial_grace / 24) * 6)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(-100, 0, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(100, 0, 0);
            }
            else if (timer > (initial_grace / 24) * 4)
            {
                boss1.transform.position = new Vector3(25, -7.5f, 0);
                boss1trail.Clear();
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.position = new Vector3(-25, 8, 0);
                boss2trail.Clear();
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            else if (timer > (initial_grace / 24) * 2)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(-100, 0, 0);
                if (!boss1.AudioTest())
                {
                    boss1.DashSound();
                }
                if (!boss2.AudioTest())
                {
                    boss2.DashSound();
                }
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(100, 0, 0);
            }
            else 
            {
                boss1.transform.position = new Vector3(25, 8, 0);
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss1trail.Clear();
                boss2.transform.position = new Vector3(-25, -7.5f, 0);
                boss2trail.Clear();
                boss2.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.eulerAngles = new Vector3(0, 0, 180);
            }
        }
        else
        {
            ResetAll();
            ps1.Stop();
            ps2.Stop();
        }
    }

    private void UpdateValues(int choice)
    {
        switch (choice) 
        {
            case 1:
                attack1 -= 5;
                attack2 += 2;
                attack3 += 2;
                attack4 += 2;
                break;
            case 2:
                attack1 += 2;
                attack2 -= 10;
                attack3 += 2;
                attack4 += 2;
                break;
            case 3:
                attack1 += 2;
                attack2 += 2;
                attack3 -= 5;
                attack4 += 2;
                break;
            case 4:
                attack1 += 2;
                attack2 += 2;
                attack3 += 2;
                attack4 -= 5;
                break;
        }
        if (attack1 < 0){
            attack1 = 0;
        }if (attack2 < 0){
            attack2 = 0;
        }if (attack2 < 0){
            attack2 = 0;
        }if (attack2 < 0){
            attack2 = 0;
        }
        needToUpdate = false;
    }
    #endregion
}
