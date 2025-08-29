using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damage = 20f;     // ��������
    [SerializeField] private float radius = 3f;      // ���� ����

    private void Start()
    {
        // ���� ���� (0.5�� ��)
        Invoke(nameof(Explode), 0.55f);
        // �ڱ� �ڽ� �ı� (0.55�� ��)
        Destroy(gameObject, 1.1f);
    }

    private void Explode()
    {
        // ���� ���� ���� ��� Collider2D Ž��
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // Player�� �������� ���� �� �ֵ��� IDamageable �������̽� �Ǵ� Player ��ũ��Ʈ ȣ��
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
        // Scene �信�� ���� ������ �ð������� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
