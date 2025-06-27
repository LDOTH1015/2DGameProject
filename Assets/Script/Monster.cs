using UnityEngine;

public class Monster :MonoBehaviour, IDamageable
{
    public Vector3 GetPosition()
    {
        throw new System.NotImplementedException();
    }

    public bool IsDeadMan()
    {
        throw new System.NotImplementedException();
    }

    public void KnockBack(float knockbackPower)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDotDamage(float amount, float duration, float interval)
    {
        throw new System.NotImplementedException();
    }
}