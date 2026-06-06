using UnityEngine;

public class MenusUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject stageSelectMenu;
    [SerializeField] private GameObject circuitMenu;
    [SerializeField] private GameObject confirmationMenu;
    [Header("Menu Backend")]
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject parentMenu;
    private EButtonFunctions requestedButtonFunction;

    void Awake()
    {
        currentMenu = mainMenu;
        ToggleMainMenuVisibility(true);
        ToggleEnterStageSelectMenu(false);
    }
    public void ToggleMainMenuVisibility(bool visible)
    {
        mainMenu.SetActive(visible);
    }

    public void ToggleEnterStageSelectMenu(bool visible)
    {
        stageSelectMenu.SetActive(visible);
    }

    public void ToggleCurrentMenuVisibility(bool visible)
    {
        currentMenu.SetActive(visible);
    }

    public void ToggleConfirmationMenuVisibility(bool visible)
    {
        confirmationMenu.SetActive(visible);
    }
    public void GoBackMenu()
    {
        Debug.Log("GoBackMenu");
        ToggleCurrentMenuVisibility(false);
        currentMenu = parentMenu;
        if (currentMenu = mainMenu)
        {
            ToggleMainMenuVisibility(true);
            parentMenu = null;
            requestedButtonFunction = EButtonFunctions.Default;
        }
        ToggleCurrentMenuVisibility(true);
    }
    public void EnterStageSelectMenu()
    {
        Debug.Log("Enter Stage Select Menu");
        ToggleCurrentMenuVisibility(false);
        currentMenu = stageSelectMenu;
        parentMenu = mainMenu;
        ToggleEnterStageSelectMenu(true);
    }
    // Stage Menu
    
    // Circuit Menu
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

    public void QuitGame()
    {
        ToggleCurrentMenuVisibility(false);
        requestedButtonFunction = EButtonFunctions.Quit;
        currentMenu = confirmationMenu;
        parentMenu = mainMenu;
        ToggleConfirmationMenuVisibility(true);
    }

    public void SelectStage(int stage)
    {
        int targetStage = stage;
        Debug.Log($"{targetStage}");
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
