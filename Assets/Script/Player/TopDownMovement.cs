using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TopDownMovement : MonoBehaviour, IDamageable
{
    private TopDownController controller;
    private Rigidbody2D movementRigidbody;
    protected Animator animator;

    public Arrow arrow;
    private ObjectPool<Arrow> arrowPool;

    private Vector2 movementDirection = Vector2.zero;
    private bool isEvasion = false;

    // === ���� ��ȯ�� ===
    [Header("Visual (Child)")]
    [SerializeField] private Transform visualRoot;   // ��: MainSprite1
    private SpriteRenderer spriteRenderer;           // visualRoot���� ������
    private Vector2 lastNonZeroInput = Vector2.right;

    private float evasionDuraion = 0.2f;
    private float evationSpeed = 16.0f;

    [SerializeField] private GameObject endPanel;
    [SerializeField] private Image potion;
    [SerializeField] private GameObject inventory;

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        animator = GetComponentInChildren<Animator>();
        movementRigidbody = GetComponent<Rigidbody2D>();
        arrowPool = new ObjectPool<Arrow>(arrow, 5, transform);

        // --- ���� ��ȯ ���� ���� ---
        if (visualRoot == null)
        {
            var t = transform.Find("MainSprite1");
            if (t != null) visualRoot = t;
            else if (transform.childCount > 0) visualRoot = transform.GetChild(0);
        }
        if (visualRoot != null)
            spriteRenderer = visualRoot.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogWarning("[TopDownMovement] SpriteRenderer�� ã�� ���߽��ϴ�. MainSprite1�� SpriteRenderer�� �ִ��� Ȯ���ϼ���.");
    }

    private void Start()
    {
        controller.OnMoveEvent += Move;
        controller.OnEvasionEvent += Evasion;
        controller.OnFireEvent += Fire;
        controller.OnInvenEvent += Inven;
        controller.OnPotionEvent += Potion;
    }

    private void Move(Vector2 direction)
    {
        if (isEvasion) return;
        movementDirection = direction;

        // �̵� �ִ�
        animator.SetBool("Move", direction.sqrMagnitude > 0.0001f);

        // === ���� ��ȯ ===
        if (direction.sqrMagnitude > 0.0001f)
        {
            lastNonZeroInput = direction;
            ApplyFacing(direction);
        }
    }

    private void ApplyFacing(Vector2 dir)
    {
        if (spriteRenderer == null) return;
        // �¿츸 ��ȯ: x ������ ���� �ٶ�
        if (Mathf.Abs(dir.x) > 0.001f)
            spriteRenderer.flipX = dir.x < 0f;
        // �ʿ��ϸ� ���Ͽ� ���� ���̾�/�ִ� �б� ���⿡ �߰� ����
    }

    private void Inven()
    {
        inventory.SetActive(true);
    }

    private void Potion()
    {
        if (PlayerStatus.Instance.curruntHP + 50 >= PlayerStatus.Instance.maxHp)
            PlayerStatus.Instance.curruntHP = PlayerStatus.Instance.maxHp;
        else
            PlayerStatus.Instance.curruntHP += 50;

        potion.fillAmount = 1;
    }

    private void Fire()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector2 shootDirection = (nearestEnemy.transform.position - this.transform.position).normalized;
            Arrow projectile = arrowPool.Get(this.transform.position, Quaternion.identity);
            projectile.Initialize(shootDirection, 10, 5f);
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");
        GameObject nearestEnemy = null;
        float shortestDistance = 10f;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    private void Evasion()
    {
        if (isEvasion) return;

        if (PlayerStatus.Instance.curruntStamina >= 50)
        {
            PlayerStatus.Instance.curruntStamina -= 50;

            // ȸ�� �ִ� Ʈ����
            animator.ResetTrigger("Evasion");
            animator.SetTrigger("Evasion");

            // ȸ�� �� �̵� �ִ� ��
            animator.SetBool("Move", false);

            // ȸ�� ����: �Է� ������ ������ ���� ����(�ִٸ� �װ� ���� ���� ��������)
            Vector2 dir = (movementDirection.sqrMagnitude > 0.0001f) ? movementDirection : Vector2.right;
            StartCoroutine(EvasionCoroutine(dir.normalized));
        }
    }

    private IEnumerator EvasionCoroutine(Vector2 direction)
    {
        isEvasion = true;

        float elapsed = 0f;
        while (elapsed < evasionDuraion)
        {
            movementRigidbody.velocity = direction * evationSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }
        movementRigidbody.velocity = Vector2.zero;
        isEvasion = false;

        // ȸ�� ���� �� ���� �Է� �������� ����/�ִ� �纸��
        animator.SetBool("Move", movementDirection.sqrMagnitude > 0.0001f);
        if (movementDirection.sqrMagnitude > 0.0001f)
            ApplyFacing(movementDirection);
    }

    private void FixedUpdate()
    {
        if (!isEvasion)
        {
            ApplyMovement(movementDirection);
        }
    }

    private void ApplyMovement(Vector2 direction)
    {
        movementRigidbody.velocity = direction * 5;

        // �̵� �߿��� ���� ���� ����(�ɼ�)
        if (direction.sqrMagnitude > 0.0001f)
            ApplyFacing(direction);
    }

    public void TakeDamage(float amount)
    {
        if (PlayerStatus.Instance.curruntHP - amount <= 0f)
        {
            IsDeadMan();
        }
        else
        {
            PlayerStatus.Instance.curruntHP -= amount;
        }
    }

    public void KnockBack(float knockbackPower) => throw new NotImplementedException();
    public void TakeDotDamage(float amount, float duration, float interval) => throw new NotImplementedException();

    public bool IsDeadMan()
    {
        endPanel.SetActive(true);
        Time.timeScale = 0f;
        return true;
    }

    public Vector3 GetPosition() => transform.position;
}
