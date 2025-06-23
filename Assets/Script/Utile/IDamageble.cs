using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
    void KnockBack(float knockbackPower);
    void TakeDotDamage(float amount, float duration, float interval);

    bool IsDeadMan();
    
    Vector3 GetPosition();
}
