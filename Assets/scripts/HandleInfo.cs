using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleInfo : MonoBehaviour
{
    public Slider mana;
    playerInfo Player;
    int current_health, total_health;
    public GameObject health_prefab;
    public GameObject health_game_object;
    List<GameObject> pips = new List<GameObject>();
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerInfo>();
        for(int i = 0; i < Player.GetHealth(); i++)
        {
            GameObject newpip = Instantiate(health_prefab, health_game_object.transform);
            pips.Add(newpip);
            newpip.transform.position += new Vector3(i * 60, 0, 0);
        }
        total_health = Player.GetHealth();
        current_health = total_health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.GetHealth() < current_health)
        {
            pips[current_health - 1].GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            current_health = Player.GetHealth();
        }

        //mana.value = Player.GetMana()/100;
    }
}
