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

    public ESurfaceType GetSurfaceType()
    {
        return ESurfaceType.Button;
    }

    public void SetSurfaceType(ESurfaceType type)
    {
        throw new System.NotImplementedException();
    }
}
