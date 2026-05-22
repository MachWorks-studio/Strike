using UnityEngine;
using UnityEngine.EventSystems;

public class Button3D : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        GameManager.Instance.StartRound();
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
