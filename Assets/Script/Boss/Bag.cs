using System.Collections;
using UnityEngine;

public class Bag : Monster
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeInterval = 3f;   // n�ʸ��� �� �� �õ�
    [SerializeField] private float windupTime = 0.5f;     // ���� 0.5�� ��(�غ� �ð�)
    [SerializeField] private float chargeDistance = 5f;   // �������� �̵��� �Ÿ�
    [SerializeField] private float chargeSpeed = 12f;     // ���� �ӵ�

    private Rigidbody2D _rb;          // ��ü ����(�θ��� rb�� private�̹Ƿ� ���� ����)
    private float _cooldown = 0f;     // ���� �������� ���� �ð�
    private bool _isWindup = false;   // �غ� ��
    private bool _isCharging = false; // ���� ��

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _cooldown = chargeInterval;
    }

    // �⺻ �������� ������ ���� �ʰ�, '����' ���� �������� ��ü
    protected override void Update()
    {
        // �÷��̾� Ž��(�θ��� SearchPlayer�� private�̶� ���� ������ ���⼭ ����)
        if (targetTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist <= playerDetectRange)
                {
                    // �θ��� target/targetTransform�� ���� private/protected
                    // targetTransform�� protected�� ���� ����
                    targetTransform = player.transform;
                }
            }
        }

        // �غ�/���� ���̸� �ܺ� �Է�(����) ��
        if (_isWindup || _isCharging)
        {
            return;
        }

        // ��Ÿ�� ����
        if (_cooldown > 0f)
        {
            _cooldown -= Time.deltaTime;
        }

        // ���� ������, Ÿ���� ������ ���� ��ƾ ����
        if (_cooldown <= 0f && targetTransform != null)
        {
            StartCoroutine(WindupAndCharge());
            return;
        }

        // ��ҿ��� �÷��̾ ���� �ȱ�(�θ� MoveToTarget�� private�̶� ���� ���� ����)
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

        // 0.5�� ���� �÷��̾� ��ġ ������
        Vector2 snapshotPlayerPos = targetTransform != null
            ? (Vector2)targetTransform.position
            : (Vector2)transform.position;

        // �غ� �ð�(�̶� �ִϸ��̼�/����/����Ʈ ������ ����)
        yield return new WaitForSeconds(windupTime);

        _isWindup = false;

        // ������ ���������� ������ '�� �ϳ�'�� ����
        Vector2 dir = (snapshotPlayerPos - (Vector2)transform.position).normalized;
        if (dir.sqrMagnitude < 0.0001f)
        {
            // ���� �ڸ��ų� �ʹ� ������ ������ ������ ����
            dir = Vector2.right;
        }

        _isCharging = true;

        // ���� �Ÿ���ŭ�� ���� �̵� (���� �����Ӹ��� MovePosition���� ��Ȯ�� �Ÿ� ����)
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

        // ���� ����
        _rb.velocity = Vector2.zero;
        _isCharging = false;
        _cooldown = chargeInterval;
    }

    // (����) �����Ϳ��� ���� �Ÿ� ����ȭ
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (_isCharging || _isWindup) return;

        Gizmos.color = Color.red;
        if (targetTransform != null)
        {
            // ���� ����, �������� �ƴ� '����' �������� �̸����� ����
            Vector2 dir = ((Vector2)targetTransform.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + dir * chargeDistance);
        }
    }
}
