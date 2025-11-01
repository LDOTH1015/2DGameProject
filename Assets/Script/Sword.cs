using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask enemyLayers; // Enemy 레이어만 체크

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Enemy 레이어가 아니면 무시
        if ((enemyLayers.value & (1 << col.gameObject.layer)) == 0)
            return;

        // 적만 통과했으므로 IDamageable 호출
        if (col.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
        }
    }
}
