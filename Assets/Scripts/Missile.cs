using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    //Randomly "burst out" in random directions before targeting something
    public Transform target;
    public float speed;
    public float timeToActivate;
    public float rotationSpeed;
    public float lifetime;
    public float lowerRandom;
    public float upperRandom;

    private Rigidbody2D rb2d;
    private float time;
    private bool launched;

    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        //float randomRotation = Random.rotation.z;
        Vector2 randomVelocity = new Vector2(Random.Range(lowerRandom, upperRandom), Random.Range(lowerRandom, upperRandom));
        rb2d.AddForce(randomVelocity);
    }

    private void FixedUpdate()
    {
        // Slerp angle towards target then dont after launching
        Vector3 direction = target.position - transform.position;
        float rotationZ = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90.0f;
        if (!launched)
        {
            if (time >= timeToActivate)
            {
                Debug.Log("launching");
                // Activate missile by giving it a burst of velocity
                rb2d.AddForce(transform.up * speed);
                launched = true;
                time = 0;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, 0.0f, rotationZ), Time.deltaTime * rotationSpeed);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (launched)
        {
            
            if (time >= lifetime)
            {
                Destroy(gameObject);
            }
        }

    }
}
