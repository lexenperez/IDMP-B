using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : Enemy
{
    public GameObject Head;
    public BoxCollider2D Collider;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip spin;
    [SerializeField] private AudioClip circle;
    [SerializeField] private AudioClip chargeup;
    [SerializeField] private AudioClip explode;
    [SerializeField] private AudioClip deflect;
    [SerializeField] private AudioSource secondaudio;
    // Start is called before the first frame update
    void Awake()
    {
        base.Init();
        Collider = (BoxCollider2D)base.baseCollider;
    }

    // Update is called once per frame
    void Update()
    {
        Head.transform.position = transform.position;
        Head.transform.eulerAngles = transform.rotation.eulerAngles;
        Head.transform.position -= transform.right * 1.76f;
    }

    public void DashSound()
    {
        secondaudio.PlayOneShot(dash);
    }

    public void SpinUp()
    {
        secondaudio.PlayOneShot(chargeup);
    }

    public void Beyblade()
    {
        secondaudio.PlayOneShot(spin);
    }

    public void Circle()
    {
        secondaudio.PlayOneShot(circle);
    }

    public void StopAudio()
    {
        secondaudio.Stop();
    }

    public void Explode()
    {
        secondaudio.PlayOneShot(explode);
    }

    public bool AudioTest()
    {
        return secondaudio.isPlaying;
    }

    public void DestroyHead()
    {
        Destroy(Head.gameObject);
    }

    public void Deflect()
    {
        isInvincible = true;
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(deflect);
        }
    }

    public void UnDeflect()
    {
        isInvincible = false;
    }
}
