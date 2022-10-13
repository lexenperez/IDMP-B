using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSfx : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip onFireSfx;
    //[SerializeField] private float offset;
    public float timeToActivate = 0;
    private float time = 0;
    private bool fireSfxSent = false;
    private bool chargeSfxSent = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!fireSfxSent)
        {
            time += Time.deltaTime;
        }
        
        if (time >= timeToActivate - audioSource.clip.length)
        {
            if (!chargeSfxSent)
            {
                audioSource.Play();
                chargeSfxSent = true;
            }

        }

        if (time >= timeToActivate)
        {
            audioSource.PlayOneShot(onFireSfx);
            fireSfxSent = true;
            time = -100;
        }

        if (fireSfxSent)
        {
            if (!audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
        }


    }

}
