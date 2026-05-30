using UnityEngine;

public class MenusUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject stageSelectMenu;
    [SerializeField] private GameObject circuitMenu;
    [SerializeField] private GameObject currentMenu;
    [SerializeField] private GameObject parentMenu;

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
    public void GoBackMenu()
    {
        Debug.Log("GoBackMenu");
        ToggleCurrentMenuVisibility(false);
        currentMenu = parentMenu;
        if (currentMenu = mainMenu)
        {
            ToggleMainMenuVisibility(true);
            parentMenu = null;
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
        Debug.Log("QuitGame");
        GameManager.Instance.QuitGame();
    }

    public void SelectStage(int stage)
    {
        int targetStage = stage;
        Debug.Log($"{targetStage}");
    }
}
