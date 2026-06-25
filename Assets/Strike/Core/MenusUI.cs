using UnityEngine;

public class MenusUI : MonoBehaviour
{
    [SerializeField] private bool startInInitialMenu = false;
    [SerializeField] private GameObject initialMenu;
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
    [Header("Menu Sounds")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
	private AudioSource menuAudioSource;
    private EButtonFunctions requestedButtonFunction;

    void Awake()
    {
		menuAudioSource = GetComponent<AudioSource>();
        parentMenu = null;
        ingameEndRoundButton.SetActive(false);
        if (startInInitialMenu)
        {
            currentMenu = initialMenu;
            ToggleInitialMenuVisibility(true);
            ToggleMainMenuVisibility(false);
        }
        else
        {
            currentMenu = mainMenu;
            ToggleMainMenuVisibility(true);
            ToggleInitialMenuVisibility(false);
        }
        ToggleEnterStageSelectMenuVisibility(false);
        ToggleSettingsMenuVisibility(false);
        ToggleCircuitMenuVisibility(false);
        ToggleConfirmationMenuVisibility(false);
    }

    void Start()
    {
        FadeManager.Instance.FadeFromBlack();
    }
    //--- Event Bindings ---
    private void OnEnable()
    {
        GameManager.OnEndCircuit += GameObjectOnEndCircuit;
        GameManager.OnEndRound += GameManagerOnEndRound;
        if (FadeManager.Instance != null)
            FadeManager.Instance.OnFadeToBlackComplete += HandleFadeComplete;
    }

    private void OnDisable()
    {
        GameManager.OnEndCircuit -= GameObjectOnEndCircuit;
        GameManager.OnEndRound -= GameManagerOnEndRound;
        if (FadeManager.Instance != null)
            FadeManager.Instance.OnFadeToBlackComplete -= HandleFadeComplete;
    }
    private void GameObjectOnEndCircuit()
    {
        EnterPostCircuitMenu();
        Debug.Log("Circuit Complete");
        ingameEndRoundButton.SetActive(false);
    }

    private void HandleFadeComplete()
    {
        if (requestedButtonFunction == EButtonFunctions.Quit)
            GameManager.Instance.QuitGame();
    }
    private void GameManagerOnEndRound()
    {
        //ingameEndRoundButton.SetActive(false);
    }
    
    //--- Toggle Menu Visibility ---
    public void ToggleCurrentMenuVisibility(bool visible) => currentMenu.SetActive(visible);
    public void ToggleInitialMenuVisibility(bool visible) => initialMenu.SetActive(visible);
    public void ToggleMainMenuVisibility(bool visible) => mainMenu.SetActive(visible);
    public void ToggleEnterStageSelectMenuVisibility(bool visible) => stageSelectMenu.SetActive(visible);
    public void TogglePostCircuitMenuVisibility(bool visible) => postCircuitMenu.SetActive(visible);
    public void ToggleSettingsMenuVisibility(bool visible) => settingsMenu.SetActive(visible);
    public void ToggleCircuitMenuVisibility(bool visible) => circuitMenu.SetActive(visible);
    public void ToggleConfirmationMenuVisibility(bool visible) => confirmationMenu.SetActive(visible);
    
    // Button Functionalities for going to menus
    public void GoBackMenu()
    {
        PlayClickSound();
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
        PlayClickSound();
        ToggleCurrentMenuVisibility(false);
        ToggleEnterStageSelectMenuVisibility(true);
        currentMenu = stageSelectMenu;
        parentMenu = mainMenu;
    }
    
    public void EnterSettingsMenu()
    {
        PlayClickSound();
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
        PlayClickSound();
        ToggleCurrentMenuVisibility(false);
        ToggleCircuitMenuVisibility(true);
        currentMenu = circuitMenu;
        parentMenu = stageSelectMenu;
    }

    public void HideAllMenus()
    {
        ToggleInitialMenuVisibility(false);
        ToggleMainMenuVisibility(false);
        ToggleEnterStageSelectMenuVisibility(false);
        ToggleSettingsMenuVisibility(false);
        ToggleCircuitMenuVisibility(false);
        ToggleConfirmationMenuVisibility(false);
    }
    
    //--- Circuit Menu ---
    public void StartCircuit()
    {
        PlayClickSound();
        GameManager.Instance.StartCircuit(true);
        HideAllMenus();
        ingameEndRoundButton.SetActive(true);
    }
   //--- Settings Menu ---
    public void QuitGame(GameObject menuOrigin)
    {
        PlayClickSound();
        ToggleCurrentMenuVisibility(false);
        ToggleConfirmationMenuVisibility(true);
        requestedButtonFunction = EButtonFunctions.Quit;
        currentMenu = confirmationMenu;
        parentMenu = menuOrigin;
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
        PlayClickSound();
        switch (requestedButtonFunction)
        {
            case EButtonFunctions.Default:
                break;
            case EButtonFunctions.Quit:
                if (FadeManager.Instance != null)
                {
                    FadeManager.Instance.FadeToBlack();
                    GameManager.Instance.QuitGame(1f);
                }
                else
                {
                    GameManager.Instance.QuitGame(0f);
                }
                break;
        }
    }
    public void PlayHoverSound() => menuAudioSource.PlayOneShot(hoverSound);
    public void PlayClickSound() => menuAudioSource.PlayOneShot(clickSound);
}
