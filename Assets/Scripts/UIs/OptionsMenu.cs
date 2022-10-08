using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static OptionsMenu;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] private GameObject controlsMenuUI;
    [SerializeField] private GameObject gameplayMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        toggle.SetIsOnWithoutNotify(GameManager.isPlayerInvincible);
    }

    public void ViewControls()
    {
        optionsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(true);
    }

    public void ViewGameplay()
    {
        optionsMenuUI.SetActive(false);
        gameplayMenuUI.SetActive(true);
    }

    public void GoBack()
    {
        if (controlsMenuUI.activeSelf)
            controlsMenuUI.SetActive(false);

        else if (gameplayMenuUI.activeSelf)
            gameplayMenuUI.SetActive(false);

        optionsMenuUI.SetActive(true);
    }

    public void SetValueToGameManager()
    {
        GameManager.isPlayerInvincible = toggle.isOn;
    }


}
