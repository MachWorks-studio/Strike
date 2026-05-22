using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
    ESurfaceType GetSurfaceType();
    void SetSurfaceType(ESurfaceType type);
}
