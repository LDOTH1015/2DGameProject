using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownAimRotation : MonoBehaviour
{
    [Header("Renderers / Pivots")]
    [SerializeField] private SpriteRenderer armRenderer;      // ���� SpriteRenderer
    [SerializeField] private Transform armPivot1;             // ���� Pivot Transform
    [SerializeField] private SpriteRenderer characterRenderer1;

    [Header("Input Source")]
    [SerializeField] private TopDownController _controller;   // ��ǲ �ҽ�

    [Header("Orbit Settings")]
    [Tooltip("�÷��̾� ��Ʈ/��ü/�� ����")]
    [SerializeField] private Transform followTarget;
    [Tooltip("�÷��̾� ���� ������� �Ÿ�")]
    [SerializeField] private float orbitRadius = 0.6f;
    [Tooltip("���� ������ (y�� ������)")]
    [SerializeField] private float heightOffset = 0.0f;
    [Tooltip("Pivot�� �߾��� �� �����̰� �÷��̾� ������ ������ ��� �Ÿ�")]
    [SerializeField] private float gripOffset = 0.2f;

    [Header("Sorting (Optional)")]
    [Tooltip("���⸦ ĳ���� ��/�ڷ� �ڵ� �������� ����")]
    [SerializeField] private bool useDynamicSorting = true;
    [SerializeField] private int baseSortingOrder = 0;
    [SerializeField] private int frontOffset = 1;
    [SerializeField] private int backOffset = -1;

    // ���� ���� ���� (deg)
    public float CurrentAimAngle { get; private set; } = 0f;
    private bool _isOverridden = false;   // ���� �� ���� ����

    private void Awake()
    {
        // TopDownController �ڵ� Ž��
        if (_controller == null)
            _controller = GetComponent<TopDownController>();
        if (_controller == null)
            _controller = GetComponentInParent<TopDownController>();
        if (_controller == null)
            _controller = GetComponentInChildren<TopDownController>();

        if (_controller == null)
        {
            Debug.LogError("[TopDownAimRotation] TopDownController not found. �ν����Ϳ��� ���� �Ҵ��ϼ���.");
            enabled = false;
            return;
        }

        if (followTarget == null)
            followTarget = _controller.transform;

        _controller.OnLookEvent += OnAim;
    }

    private void OnDestroy()
    {
        if (_controller != null) _controller.OnLookEvent -= OnAim;
    }

    // �Է� ��� ����
    public void OnAim(Vector2 newAimDirection)
    {
        if (_isOverridden) return;
        float rotZ = Mathf.Atan2(newAimDirection.y, newAimDirection.x) * Mathf.Rad2Deg;
        SetAngleDeg(rotZ);
    }

    public void SetOverride(bool enabled) => _isOverridden = enabled;

    public void SetAngleDeg(float angleDeg)
    {
        CurrentAimAngle = angleDeg;
        if (characterRenderer1 != null)
            characterRenderer1.flipX = Mathf.Abs(angleDeg) > 90f;
    }

    private void LateUpdate()
    {
        if (armPivot1 == null || followTarget == null) return;

        // 1) ���� ���� ���� �� ���� ����
        float rad = CurrentAimAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)); // �÷��̾� �� ���� ����

        // 2) ���� ��ġ + ������ ����
        Vector3 center = followTarget.position;
        Vector2 bladeDir = dir.normalized;
        Vector3 orbitPos = center
                         + (Vector3)(dir * orbitRadius)
                         - (Vector3)(bladeDir * gripOffset);
        orbitPos.y += heightOffset;
        armPivot1.position = orbitPos;

        // 3) ȸ��: Į���� �׻� �÷��̾� �ݴ�(-bladeDir)�� ���ϰ� ��
        armPivot1.up = -bladeDir;

        // 4) ĳ���� flip ����
        if (characterRenderer1 != null)
            characterRenderer1.flipX = Mathf.Abs(CurrentAimAngle) > 90f;

        // 5) ��/�� ����
        if (useDynamicSorting && armRenderer != null)
        {
            bool isBehind = bladeDir.y > 0f;
            armRenderer.sortingOrder = baseSortingOrder + (isBehind ? backOffset : frontOffset);
        }
    }
}
