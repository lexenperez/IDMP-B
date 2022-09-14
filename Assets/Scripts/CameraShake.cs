using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Matt Buckley https://medium.com/nice-things-ios-android-development/basic-2d-screen-shake-in-unity-9c27b56b516
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float dampingSpeed;
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0;
            transform.localPosition = initialPosition;
        }
    }

    public void ScreenShake(float duration)
    {
        shakeDuration = duration;
    }
}
