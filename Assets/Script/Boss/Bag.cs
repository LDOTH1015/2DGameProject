using System.Collections;
using UnityEngine;

public class BagMonster : Monster
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeInterval = 3f;   // n초마다 돌진
    [SerializeField] private float windupTime = 0.5f;     // 돌진 준비 시간(게이지 차는 시간)
    [SerializeField] private float chargeDistance = 6f;   // 돌진 거리
    [SerializeField] private float chargeSpeed = 14f;     // 돌진 속도

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;  // 바라보는 방향 전환용
    [SerializeField] private RangeBag indicator;      // 가방 자식에 둔 IndicatorRoot

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
        // 플레이어 탐색
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
    /// 플레이어 위치를 바라보게 flip 처리
    /// (기본 스프라이트가 ↙ 기준이라고 가정)
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

        // 스냅샷 방향 (가방 → 플레이어)
        Vector2 snapshotPos = targetTransform != null
            ? (Vector2)targetTransform.position
            : (Vector2)transform.position;
        Vector2 dir = (snapshotPos - (Vector2)transform.position).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.down;

        // === 인디케이터 표시 ===
        if (indicator != null)
        {
            indicator.gameObject.SetActive(true);
            indicator.Show(dir, windupTime);
        }

        // 준비 시간 동안 게이지 차오름
        yield return new WaitForSeconds(windupTime);

        if (indicator != null) indicator.HideImmediate();

        isWindup = false;
        isCharging = true;

        // === 실제 돌진 ===
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
            // 플레이어는 맞고 계속 돌진
            return;
        }

        if (collision.collider.CompareTag("Wall"))
        {
            // 벽 충돌 시 돌진 종료
            isCharging = false;
            rb.velocity = Vector2.zero;
            cooldown = chargeInterval;

            if (indicator != null) indicator.HideImmediate();
        }
    }
}
