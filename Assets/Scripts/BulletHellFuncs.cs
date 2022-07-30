using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellFuncs : MonoBehaviour
{
    // Coroutines for spawning bullets

    public static IEnumerator CircularBullet(int stepSize, float spawnInterval, int repeats, float speed, float rotation, Circle c, GameObject obj, Transform parent)
    {
        // Calculate points on circle first
        List<Vector2> points = new List<Vector2>();
        for (int t = 0; t < stepSize; t++)
        {
            float angle = ((Mathf.PI * 2.0f / stepSize) * t) + (Mathf.Deg2Rad * rotation);
            Vector2 x = MathFuncs.PositionOnCircle(c, angle);
            points.Add(x);
        }

        // Now that we have the points, repeat each spawn at each location per "repeats" with at each spawnInterval
        for (int i = 0; i < repeats; i++)
        {
            foreach (Vector2 p in points)
            {
                // Create objects and push it away from parent
                GameObject go = Instantiate(obj, p, Quaternion.identity, parent);
                Debug.Log(go);
                Vector3 dir = (go.transform.position - parent.position).normalized;
                go.GetComponent<Rigidbody2D>().AddForce(dir * speed);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // This might be super expensive to use
    public static IEnumerator SpiralBullet(float spawnInterval, int repeats, float speed, Vector3 position, BulletVars bv, GameObject obj, Transform parent)
    {
        for (int i = 0; i < repeats; i++)
        {
            GameObject go = Instantiate(obj, position, Quaternion.identity, parent);
            go.GetComponent<BulletLog>().bv = bv;
            go.GetComponent<BulletLog>().speed = speed;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    //Same as circle except pick one point and move left/right on the circle x amount of times
    //IEnumerator ShotgunBullet()
}

[System.Serializable]
public class BulletVars
{
    public float a;
    public float b;
    public MathFuncs.GrowthFactor gf;
}
