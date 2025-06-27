using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float playerDetectRange = 5f;

    private float HP = 100f;
    private GameObject target;
    private Transform targetTransform;
    private Rigidbody2D rb;

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
        MoveToTarget();
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

    public Vector3 GetPosition()
    {
        return transform.position;
    }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<TopDownMovement>().TakeDamage(10f);
        }
    }
}
