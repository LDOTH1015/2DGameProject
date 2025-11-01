using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float playerDetectRange = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackDelay = 1f;

    protected float HP = 100f;
    protected GameObject target;
    protected Transform targetTransform;
    protected Rigidbody2D rb;

    protected bool canAttack = true;
    protected bool isInAttackRange = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    internal void Initialize(GameObject target)
    {
        HP = 100f;
        this.target = target;
        targetTransform = target.transform;
    }

    protected virtual void Update()
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

    protected void SearchPlayer()
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

    protected void MoveToTarget()
    {
        if (targetTransform == null) return;

        Vector2 direction = (targetTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    protected IEnumerator Attack()
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
        Destroy(this.gameObject);
        return true;
    }

    public void KnockBack(float knockbackPower)
    {
        Vector2 knockbackDir = (transform.position - targetTransform.position).normalized;
        rb.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
    }

    public virtual void TakeDamage(float amount)
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
