using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy
{
    [SerializeField] private ShotgunBullet sb;
    [SerializeField] private Transform[] crossHairs;
    [SerializeField] private GameObject bullet;
    [SerializeField] private string playerTag;

    // Remove collider, just using for radius debugging
    //private CircleCollider2D debugRadius;

    public float DistanceForTrigger;

    private float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        //debugRadius = GetComponent<CircleCollider2D>();
        //List<Vector2> points = new List<Vector2>();
        thePlayer = GameObject.FindGameObjectWithTag(playerTag);

    }

    // Update is called once per frame
    void Update()
    {
        //debugRadius.radius = DistanceForTrigger;
        int step = -sb.totalRight;
        int total = sb.totalLeft + sb.totalRight;

        sb.rotation = MathFuncs.AngleTowardsObject(thePlayer, transform);
        for (int t = 0; t < total; t++)
        {
            // Find angle via step rotation addition
            float angle = (Mathf.Deg2Rad * sb.rotation) + (Mathf.Deg2Rad * (float)step * sb.distanceBetweenBullets);
            Vector2 x = MathFuncs.PositionOnCircle(sb.circle, angle);
            crossHairs[t].transform.position = new Vector3(transform.position.x + x.x, transform.position.y + x.y, 0.0f);
            crossHairs[t].transform.rotation = Quaternion.Euler(new Vector3(0,0,MathFuncs.AngleTowardsObject(gameObject, crossHairs[t].transform)));
            step++;
        }
        t += Time.deltaTime;
        if (t > 2 && thePlayer)
        {
            t = 0;
            if (Vector2.Distance(transform.position, thePlayer.transform.position) <= DistanceForTrigger)
            StartCoroutine(BulletHellFuncs.ShotgunBullet(sb, bullet, transform));
        }

        if (IsDead())
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DistanceForTrigger);
    }
}
