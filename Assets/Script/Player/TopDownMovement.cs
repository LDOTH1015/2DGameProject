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

    // === 방향 전환용 ===
    [Header("Visual (Child)")]
    [SerializeField] private Transform visualRoot;   // 예: MainSprite1
    private SpriteRenderer spriteRenderer;           // visualRoot에서 가져옴
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

        // --- 방향 전환 참조 세팅 ---
        if (visualRoot == null)
        {
            var t = transform.Find("MainSprite1");
            if (t != null) visualRoot = t;
            else if (transform.childCount > 0) visualRoot = transform.GetChild(0);
        }
        if (visualRoot != null)
            spriteRenderer = visualRoot.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogWarning("[TopDownMovement] SpriteRenderer를 찾지 못했습니다. MainSprite1에 SpriteRenderer가 있는지 확인하세요.");
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

        // 이동 애니
        animator.SetBool("Move", direction.sqrMagnitude > 0.0001f);

        // === 방향 전환 ===
        if (direction.sqrMagnitude > 0.0001f)
        {
            lastNonZeroInput = direction;
            ApplyFacing(direction);
        }
    }

    private void ApplyFacing(Vector2 dir)
    {
        if (spriteRenderer == null) return;
        // 좌우만 전환: x 음수면 왼쪽 바라봄
        if (Mathf.Abs(dir.x) > 0.001f)
            spriteRenderer.flipX = dir.x < 0f;
        // 필요하면 상하에 따른 레이어/애니 분기 여기에 추가 가능
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

            // 회피 애니 트리거
            animator.ResetTrigger("Evasion");
            animator.SetTrigger("Evasion");

            // 회피 중 이동 애니 끔
            animator.SetBool("Move", false);

            // 회피 방향: 입력 없으면 마지막 방향 유지(있다면 네가 쓰던 변수 기준으로)
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

        // 회피 끝난 후 현재 입력 기준으로 방향/애니 재보정
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

        // 이동 중에도 방향 유지 보정(옵션)
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
