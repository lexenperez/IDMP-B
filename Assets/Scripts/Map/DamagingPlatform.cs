using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingPlatform : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private GameObject hitbox;
    [SerializeField] private float duration;
    private float t = 0;
    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        hitbox.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > duration)
        {
            sr.color = originalColor;
            hitbox.SetActive(false);
            t = 0;
        }
    }

    public void EnableDamage()
    {
        t = 0;
        sr.color = Color.red;
        hitbox.SetActive(true);

    }
}
