using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageBoss2 : MonoBehaviour
{
    [SerializeField] GameObject boss1, boss2;
    private GameObject player;
    [SerializeField] bool phase1 = true;
    private float timer = 0;
    public float boss1Health = 100, boss2Health = 100;
    public float totalHealth = 100;
    private Vector3 home1 = new Vector3(-30, 7, 0), home2 = new Vector3(-30, 10, 0);
    private int NoOfBosses = 2;
    private bool busy = false;
    private int choice, lastchoice = 0;

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
    private float cooldown;
    private Vector3 v, o;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        v = new Vector3(-13, 0, 0);
    }

    #region FSM
    private void FixedUpdate()
    {
        if (boss1Health <= 0 || boss2Health <= 0)
        {
            if (phase1)
            {
                Phase2();
            }
        }
        timer += Time.deltaTime;
        if (!busy)
        {
            //small grace window between attacks
            if (timer < 1)
            {
                //do nothing
            }
            else
            {
                timer = 0;
                //pick attack
                choice = Random.Range(1, 5);
                //dont repeat last choice
                while (choice == lastchoice)
                {
                    choice = Random.Range(1, 5);
                }
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
                lastchoice = choice;
                busy = true;
            }

        }
        //excecute chosen attack
        else
        {
            if (charge)
            {
                ChargeAttack();
            }
            if (doublecharge)
            {
                DoubleCharge();
            }
            if (beyblade)
            {
                BeyBlade();
            }
            if (circle)
            {
                Circle();
            }
        }
    }
    #endregion

    #region Attacks
    private void ChargeAttack()
    {
        if (timer < totalChargeTime)
        {
            //has to be in reverse order in order to not repeat actions
            //only accelerate for 0.1 sec
            if (timer > (totalChargeTime / 5) * 4.1f)
            {
                //do nothing
            }
            //attack 2
            else if (timer > (totalChargeTime / 5) * 4)
            {
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(1 * attackSpeed, 0, 0);
            }
            //position 2
            else if (timer > (totalChargeTime / 5) * 3)
            {
                boss2.transform.eulerAngles = new Vector3(0,180,0);
                boss2.transform.position = new Vector3(-13, player.transform.position.y, 0);
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            //only accelerate for 0.1 sec
            else if (timer > (totalChargeTime / 5) * 2.1f)
            {
                //do nothing
            }
            //attack
            else if(timer > (totalChargeTime / 5) * 2)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(-1 * attackSpeed, 0, 0);

            }
            //position
            else if(timer > totalChargeTime / 5)
            {
                boss1.transform.position = new Vector3(13, player.transform.position.y, 0);
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
                //do nothing
            }
            //attack
            else if (timer > (totalDoubleTime / 5) * 2)
            {
                boss1.GetComponent<Rigidbody2D>().velocity = new Vector3(-1 * attackSpeed, 0, 0);
                boss2.GetComponent<Rigidbody2D>().velocity = new Vector3(1 * attackSpeed, 0, 0);
            }
            //position
            else if (timer > totalDoubleTime / 5)
            {
                boss1.transform.position = new Vector3(13, player.transform.position.y, 0);
                boss1.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                boss2.transform.eulerAngles = new Vector3(0, 180, 0);
                boss2.transform.position = new Vector3(-13, player.transform.position.y, 0);
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
            }
            boss1.transform.position = centre.transform.position + new Vector3(-4.1f, 0, 0);
            boss1.transform.parent = centre.transform;
            boss2.transform.eulerAngles = new Vector3(0, 180, 0);
            boss2.transform.position = centre.transform.position + new Vector3(4.1f, 0, 0);
            boss2.transform.parent = centre.transform;
        }
        else if (timer < totalBeybladeTime)
        {
            centre.GetComponent<circle>().Resume();
        }
        else
        {
            ResetAll();
        }
    }

    private void Circle()
    {
        
        if (timer < totalCircleTime)
        {
            
            v = Quaternion.AngleAxis(360 / totalCircleTime*5 * Time.deltaTime, Vector3.forward) * v;
            o = new Vector3(0, Mathf.Sin(2 * Mathf.PI / totalCircleTime*5 * timer), 0);
            boss1.transform.position = new Vector3(0, 0, 0) + v + o * 6;
            boss1.transform.eulerAngles = new Vector3(0, 0, 90 + 360 * timer/NoOfBosses);
            boss2.transform.position = new Vector3(0, 0, 0) - v - o * 6;
            boss2.transform.eulerAngles = new Vector3(0, 0, 270 + 360 * timer/NoOfBosses);
            //shoot projectiles at player while moving
            if (cooldown > 0.4f)
            {
                if ((float)Mathf.Floor(timer) % 2 == 0)
                {
                    GameObject currentBullet = Instantiate(bullet);
                    currentBullet.transform.position = boss1.transform.position;
                    currentBullet.GetComponent<Rigidbody2D>().velocity =
                        (player.transform.position - currentBullet.transform.position).normalized * 5;
                    cooldown = 0;
                }
                else
                {
                    GameObject currentBullet = Instantiate(bullet);
                    currentBullet.transform.position = boss2.transform.position;
                    currentBullet.GetComponent<Rigidbody2D>().velocity =
                        (player.transform.position - currentBullet.transform.position).normalized * 5;
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
    #endregion

    #region Util
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
        boss1.transform.eulerAngles = new Vector3(0, 0, 0);
        boss2.transform.position = home2;
        boss2.transform.parent = null;
        boss2.transform.eulerAngles = new Vector3(0, 0, 0);
        timer = 0;
        v = new Vector3(-13, 0, 0);
        busy = false;
    }

    private void Phase2()
    {
        ResetAll();
        NoOfBosses = 1;
        boss2 = boss1;
        //speed up charge and double charge
        totalChargeTime = 3.5f;
        totalDoubleTime = 3;
        //speed up beyblade attack and circle attack
        centre.GetComponent<circle>().speed = 2;
        totalCircleTime = 5;
        //make changes only once
        phase1 = false;
    }
    #endregion
}
