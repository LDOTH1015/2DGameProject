using System.Collections;
using UnityEngine;

public class Bag : Monster
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeInterval = 3f;   // n초마다 한 번 시도
    [SerializeField] private float windupTime = 0.5f;     // 돌진 0.5초 전(준비 시간)
    [SerializeField] private float chargeDistance = 5f;   // 직선으로 이동할 거리
    [SerializeField] private float chargeSpeed = 12f;     // 돌진 속도

    private Rigidbody2D _rb;          // 자체 참조(부모의 rb는 private이므로 별도 보관)
    private float _cooldown = 0f;     // 다음 돌진까지 남은 시간
    private bool _isWindup = false;   // 준비 중
    private bool _isCharging = false; // 돌진 중

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _cooldown = chargeInterval;
    }

    // 기본 근접공격 로직을 쓰지 않고, '돌진' 전용 로직으로 교체
    protected override void Update()
    {
        // 플레이어 탐색(부모의 SearchPlayer는 private이라 동일 로직을 여기서 수행)
        if (targetTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist <= playerDetectRange)
                {
                    // 부모의 target/targetTransform은 각각 private/protected
                    // targetTransform은 protected라 접근 가능
                    targetTransform = player.transform;
                }
            }
        }

        // 준비/돌진 중이면 외부 입력(추적) 끔
        if (_isWindup || _isCharging)
        {
            return;
        }

        // 쿨타임 감소
        if (_cooldown > 0f)
        {
            _cooldown -= Time.deltaTime;
        }

        // 쿨이 끝났고, 타겟이 있으면 돌진 루틴 시작
        if (_cooldown <= 0f && targetTransform != null)
        {
            StartCoroutine(WindupAndCharge());
            return;
        }

        // 평소에는 플레이어를 향해 걷기(부모 MoveToTarget은 private이라 동일 로직 구현)
        if (targetTransform != null)
        {
            Vector2 dir = ((Vector2)targetTransform.position - (Vector2)transform.position).normalized;
            _rb.velocity = dir * moveSpeed;
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator WindupAndCharge()
    {
        _isWindup = true;
        _rb.velocity = Vector2.zero;

        // 0.5초 전의 플레이어 위치 스냅샷
        Vector2 snapshotPlayerPos = targetTransform != null
            ? (Vector2)targetTransform.position
            : (Vector2)transform.position;

        // 준비 시간(이때 애니메이션/사운드/이펙트 넣으면 좋음)
        yield return new WaitForSeconds(windupTime);

        _isWindup = false;

        // 스냅샷 지점까지의 방향을 '단 하나'로 고정
        Vector2 dir = (snapshotPlayerPos - (Vector2)transform.position).normalized;
        if (dir.sqrMagnitude < 0.0001f)
        {
            // 같은 자리거나 너무 가까우면 임의의 오른쪽 방향
            dir = Vector2.right;
        }

        _isCharging = true;

        // 지정 거리만큼만 직선 이동 (물리 프레임마다 MovePosition으로 정확한 거리 관리)
        float remaining = chargeDistance;
        while (remaining > 0f)
        {
            float step = chargeSpeed * Time.deltaTime;
            if (step > remaining) step = remaining;

            Vector2 delta = dir * step;
            _rb.MovePosition(_rb.position + delta);

            remaining -= step;
            yield return null;
        }

        // 돌진 종료
        _rb.velocity = Vector2.zero;
        _isCharging = false;
        _cooldown = chargeInterval;
    }

    // (선택) 에디터에서 돌진 거리 가시화
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (_isCharging || _isWindup) return;

        Gizmos.color = Color.red;
        if (targetTransform != null)
        {
            // 현재 기준, 스냅샷이 아닌 '지금' 방향으로 미리보기 라인
            Vector2 dir = ((Vector2)targetTransform.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + dir * chargeDistance);
        }
    }
}
