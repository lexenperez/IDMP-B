using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss2Health : MonoBehaviour
{
    [SerializeField] float Health1, Health2;
    [SerializeField] Slider slider1, slider2;
    private ManageBoss2 bosses;
    
    // Start is called before the first frame update
    void Start()
    { 
        bosses = GameObject.FindGameObjectWithTag("Boss2").GetComponent<ManageBoss2>();
        Health1 = bosses.boss1Health / bosses.totalHealth;
        Health2 = bosses.boss2Health / bosses.totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider1.value = Health1;
        slider2.value = Health2;

        if (Input.GetKeyDown(KeyCode.G))
        {
            Health1 -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Health2 -= 0.1f;
        }
    }
}
