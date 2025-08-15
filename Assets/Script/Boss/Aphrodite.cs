using System.Collections;
using UnityEngine;

public class Aphrodite : Monster
{
    [Header("Boss Patterns")]
    [SerializeField] private GameObject mapAreaObject;   // �� GameObject (Ÿ�ϸ�/��������Ʈ/�޽� OK)
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Monster minionPrefab;

    [Header("Bomb Pattern Settings")]
    [SerializeField] private float bombSpawnInterval = 3f;
    [SerializeField] private int bombsPerWave = 6;               // �⺻�� ���� �÷���
    [SerializeField] private float bombSafeRadiusFromPlayer = 0.8f;
    [SerializeField] private float spawnMargin = 0.5f;           // �� �����ڸ����� ����

    [Header("Bounds Fallback (optional)")]
    [SerializeField] private Vector2 sizeOverride = new Vector2(30f, 17f); // Renderer�� ���� �� ���� ���� ũ��
    [SerializeField] private bool includeChildrenRenderers = true;         // �ڽ� ���������� ��������

    [Header("Minion Pattern Settings")]
    [SerializeField] private int minionSpawnCount = 3;
    [SerializeField] private float minionSpawnSpread = 1.5f;

    private float maxHP;
    private bool halfTriggered = false;
    private Coroutine bombRoutine;

    protected override void Awake() { base.Awake(); }
    private void OnEnable() { if (bombRoutine == null) bombRoutine = StartCoroutine(BombPatternLoop()); }
    private void OnDisable() { if (bombRoutine != null) { StopCoroutine(bombRoutine); bombRoutine = null; } }

    private void Start()
    {
        maxHP = HP <= 0f ? 100f : HP;
    }

    protected override void Update()
    {
        base.Update();
        if (!halfTriggered && HP <= maxHP * 0.5f)
        {
            halfTriggered = true;
            SpawnMinions();
        }
    }

    private IEnumerator BombPatternLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(bombSpawnInterval);
            if (!gameObject.activeInHierarchy) yield break;

            for (int i = 0; i < bombsPerWave; i++)
            {
                Vector2 spawnPos = GetRandomPointInMapBounds();
                if (targetTransform != null &&
                    Vector2.Distance(spawnPos, targetTransform.position) < bombSafeRadiusFromPlayer)
                {
                    // �÷��̾� �ʹ� �����ϸ� �� �� �� �̱�
                    spawnPos = GetRandomPointInMapBounds();
                }
                Instantiate(bombPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    private void SpawnMinions()
    {
        GameObject playerGO = GameObject.FindWithTag("Player");
        for (int i = 0; i < minionSpawnCount; i++)
        {
            float angle = (360f / Mathf.Max(1, minionSpawnCount)) * i;
            Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * minionSpawnSpread;
            Vector3 spawnPos = transform.position + (Vector3)offset;
            Monster m = Instantiate(minionPrefab, spawnPos, Quaternion.identity);
            if (playerGO != null) m.Initialize(playerGO);
        }
    }

    // ��� �ٽ�: �� GameObject�� "���� ���� �ٿ��"�� ���
    private Bounds GetMapWorldBounds()
    {
        if (mapAreaObject == null)
            return new Bounds(transform.position, new Vector3(sizeOverride.x, sizeOverride.y, 1f));

        // 1) Renderer���� ��� ������ bounds ���� (SpriteRenderer, TilemapRenderer, MeshRenderer ���� ����)
        var renderers = includeChildrenRenderers
            ? mapAreaObject.GetComponentsInChildren<Renderer>()
            : mapAreaObject.GetComponents<Renderer>();

        if (renderers != null && renderers.Length > 0)
        {
            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                b.Encapsulate(renderers[i].bounds);
            return b;
        }

        // 2) Renderer�� ������ Transform�� ���� ũ�� ������(�Ǵ� sizeOverride) ���
        var t = mapAreaObject.transform;
        // lossyScale�� �θ� ������ �ݿ�
        Vector3 size = new Vector3(
            Mathf.Abs(t.lossyScale.x) > 0.001f ? Mathf.Abs(t.lossyScale.x) : sizeOverride.x,
            Mathf.Abs(t.lossyScale.y) > 0.001f ? Mathf.Abs(t.lossyScale.y) : sizeOverride.y,
            1f
        );
        return new Bounds(t.position, size);
    }

    // ��� �ٿ�� ������ ���� ����Ʈ�� �а� ���ø�
    private Vector2 GetRandomPointInMapBounds()
    {
        Bounds b = GetMapWorldBounds();
        float minX = b.min.x + spawnMargin;
        float maxX = b.max.x - spawnMargin;
        float minY = b.min.y + spawnMargin;
        float maxY = b.max.y - spawnMargin;

        // Ȥ�� margin�� ����ġ�� ū ��츦 ����� ������ġ
        if (minX > maxX) { float c = (minX + maxX) * 0.5f; minX = c - 0.1f; maxX = c + 0.1f; }
        if (minY > maxY) { float c = (minY + maxY) * 0.5f; minY = c - 0.1f; maxY = c + 0.1f; }

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        return new Vector2(x, y);
    }

    public override void TakeDamage(float amount) { base.TakeDamage(amount); }
}
