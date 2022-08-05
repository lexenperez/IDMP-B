using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class playerInfo : MonoBehaviour
{

    static float mana = 0;
    static int health = 3;
    float elapsedTime, timer = 0.2f;
    float healthTimer = -0.1f;

    public PostProcessVolume ppv;
    ChromaticAberration ca;
    Vignette vign;

    //post processing efects
    void Start()
    {
        ppv.profile.TryGetSettings(out ca);
        ppv.profile.TryGetSettings(out vign);
    }

    // Update is called once per frame
    void Update()
    {
        //slowly regen mana
        if (mana < 100)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= timer)
            {
                elapsedTime = 0;
                mana++;
            }
        }

        //simulate taken damage
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            health -= 1;
            healthTimer = 0;
            ca.intensity.value = 1.0f;
            vign.intensity.value = 0.5f;
        }

        //slow time when damage taken timer
        if (healthTimer > 0.15)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
            healthTimer = -0.1f;
            ca.intensity.value = 0.0f;
            vign.intensity.value = 0.0f;
        }
        else if (healthTimer < 0)
        {
            //do nothing
        }
        //add to time and reduce chromatic abb exponentially
        else
        {
            ca.intensity.value = 1.0f - (healthTimer * healthTimer * healthTimer * healthTimer * 1975.30f);
            vign.intensity.value = 0.5f - (healthTimer * healthTimer * healthTimer * healthTimer * 987.654f);
            healthTimer += Time.deltaTime;
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetMana()
    {
        return mana;
    }
}
