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
    public bool start = false;
    private int totalParams = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            LeanTween.resume(gameObject);
            start = false;
        }
    }
}
