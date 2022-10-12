using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBossBar : MonoBehaviour
{
    public Image HPBar;
    public NPC npc;

    public void UpdateHealthBar()
    {
        HPBar.fillAmount = Mathf.Clamp(npc.hp / npc.maxHp, 0, 1f);
    }
    // Start is called before the first frame update
    void Start()
    {
        //HPBar.type = Image.Type.Filled;
        //HPBar.fillMethod = Image.FillMethod.Horizontal;
        //HPBar.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
