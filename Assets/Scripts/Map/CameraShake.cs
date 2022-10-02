using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Matt Buckley https://medium.com/nice-things-ios-android-development/basic-2d-screen-shake-in-unity-9c27b56b516
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float noiseMagnitude;
    [SerializeField] private float noiseAmplitude;
    [SerializeField] private float dampingSpeed;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera vCam;
    private Cinemachine.CinemachineBasicMultiChannelPerlin noise;
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (vCam) noise = vCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
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
        if (vCam)
        {
            StartCoroutine(vCamShake(duration));
        }
    }

    private IEnumerator vCamShake(float duration)
    {
        Noise(noiseAmplitude, noiseMagnitude);
        yield return new WaitForSeconds(duration);
        Noise(0, 0);
    }

    private void Noise(float ampGain, float freqGain)
    {
        noise.m_AmplitudeGain = ampGain;
        noise.m_FrequencyGain = freqGain;
    }
}
