using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button3D : MonoBehaviour, IDamageable
{
    [SerializeField] private EButtonFunctions buttonFunction;
    private bool buttonIsReady = true;
    public void TakeDamage(float damage)
    {
        if (buttonIsReady == false)
            return;
        switch (buttonFunction)
        {
            case EButtonFunctions.Default:
                Debug.Log("Default, nothing implemented");
                break;
            case EButtonFunctions.StartRound:
                GameManager.Instance.StartCircuit(true);
                break;
            case EButtonFunctions.EndRound:
                GameManager.Instance.EndCircuit();
                break;
            case EButtonFunctions.Quit:
                GameManager.Instance.QuitGame();
                break;
        }
    }
    /* - Deprecated, now we use Menu scripts to handle that
    private void OnEnable()
    {
        GameManager.OnStartCircuit += HideButton;
        GameManager.OnEndCircuit += ShowButton;
    }
    */
    IEnumerator ButtonReadyTimer()
    {
        buttonIsReady = false;
        yield return new WaitForSeconds(0.5f);
        buttonIsReady = true;
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
