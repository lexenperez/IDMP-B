using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingTrap : MonoBehaviour
{
    [System.Serializable]
    public struct RisingParams
    {
        public Transform transform;
        public float time;
        public float delay;


    }

    [Header("Position, Time and Delay for LeanTween")]
    [SerializeField] private RisingParams[] parameters;
    [SerializeField] private bool camShake = true;
    [SerializeField] private float shakeDuration;
    [SerializeField] private Camera cam;
    [SerializeField] private AudioClip lavaSfx;

    public float repeatTimer = 0;

    private AudioSource audioSource;
    public bool start = false;
    private int totalParams = 0;
    private float t = 0;
    private LTSeq sq;
    
    void Start()
    {
        sq = LeanTween.sequence();
        totalParams = parameters.Length;
        for (int i = 0; i < totalParams; i++)
        {
            sq.append(parameters[i].delay);
            if (camShake) sq.append(() => cam.GetComponent<CameraShake>().ScreenShake(shakeDuration));
            sq.append(LeanTween.move(gameObject, parameters[i].transform.position, parameters[i].time));
        }
        // Store and pause the animation
        LeanTween.pause(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            LeanTween.resume(gameObject);
            start = false;
        }

        if (repeatTimer > 0)
        {
            t += Time.deltaTime;
            if (t >= repeatTimer)
            {
                t = 0;
                RefreshParameters();
                ResumeTween();
            }
        }
    }

    public void RefreshParameters()
    {
        sq = LeanTween.sequence();
        totalParams = parameters.Length;
        for (int i = 0; i < totalParams; i++)
        {
            sq.append(parameters[i].delay);
            if (camShake) sq.append(() => cam.GetComponent<CameraShake>().ScreenShake(shakeDuration));
            sq.append(() => audioSource.PlayOneShot(lavaSfx));
            sq.append(LeanTween.move(gameObject, parameters[i].transform.position, parameters[i].time));
        }
    }

    public void AddParameters(RisingParams rs)
    {
        sq.append(rs.delay);
        if (camShake) sq.append(() => cam.GetComponent<CameraShake>().ScreenShake(shakeDuration));
        sq.append(LeanTween.move(gameObject, rs.transform.position, rs.time));
    }

    public void SetStart(bool value)
    {
        start = value;
    }

    public void ResumeTween()
    {
        LeanTween.resume(gameObject);
    }

    public void PauseTween()
    {
        LeanTween.pause(gameObject);
    }
}
