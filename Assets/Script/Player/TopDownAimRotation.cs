using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownAimRotation : MonoBehaviour
{
    [Header("Renderers / Pivots")]
    [SerializeField] private SpriteRenderer armRenderer;      // 무기 SpriteRenderer
    [SerializeField] private Transform armPivot1;             // 무기 Pivot Transform
    [SerializeField] private SpriteRenderer characterRenderer1;

    [Header("Input Source")]
    [SerializeField] private TopDownController _controller;   // 인풋 소스

    [Header("Orbit Settings")]
    [Tooltip("플레이어 루트/몸체/손 소켓")]
    [SerializeField] private Transform followTarget;
    [Tooltip("플레이어 기준 무기까지 거리")]
    [SerializeField] private float orbitRadius = 0.6f;
    [Tooltip("세로 보정값 (y축 오프셋)")]
    [SerializeField] private float heightOffset = 0.0f;
    [Tooltip("Pivot이 중앙일 때 손잡이가 플레이어 쪽으로 오도록 당길 거리")]
    [SerializeField] private float gripOffset = 0.2f;

    [Header("Sorting (Optional)")]
    [Tooltip("무기를 캐릭터 앞/뒤로 자동 소팅할지 여부")]
    [SerializeField] private bool useDynamicSorting = true;
    [SerializeField] private int baseSortingOrder = 0;
    [SerializeField] private int frontOffset = 1;
    [SerializeField] private int backOffset = -1;

    // 현재 조준 각도 (deg)
    public float CurrentAimAngle { get; private set; } = 0f;
    private bool _isOverridden = false;   // 베기 중 각도 고정

    private void Awake()
    {
        // TopDownController 자동 탐색
        if (_controller == null)
            _controller = GetComponent<TopDownController>();
        if (_controller == null)
            _controller = GetComponentInParent<TopDownController>();
        if (_controller == null)
            _controller = GetComponentInChildren<TopDownController>();

        if (_controller == null)
        {
            Debug.LogError("[TopDownAimRotation] TopDownController not found. 인스펙터에서 직접 할당하세요.");
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

    // 입력 기반 조준
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

        // 1) 현재 에임 각도 → 방향 벡터
        float rad = CurrentAimAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)); // 플레이어 → 무기 방향

        // 2) 공전 위치 + 손잡이 보정
        Vector3 center = followTarget.position;
        Vector2 bladeDir = dir.normalized;
        Vector3 orbitPos = center
                         + (Vector3)(dir * orbitRadius)
                         - (Vector3)(bladeDir * gripOffset);
        orbitPos.y += heightOffset;
        armPivot1.position = orbitPos;

        // 3) 회전: 칼날은 항상 플레이어 반대(-bladeDir)를 향하게 함
        armPivot1.up = -bladeDir;

        // 4) 캐릭터 flip 보정
        if (characterRenderer1 != null)
            characterRenderer1.flipX = Mathf.Abs(CurrentAimAngle) > 90f;

        // 5) 앞/뒤 소팅
        if (useDynamicSorting && armRenderer != null)
        {
            bool isBehind = bladeDir.y > 0f;
            armRenderer.sortingOrder = baseSortingOrder + (isBehind ? backOffset : frontOffset);
        }
    }
}
