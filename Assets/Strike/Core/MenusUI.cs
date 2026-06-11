using UnityEngine;

public class MenusUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject stageSelectMenu;
    [SerializeField] private GameObject circuitMenu;
    [SerializeField] private GameObject postCircuitMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject confirmationMenu;
    [Header("Menu Backend")]
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject parentMenu;
    [Header("Game Object References")]
    [SerializeField] private GameObject ingameEndRoundButton;
    private EButtonFunctions requestedButtonFunction;

    void Awake()
    {
        currentMenu = mainMenu;
        parentMenu = null;
        ingameEndRoundButton.SetActive(false);
        ToggleMainMenuVisibility(true);
        ToggleEnterStageSelectMenuVisibility(false);
        ToggleSettingsMenuVisibility(false);
        ToggleCircuitMenuVisibility(false);
        ToggleConfirmationMenuVisibility(false);
    }
    //--- Event Bindings ---
    private void OnEnable()
    {
        GameManager.OnEndCircuit += GameObjectOnEndCircuit;
        GameManager.OnEndRound += GameManagerOnEndRound;
    }
    private void OnDisable()
    {
        GameManager.OnEndCircuit -= GameObjectOnEndCircuit;
        GameManager.OnEndRound -= GameManagerOnEndRound;
    }
    private void GameObjectOnEndCircuit()
    {
        EnterPostCircuitMenu();
        Debug.Log("Circuit Complete");
        ingameEndRoundButton.SetActive(false);
    }
    private void GameManagerOnEndRound()
    {
        ingameEndRoundButton.SetActive(false);
    }

    //--- Toggle Menu Visibility ---
    public void ToggleCurrentMenuVisibility(bool visible) => currentMenu.SetActive(visible);
    public void ToggleMainMenuVisibility(bool visible) => mainMenu.SetActive(visible);
    public void ToggleEnterStageSelectMenuVisibility(bool visible) => stageSelectMenu.SetActive(visible);
    public void TogglePostCircuitMenuVisibility(bool visible) => postCircuitMenu.SetActive(visible);
    public void ToggleSettingsMenuVisibility(bool visible) => settingsMenu.SetActive(visible);
    public void ToggleCircuitMenuVisibility(bool visible) => circuitMenu.SetActive(visible);
    public void ToggleConfirmationMenuVisibility(bool visible) => confirmationMenu.SetActive(visible);
    
    // Button Functionalities for going to menus
    public void GoBackMenu()
    {
        ToggleCurrentMenuVisibility(false);
        currentMenu = parentMenu;
        if (currentMenu == mainMenu)
        {
            parentMenu = null;
            requestedButtonFunction = EButtonFunctions.Default;
            ToggleMainMenuVisibility(true);
        }

        if (parentMenu == currentMenu)
        {
            if (currentMenu == stageSelectMenu)
                parentMenu = mainMenu;
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
        parentMenu = mainMenu;
    }

    public void EnterPostCircuitMenu()
    {
        currentMenu = postCircuitMenu;
        parentMenu = stageSelectMenu;
        TogglePostCircuitMenuVisibility(true);
    }

    public void EnterCircuitMenu()
    {
        ToggleCurrentMenuVisibility(false);
        ToggleCircuitMenuVisibility(true);
        currentMenu = circuitMenu;
        parentMenu = stageSelectMenu;
    }

    public void HideAllMenus()
    {
        ToggleMainMenuVisibility(false);
        ToggleEnterStageSelectMenuVisibility(false);
        ToggleSettingsMenuVisibility(false);
        ToggleCircuitMenuVisibility(false);
        ToggleConfirmationMenuVisibility(false);
    }
    
    //--- Circuit Menu ---
    public void StartCircuit()
    {
        //Debug.Log("StartCircuit");
        GameManager.Instance.StartCircuit();
        HideAllMenus();
        ingameEndRoundButton.SetActive(true);
    }

    public void EndCircuit()
    {
        Debug.Log("EndCircuit");
        GameManager.Instance.EndCircuit();
        ingameEndRoundButton.SetActive(false);
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
        GameManager.Instance.InitializeStageData(targetStage);
        EnterCircuitMenu();
    }

    public void Confirm()
    {
        switch (requestedButtonFunction)
        {
            case EButtonFunctions.Default:
                break;
            case EButtonFunctions.Quit:
                GameManager.Instance.QuitGame();
                break;
        }
    }
}
