using UnityEngine;

public class MenusUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject stageSelectMenu;
    [SerializeField] private GameObject circuitMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject confirmationMenu;
    [Header("Menu Backend")]
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject parentMenu;
    private EButtonFunctions requestedButtonFunction;

    void Awake()
    {
        currentMenu = mainMenu;
        ToggleMainMenuVisibility(true);
        ToggleEnterStageSelectMenuVisibility(false);
        ToggleSettingsMenuVisibility(false);
        ToggleConfirmationMenuVisibility(false);
    }
    
    //--- Toggle Menu Visibility ---
    public void ToggleCurrentMenuVisibility(bool visible)
    {
        currentMenu.SetActive(visible);
    }
    public void ToggleMainMenuVisibility(bool visible)
    {
        mainMenu.SetActive(visible);
    }

    public void ToggleEnterStageSelectMenuVisibility(bool visible)
    {
        stageSelectMenu.SetActive(visible);
    }

    public void ToggleSettingsMenuVisibility(bool visible)
    {
        settingsMenu.SetActive(visible);
    }
    public void ToggleConfirmationMenuVisibility(bool visible)
    {
        confirmationMenu.SetActive(visible);
    }
    // Button Functionalities for going to menus
    public void GoBackMenu()
    {
        ToggleCurrentMenuVisibility(false);
        currentMenu = parentMenu;
        if (currentMenu = mainMenu)
        {
            parentMenu = null;
            requestedButtonFunction = EButtonFunctions.Default;
            ToggleMainMenuVisibility(true);
        }
        ToggleCurrentMenuVisibility(true);
    }
    public void EnterStageSelectMenu()
    {
        ToggleCurrentMenuVisibility(false);
        ToggleEnterStageSelectMenuVisibility(true);
        currentMenu = stageSelectMenu;
        parentMenu = mainMenu;
    }
    
    public void EnterSettingsMenu()
    {
        ToggleCurrentMenuVisibility(false);
        ToggleSettingsMenuVisibility(true);
        currentMenu = settingsMenu;
        parentMenu = settingsMenu;
    }
    
    //--- Circuit Menu ---
    public void StartCircuit()
    {
        //Debug.Log("StartCircuit");
        GameManager.Instance.StartCircuit();
    }

    public void EndCircuit()
    {
        Debug.Log("EndCircuit");
        GameManager.Instance.EndCircuit();
    }
   //--- Settings Menu ---
    public void QuitGame()
    {
        ToggleCurrentMenuVisibility(false);
        ToggleConfirmationMenuVisibility(true);
        requestedButtonFunction = EButtonFunctions.Quit;
        currentMenu = confirmationMenu;
        parentMenu = mainMenu;
    }
    
    //--- Stage Menu ---
    public void SelectStage(int stage)
    {
        int targetStage = stage;
        Debug.Log($"Stage {targetStage} not implemented");
    }

    public void Confirm()
    {
        switch (requestedButtonFunction)
        {
            case EButtonFunctions.Default:
                break;
            case EButtonFunctions.Quit:
                Debug.Log("QuitGame");
                GameManager.Instance.QuitGame();
                break;
        }
    }
}
