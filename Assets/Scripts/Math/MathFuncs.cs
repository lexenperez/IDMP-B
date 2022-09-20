using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFuncs : MonoBehaviour
{
    const double GoldenRatio = 1.61803398874989484820458683436;
    // Math Functions for projectiles (possibly other general math stuff)

    [System.Serializable]
    public enum GrowthFactor
    {
        GoldenRatio,
        Euler

    }

    // Y = 
    // Returns paremetric position of a circle where
    // a = Circle's x center
    // b = Circle's y center
    // r = Circle's radius
    // t = Parametric variable
    public static Vector2 PositionOnCircle(Circle c, float t)
    {
        float x = c.a + c.r * Mathf.Cos(t);
        float y = c.b + c.r * Mathf.Sin(t);
        return new Vector2(x, y);
    }

    public static Vector2 LogarithmicSpiral(float a, float b, GrowthFactor gf, float t)
    {
        if (gf == GrowthFactor.GoldenRatio)
        {
            float x = a * Mathf.Pow((float)GoldenRatio, b * t) * Mathf.Cos(t);
            float y = a * Mathf.Pow((float)GoldenRatio, b * t) * Mathf.Sin(t);
            return new Vector2(x, y);
        }
        else if (gf == GrowthFactor.Euler)
        {
            float exp = Mathf.Exp(1.0f);
            float x = a * Mathf.Pow(exp, b * t) * Mathf.Cos(t);
            float y = a * Mathf.Pow(exp, b * t) * Mathf.Sin(t);
            return new Vector2(x, y);
        }
        else return new Vector2(0, 0);

    }

    // Simulates a coinflip via random 0 to 1
    // Returns false if 0, 1 if true
    public static bool CoinFlip()
    {
        int flip = Random.Range(0, 2);
        if (flip == 0) return false;
        else return true;
    }

    // Simulates a 0-100% chance
    public static bool Chance(float percentage)
    {
        return Random.value > (1.0f - percentage);
    }
    public static float AngleTowardsObject(GameObject target, Transform origin)
    {
        if (target)
        {
            Vector3 dir = (target.transform.position);
            return Mathf.Atan2(dir.y - origin.position.y, dir.x - origin.position.x) * Mathf.Rad2Deg;
        }
        return 0.0f;
    }

}

[System.Serializable]
public class Circle
{
    public float a;
    public float b;
    public float r;
}