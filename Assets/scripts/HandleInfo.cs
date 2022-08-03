using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleInfo : MonoBehaviour
{
    public Slider health, mana;
    playerInfo Player;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        health.value = Player.health/100;
        mana.value = Player.mana/100;
    }
}
