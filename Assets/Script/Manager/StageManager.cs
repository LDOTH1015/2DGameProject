using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Map Bounds (사각형)")]
    [SerializeField] private BoxCollider2D mapBounds;   // 맵 외곽 BoxCollider2D
    [SerializeField] private float inwardMax = 3f;       // 테두리에서 안쪽으로 들어오는 최대 거리

    [Header("Target (Player)")]
    [SerializeField] private GameObject player;

    [Header("Spawn (하드코딩 단일 웨이브)")]
    [SerializeField] private int spawnCount = 10;        // 이번 스테이지에서 스폰할 총 마릿수
    [SerializeField] private float spawnInterval = 0.2f; // 연속 스폰 간격(0이면 즉시 전량)
    [SerializeField] private float startDelay = 0f;      // 스테이지 시작 전 대기

    [Header("Monster Sources")]
    [SerializeField] private GameObject[] monsterPrefabs; // 현재 하드코딩: 여기서 랜덤 스폰
    [SerializeField] private string[] monsterNames;       // ★추가 준비: 앞으로 "이름 기반 스폰" 예정 (현재는 옵션)

    [Header("Prefab Library (이름→프리팹 룩업용)")]
    [Tooltip("여기에 모든 몬스터 프리팹을 등록해 두면, monsterNames에서 이름으로 찾아 스폰 가능")]
    [SerializeField] private GameObject[] prefabLibrary;  // ★추가 준비: 이름 기반 대비 라이브러리

    [Header("UI Toggle During Stage")]
    [Tooltip("스테이지 진행 중에는 비활성화되고, 클리어되면 다시 활성화될 UI 오브젝트들")]
    [SerializeField] private GameObject[] uiToToggleDuringStage;

    public event Action OnStageStarted;
    public event Action OnStageCleared;

    private readonly List<Monster> _alive = new List<Monster>();
    private bool _spawning;
    private bool _stageCleared;

    // ★추가: 이름→프리팹 캐시 (프리팹 이름 키)
    private readonly Dictionary<string, GameObject> _prefabByName =
        new Dictionary<string, GameObject>(StringComparer.Ordinal);

    // 현재 스테이지 진행 중 여부 (버튼 중복 클릭/토글 제어용)
    public bool IsStageRunning => _spawning || (_alive.Count > 0 && !_stageCleared);

    // 내부 플래그: 한 번이라도 시작 버튼 눌렀는지
    private bool IsAnyStageEverStarted = false;

    private void Awake()
    {
        if (mapBounds == null)
            Debug.LogError("[StageManager] mapBounds(BoxCollider2D)를 연결하세요.");

        if (player == null)
            Debug.LogWarning("[StageManager] player가 비어 있음(Initialize에 null 전달됨).");

        BuildPrefabLookup(); // ★이름 기반 스폰 준비

        // 시작 시에는 UI를 보이도록(대기 화면)
        SetStageUIVisible(true);
    }

    // ✅ 자동 시작 없음 (버튼으로만 시작)
    // private void Start() { }

    private void Update()
    {
        PruneDead(); // 죽은 객체 파괴 후 리스트 정리

        // 스폰이 끝났고, 살아있는 리스트가 비면 스테이지 클리어
        if (!_stageCleared && !_spawning && _alive.Count == 0 && IsAnyStageEverStarted)
        {
            _stageCleared = true;
            Debug.Log("[StageManager] Stage Cleared!");
            OnStageCleared?.Invoke();

            // ★클리어 시 UI 다시 표시
            SetStageUIVisible(true);
        }
    }

    /// <summary>
    /// UI 버튼 OnClick에 연결해서 호출하세요.
    /// </summary>
    public void StartStageFromButton() => StartStage();

    /// <summary>
    /// 스테이지 시작 (버튼에서 호출)
    /// </summary>
    public void StartStage()
    {
        if (_spawning || (_alive.Count > 0 && !_stageCleared))
        {
            Debug.Log("[StageManager] 이미 진행 중입니다.");
            return;
        }

        StopAllCoroutines();
        _alive.Clear();
        _stageCleared = false;
        IsAnyStageEverStarted = true;

        Debug.Log("[StageManager] Stage Started");
        OnStageStarted?.Invoke();

        // ★시작 시 UI 숨김
        SetStageUIVisible(false);

        StartCoroutine(CoRunSingleWave());
    }

    private System.Collections.IEnumerator CoRunSingleWave()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        _spawning = true;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = null;

            // ★미래 변경 포인트: 이름 기반 스폰 우선
            if (monsterNames != null && monsterNames.Length > 0)
            {
                string key = monsterNames[UnityEngine.Random.Range(0, monsterNames.Length)];
                prefab = GetPrefabByName(key); // ← 이름으로 프리팹 조회
                if (prefab == null)
                {
                    Debug.LogWarning($"[StageManager] 이름 '{key}' 프리팹을 찾지 못함. prefabLibrary를 확인하세요.");
                }
            }

            if (prefab == null)
            {
                if (monsterPrefabs != null && monsterPrefabs.Length > 0)
                {
                    prefab = monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Length)];
                }
            }

            SpawnOne(prefab);

            if (spawnInterval > 0f)
                yield return new WaitForSeconds(spawnInterval);
        }

        _spawning = false;
    }

    private void SpawnOne(GameObject prefab)
    {
        if (prefab == null) return;

        Vector2 pos = GetRandomBorderPosition();
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);

        var monster = go.GetComponent<Monster>();
        if (monster != null)
        {
            monster.Initialize(player);
            _alive.Add(monster);
        }
        else
        {
            Debug.LogWarning($"[StageManager] {prefab.name}에는 Monster 컴포넌트가 없습니다.");
        }
    }

    private Vector2 GetRandomBorderPosition()
    {
        Bounds b = mapBounds.bounds;

        int side = UnityEngine.Random.Range(0, 4);
        float x, y;
        Vector2 inwardNormal;

        switch (side)
        {
            case 0: // Bottom
                x = UnityEngine.Random.Range(b.min.x, b.max.x);
                y = b.min.y; inwardNormal = Vector2.up; break;
            case 1: // Top
                x = UnityEngine.Random.Range(b.min.x, b.max.x);
                y = b.max.y; inwardNormal = Vector2.down; break;
            case 2: // Left
                x = b.min.x;
                y = UnityEngine.Random.Range(b.min.y, b.max.y);
                inwardNormal = Vector2.right; break;
            default: // Right
                x = b.max.x;
                y = UnityEngine.Random.Range(b.min.y, b.max.y);
                inwardNormal = Vector2.left; break;
        }

        float inward = UnityEngine.Random.Range(0f, Mathf.Max(0f, inwardMax));
        Vector2 p = new Vector2(x, y) + inwardNormal * inward;

        const float eps = 0.01f;
        p.x = Mathf.Clamp(p.x, b.min.x + eps, b.max.x - eps);
        p.y = Mathf.Clamp(p.y, b.min.y + eps, b.max.y - eps);
        return p;
    }

    /// <summary>
    /// 죽은/파괴된 몬스터 정리 (Destroy까지 수행)
    /// </summary>
    private void PruneDead()
    {
        for (int i = _alive.Count - 1; i >= 0; i--)
        {
            var m = _alive[i];
            if (m == null) // 이미 Destroy된 경우
            {
                _alive.RemoveAt(i);
                continue;
            }
            if (!m.gameObject.activeInHierarchy)
            {
                Destroy(m.gameObject); // 혹시 SetActive(false) 상태로 남아있다면 여기서 파괴
                _alive.RemoveAt(i);
            }
        }
    }

    public int AliveCount => _alive.Count;

    // ===== 이름 기반 스폰 준비 유틸 =====

    private void BuildPrefabLookup()
    {
        _prefabByName.Clear();
        if (prefabLibrary == null) return;

        foreach (var p in prefabLibrary)
        {
            if (p == null) continue;
            var key = p.name; // ★중요: “프리팹 이름”을 키로 사용 (추후 Addressables/Resources로 대체 용이)
            if (!_prefabByName.ContainsKey(key))
                _prefabByName.Add(key, p);
        }
    }

    private GameObject GetPrefabByName(string nameKey)
    {
        if (string.IsNullOrEmpty(nameKey)) return null;
        if (_prefabByName.TryGetValue(nameKey, out var prefab))
            return prefab;
        return null;
    }

    // ===== UI 토글 유틸 =====

    private void SetStageUIVisible(bool visible)
    {
        if (uiToToggleDuringStage == null) return;
        for (int i = 0; i < uiToToggleDuringStage.Length; i++)
        {
            var go = uiToToggleDuringStage[i];
            if (go == null) continue;
            go.SetActive(visible);
        }
    }
}
