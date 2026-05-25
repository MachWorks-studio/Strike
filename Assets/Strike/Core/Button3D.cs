using UnityEngine;
using UnityEngine.EventSystems;

public class Button3D : MonoBehaviour, IDamageable
{
    [SerializeField] private EButtonFunctions buttonFunction;
    public void TakeDamage(float damage)
    {
        switch (buttonFunction)
        {
            case EButtonFunctions.Default:
                Debug.Log("Default, nothing implemented");
                break;
            case EButtonFunctions.StartRound:
                GameManager.Instance.StartCircuit();
                break;
            case EButtonFunctions.EndRound:
                GameManager.Instance.EndCircuit();
                break;
            case EButtonFunctions.Quit:
                GameManager.Instance.QuitGame();
                break;
        }
    }

    private void OnEnable()
    {
        GameManager.OnStartCircuit += HideButton;
        GameManager.OnEndCircuit += ShowButton;
    }

    public ESurfaceType GetSurfaceType()
    {
        return ESurfaceType.Button;
    }

    public void SetSurfaceType(ESurfaceType type)
    {
        throw new System.NotImplementedException();
    }

    private void HideButton()
    {
        if (buttonFunction == EButtonFunctions.StartRound)
            gameObject.SetActive(false);
    }

    private void ShowButton()
    {
        gameObject.SetActive(true);
    }
}
