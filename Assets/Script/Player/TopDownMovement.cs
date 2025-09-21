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

    [Header("Visual (Child)")]
    [SerializeField] private Transform visualRoot;   // 예: MainSprite1
    private SpriteRenderer spriteRenderer;
    private Vector2 lastNonZeroInput = Vector2.right;

    private float evasionDuraion = 0.2f;
    private float evationSpeed = 16.0f;

    [SerializeField] private GameObject endPanel;
    [SerializeField] private Image potion;
    [SerializeField] private GameObject inventory;

    // === 근접 베기용 ===
    [Header("Melee Slash")]
    [SerializeField] private TopDownAimRotation aim;     // Aim 스크립트 참조
    [SerializeField] private Transform weaponRoot;       // 검 루트 (스케일 변경 대상)
    [SerializeField] private Collider2D weaponCollider;  // 검 충돌체 (베기 중에만 ON)
    [SerializeField] private float slashDuration = 0.5f; // 0.5초 동안 휘두름
    [SerializeField] private float slashArcDeg = 90f;    // ±45도 (총 90도 회전)
    [SerializeField] private string attackTriggerName = "Attack";

    private bool isAttacking = false;
    private int attackIndex = 0; // 1,2,(3=강공)->0
    private Vector3 weaponDefaultScale;

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        animator = GetComponentInChildren<Animator>();
        movementRigidbody = GetComponent<Rigidbody2D>();
        arrowPool = new ObjectPool<Arrow>(arrow, 5, transform);

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

        if (weaponRoot != null) weaponDefaultScale = weaponRoot.localScale;
    }

    private void Start()
    {
        controller.OnMoveEvent += Move;
        controller.OnEvasionEvent += Evasion;
        controller.OnFireEvent += Fire;
        controller.OnInvenEvent += Inven;
        controller.OnPotionEvent += Potion;

        if (weaponCollider != null) weaponCollider.enabled = false;
    }

    private void Move(Vector2 direction)
    {
        if (isEvasion || isAttacking) return;
        movementDirection = direction;

        animator.SetBool("Move", direction.sqrMagnitude > 0.0001f);

        if (direction.sqrMagnitude > 0.0001f)
        {
            lastNonZeroInput = direction;
            ApplyFacing(direction);
        }
    }

    private void ApplyFacing(Vector2 dir)
    {
        if (spriteRenderer == null) return;
        if (Mathf.Abs(dir.x) > 0.001f)
            spriteRenderer.flipX = dir.x < 0f;
    }

    private void Inven() => inventory.SetActive(true);

    private void Potion()
    {
        if (PlayerStatus.Instance.curruntHP + 50 >= PlayerStatus.Instance.maxHp)
            PlayerStatus.Instance.curruntHP = PlayerStatus.Instance.maxHp;
        else
            PlayerStatus.Instance.curruntHP += 50;

        potion.fillAmount = 1;
    }

    // === Fire 수정본 ===
    private void Fire()
    {
        if (isAttacking) return;
        if (aim == null)
        {
            Debug.LogWarning("[TopDownMovement] Aim(TopDownAimRotation) 참조가 필요합니다.");
            return;
        }

        IEnumerator Slash()
        {
            isAttacking = true;

            if (!string.IsNullOrEmpty(attackTriggerName) && animator != null)
            {
                animator.ResetTrigger(attackTriggerName);
                animator.SetTrigger(attackTriggerName);
            }
            animator.SetBool("Move", false);

            attackIndex++;
            bool isPower = false;
            if (attackIndex >= 3)
            {
                isPower = true;
                attackIndex = 0;
            }

            if (weaponRoot != null)
                weaponRoot.localScale = isPower ? weaponDefaultScale * 2f : weaponDefaultScale;

            if (weaponCollider != null)
                weaponCollider.enabled = true;

            aim.SetOverride(true);

            float center = aim.CurrentAimAngle;
            float halfArc = slashArcDeg * 0.5f;
            float startAngle = center + halfArc;
            float endAngle = center - halfArc;

            float elapsed = 0f;
            while (elapsed < slashDuration)
            {
                float t = elapsed / slashDuration;
                float curr = Mathf.Lerp(startAngle, endAngle, t);
                aim.SetAngleDeg(curr);

                elapsed += Time.deltaTime;
                yield return null;
            }
            aim.SetAngleDeg(endAngle);

            if (weaponCollider != null)
                weaponCollider.enabled = false;

            if (weaponRoot != null)
                weaponRoot.localScale = weaponDefaultScale;

            aim.SetOverride(false);
            isAttacking = false;

            animator.SetBool("Move", movementDirection.sqrMagnitude > 0.0001f);
            if (movementDirection.sqrMagnitude > 0.0001f)
                ApplyFacing(movementDirection);
        }

        StartCoroutine(Slash());
    }

    private void Evasion()
    {
        if (isEvasion || isAttacking) return;

        if (PlayerStatus.Instance.curruntStamina >= 50)
        {
            PlayerStatus.Instance.curruntStamina -= 50;

            animator.ResetTrigger("Evasion");
            animator.SetTrigger("Evasion");
            animator.SetBool("Move", false);

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

        animator.SetBool("Move", movementDirection.sqrMagnitude > 0.0001f);
        if (movementDirection.sqrMagnitude > 0.0001f)
            ApplyFacing(movementDirection);
    }

    private void FixedUpdate()
    {
        if (!isEvasion && !isAttacking)
        {
            ApplyMovement(movementDirection);
        }
    }

    private void ApplyMovement(Vector2 direction)
    {
        movementRigidbody.velocity = direction * 5;
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
