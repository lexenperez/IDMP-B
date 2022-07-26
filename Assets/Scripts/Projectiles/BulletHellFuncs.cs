using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellFuncs : MonoBehaviour
{
    // Coroutines for spawning bullets
    public static IEnumerator CircularBullet(CircleBullet cb, GameObject obj, Transform parent)
    {
        List<Vector2> points = GetPointsInCircle(cb);

        // Now that we have the points, repeat each spawn at each location per "repeats" with at each spawnInterval
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = points[i];
            // Create objects and push it away from parent
            GameObject go = Instantiate(obj);
            Vector3 conversion = p;
            go.transform.position = parent.position + conversion;
            Vector3 dir = (go.transform.position - parent.position).normalized;
            go.GetComponent<Rigidbody2D>().AddForce(dir * cb.speed);
            go.SetActive(true);
            if (i % cb.stepSize == 0 && i != 0)
            {
                yield return new WaitForSeconds(cb.spawnInterval);
            }
        }

    }

    public static IEnumerator CircularBullet(CircleBullet cb, List<Vector2> points, GameObject obj, Transform parent)
    {
        // Pre-calculated option to reduce lag when spawning in circle spawners that might freeze the game when playing
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = points[i];
            // Create objects and push it away from parent
            GameObject go = Instantiate(obj);
            Vector3 conversion = p;
            go.transform.position = parent.position + conversion;
            Vector3 dir = (go.transform.position - parent.position).normalized;
            go.GetComponent<Rigidbody2D>().AddForce(dir * cb.speed);
            go.SetActive(true);
            if (i % cb.stepSize == 0 && i != 0)
            {
                yield return new WaitForSeconds(cb.spawnInterval);
            }
        }
    }

    // This might be super expensive to use
    public static IEnumerator SpiralBullet(SpiralBullet sb, GameObject obj, Transform parent)
    {
        for (int i = 0; i < sb.repeats; i++)
        {
            GameObject go = Instantiate(obj);
            go.transform.position = parent.position + go.transform.position;
            go.GetComponent<BulletLog>().bv = sb.bulletVariables;
            go.GetComponent<BulletLog>().speed = sb.speed;
            go.SetActive(true);
            yield return new WaitForSeconds(sb.spawnInterval);
        }
    }

    //Same as circle except pick one point and move left/right on the circle x amount of times
    public static IEnumerator ShotgunBullet(ShotgunBullet sb, GameObject obj, Transform parent)
    {

        List<Vector2> points = new List<Vector2>();
        int step = -sb.totalRight;
        int total = sb.totalLeft + sb.totalRight;
        
        for (int t = 0; t < total; t++)
        {
            // Find angle via step rotation addition
            float angle = (Mathf.Deg2Rad * sb.rotation) + (Mathf.Deg2Rad * (float)step * sb.distanceBetweenBullets);
            Vector2 x = MathFuncs.PositionOnCircle(sb.circle, angle);
            points.Add(x);
            step++;
        }

        for (int i = 0; i < sb.repeats; i++)
        {
            foreach (Vector2 p in points)
            {
                // Dont make parent since it'll follow the boss
                GameObject go = Instantiate(obj);
                Vector3 conversion = p;
                go.transform.position = parent.position + conversion;
                Vector3 dir = (go.transform.position - parent.position).normalized;
                go.GetComponent<Rigidbody2D>().AddForce(dir * sb.speed);
                go.SetActive(true);
            }
            yield return new WaitForSeconds(sb.spawnInterval);
        }

    }

    public static List<Vector2> GetPointsInCircle(CircleBullet cb)
    {
        // Calculate points on circle first
        List<Vector2> points = new List<Vector2>();
        int step = 0;
        // Find all positions from stepSize (initial positions) + repeats (initial positions rotated a bit) possibly more efficient than 2 for loops at the end
        for (int t = 0; t < (cb.stepSize * cb.repeats); t++)
        {
            float angle = ((Mathf.PI * 2.0f / cb.stepSize) * t) + (Mathf.Deg2Rad * cb.rotation * (float)step);
            Vector2 x = MathFuncs.PositionOnCircle(cb.circle, angle);
            points.Add(x);
            if (t % cb.stepSize == 0 && t != 0)
            {
                step++;
            }
        }

        return points;
    }
}

[System.Serializable]
public class BulletVars
{
    public float a;
    public float b;
    public MathFuncs.GrowthFactor gf;
}

[System.Serializable]
public class CircleBullet
{
    public int stepSize;
    public float spawnInterval;
    public int repeats;
    public float speed;
    public float rotation;
    public Circle circle;

    public CircleBullet Copy()
    {
        CircleBullet c = new CircleBullet();
        c.stepSize = stepSize;
        c.spawnInterval = spawnInterval;
        c.repeats = repeats;
        c.speed = speed;
        c.rotation = rotation;
        c.circle = circle;
        return c;

    }
}

[System.Serializable]
public class SpiralBullet
{
    public float spawnInterval;
    public int repeats;
    public float speed;
    public Vector3 position;
    public BulletVars bulletVariables;

    public SpiralBullet Copy()
    {
        SpiralBullet s = new SpiralBullet();
        s.spawnInterval = spawnInterval;
        s.repeats = repeats;
        s.speed = speed;
        s.position = position;
        s.bulletVariables = bulletVariables;
        return s;
    }
}

[System.Serializable]
public class ShotgunBullet
{
    public int totalLeft, totalRight;
    public float spawnInterval;
    public int repeats;
    public float speed;
    public float rotation;
    public float distanceBetweenBullets;
    public Circle circle;
    
    public ShotgunBullet Copy()
    {
        ShotgunBullet b = new ShotgunBullet();
        b.totalLeft = totalLeft;
        b.totalRight = totalRight;
        b.spawnInterval = spawnInterval;
        b.repeats = repeats;
        b.speed = speed;
        b.rotation = rotation;
        b.distanceBetweenBullets = distanceBetweenBullets;
        b.circle = circle;
        return b;
    }
}
