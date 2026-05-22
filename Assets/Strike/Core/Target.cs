using UnityEngine;
using System;

public class Target : MonoBehaviour, IDamageable
{
    public event Action OnTargetHit;

    void Start()
    {
        
    }
    public void TakeDamage(float damage)
    {
        TargetHit();
    }
    void TargetHit()
    {
        OnTargetHit?.Invoke();
        Destroy(gameObject);
    }

    public ESurfaceType GetSurfaceType()
    {
        return ESurfaceType.Target;
    }

    public void SetSurfaceType(ESurfaceType type)
    {
        type = ESurfaceType.Target;
    }
}
