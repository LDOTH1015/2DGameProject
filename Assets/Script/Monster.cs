using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float playerDetectRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackDelay = 1f;

    private float HP = 100f;
    private GameObject target;
    private Transform targetTransform;
    private Rigidbody2D rb;

    private bool canAttack = true;
    private bool isInAttackRange = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    internal void Initialize(GameObject target)
    {
        HP = 100f;
        this.target = target;
        targetTransform = target.transform;
    }

    private void Update()
    {
        SearchPlayer();

        if (targetTransform != null)
        {
            float distance = Vector2.Distance(transform.position, targetTransform.position);

            if (distance <= attackRange)
            {
                rb.velocity = Vector2.zero;
                if (canAttack)
                {
                    StartCoroutine(Attack());
                }
            }
            else
            {
                MoveToTarget();
            }
        }
    }

    private void SearchPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= playerDetectRange)
            {
                target = player;
                targetTransform = player.transform;
            }
        }
    }

    private void MoveToTarget()
    {
        if (targetTransform == null) return;

        Vector2 direction = (targetTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        if (target != null)
        {
            TopDownMovement player = target.GetComponent<TopDownMovement>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    public Vector3 GetPosition() => transform.position;

    public bool IsDeadMan()
    {
        this.gameObject.SetActive(false);
        return true;
    }

    public void KnockBack(float knockbackPower)
    {
        Vector2 knockbackDir = (transform.position - targetTransform.position).normalized;
        rb.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
    }

    public void TakeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0f)
        {
            IsDeadMan();
        }
    }

    public void TakeDotDamage(float amount, float duration, float interval)
    {
        Debug.Log($"Monster took DOT {amount} damage for {duration} seconds every {interval} seconds!");
    }
}
