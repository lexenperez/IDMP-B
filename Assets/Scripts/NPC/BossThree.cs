using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossVars;

public class BossThree : Enemy
{
    public GameObject bs;
    public GameObject trigger;
    public Transform phaseOneT;
    // Start is called before the first frame update
    void Start()
    {
        //bs = GetComponent<BulletSpawner>();
        base.Init();
       
    }

    // Update is called once per frame
    void Update()
    {
        bs.SetActive(true);

        if (trigger.GetComponent<BoxCollider2D>().IsTouching(thePlayer.GetComponent<BoxCollider2D>()))
        {
            Debug.Log("touching p");
            transform.LeanMove(phaseOneT.position, 15f);
            trigger.SetActive(false);
        }
    }


}
