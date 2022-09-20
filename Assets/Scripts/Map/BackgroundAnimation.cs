using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    private Renderer rend;
    private Vector3 offset = Vector2.zero;
    [SerializeField] private float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        offset.x += Time.deltaTime * speed;
        rend.material.mainTextureOffset = offset;
        if (offset.x > 360.0f)
        {
            offset.x = 0;
        }
    }
}
