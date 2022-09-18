using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : Enemy
{
    [SerializeField] GameObject Head;
    // Start is called before the first frame update
    void Start()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Head.transform.position = transform.position;
        Head.transform.eulerAngles = transform.rotation.eulerAngles;
        Head.transform.position -= transform.right * 1.76f;
    }
}
