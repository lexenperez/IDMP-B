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
    // Start is called before the first frame update
    void Awake()
    {
        base.Init();
        Collider = base.baseCollider;
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
        audioSource.PlayOneShot(dash);
    }

    public void SpinUp()
    {
        audioSource.PlayOneShot(chargeup);
    }

    public void Beyblade()
    {
        audioSource.PlayOneShot(spin);
    }

    public void Circle()
    {
        audioSource.PlayOneShot(circle);
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public void Explode()
    {
        audioSource.PlayOneShot(explode);
    }

    public bool AudioTest()
    {
        return audioSource.isPlaying;
    }
}
