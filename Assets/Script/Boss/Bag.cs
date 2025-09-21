using System.Collections;
using UnityEngine;

public class BagMonster : Monster
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeInterval = 3f;   // n�ʸ��� ����
    [SerializeField] private float windupTime = 0.5f;     // ���� �غ� �ð�(������ ���� �ð�)
    [SerializeField] private float chargeDistance = 6f;   // ���� �Ÿ�
    [SerializeField] private float chargeSpeed = 14f;     // ���� �ӵ�

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;  // �ٶ󺸴� ���� ��ȯ��
    [SerializeField] private RangeBag indicator;      // ���� �ڽĿ� �� IndicatorRoot

    private float cooldown;
    private bool isWindup = false;
    private bool isCharging = false;

    protected override void Awake()
    {
        base.Awake();
        cooldown = chargeInterval;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        // �÷��̾� Ž��
        SearchPlayer();
        FaceTarget();

        if (isWindup || isCharging) return;

        if (cooldown > 0f)
            cooldown -= Time.deltaTime;

        if (cooldown <= 0f && targetTransform != null)
        {
            StartCoroutine(WindupAndCharge());
            return;
        }

        if (targetTransform != null)
            MoveToTarget();
        else
            rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// �÷��̾� ��ġ�� �ٶ󺸰� flip ó��
    /// (�⺻ ��������Ʈ�� �� �����̶�� ����)
    /// </summary>
    private void FaceTarget()
    {
        if (spriteRenderer == null || targetTransform == null) return;

        Vector2 toTarget = (Vector2)targetTransform.position - (Vector2)transform.position;
        if (toTarget.sqrMagnitude < 0.0001f) return;

        spriteRenderer.flipX = toTarget.x > 0f;
        spriteRenderer.flipY = toTarget.y > 0f;
    }

    private IEnumerator WindupAndCharge()
    {
        isWindup = true;
        rb.velocity = Vector2.zero;

        // ������ ���� (���� �� �÷��̾�)
        Vector2 snapshotPos = targetTransform != null
            ? (Vector2)targetTransform.position
            : (Vector2)transform.position;
        Vector2 dir = (snapshotPos - (Vector2)transform.position).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.down;

        // === �ε������� ǥ�� ===
        if (indicator != null)
        {
            indicator.gameObject.SetActive(true);
            indicator.Show(dir, windupTime);
        }

        // �غ� �ð� ���� ������ ������
        yield return new WaitForSeconds(windupTime);

        if (indicator != null) indicator.HideImmediate();

        isWindup = false;
        isCharging = true;

        // === ���� ���� ===
        float remaining = chargeDistance;
        while (remaining > 0f)
        {
            float step = chargeSpeed * Time.deltaTime;
            if (step > remaining) step = remaining;

            rb.MovePosition(rb.position + dir * step);
            remaining -= step;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isCharging = false;
        cooldown = chargeInterval;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging) return;

        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<TopDownMovement>();
            if (player != null)
                player.TakeDamage(attackDamage);
            // �÷��̾�� �°� ��� ����
            return;
        }

        if (collision.collider.CompareTag("Wall"))
        {
            // �� �浹 �� ���� ����
            isCharging = false;
            rb.velocity = Vector2.zero;
            cooldown = chargeInterval;

            if (indicator != null) indicator.HideImmediate();
        }
    }
}
