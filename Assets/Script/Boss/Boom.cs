using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damage = 20f;     // 데미지량
    [SerializeField] private float radius = 3f;      // 폭발 범위

    private void Start()
    {
        // 폭발 실행 (0.5초 후)
        Invoke(nameof(Explode), 0.55f);
        // 자기 자신 파괴 (0.55초 후)
        Destroy(gameObject, 1.1f);
    }

    private void Explode()
    {
        // 지정 범위 내의 모든 Collider2D 탐색
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // Player가 데미지를 받을 수 있도록 IDamageable 인터페이스 또는 Player 스크립트 호출
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Scene 뷰에서 폭발 범위를 시각적으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
