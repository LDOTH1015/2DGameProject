using UnityEngine;

public class RangeBag : MonoBehaviour
{
    public enum FillAxis { X, Y }

    [Header("Children (Transforms)")]
    [SerializeField] private Transform backgroundTf; // 반투명 바(풀 사이즈)
    [SerializeField] private Transform foregroundTf; // 선명 바(채움)

    [Header("Fill Options")]
    [SerializeField] private FillAxis fillAxis = FillAxis.X; // ← X축으로 채우기
    [SerializeField] private float durationDefault = 0.5f;   // 기본 채움 시간(초)

    // foreground의 "풀" 상태(= background와 동일) 기준값 저장
    private Vector3 _fgBaseScale;
    private Vector3 _fgBaseLocalPos;

    private float _duration;
    private float _elapsed;
    private bool _active;

    private void Awake()
    {
        if (foregroundTf != null)
        {
            // 에디터에서 "풀로 찬 상태"로 배치된 scale/position을 저장
            _fgBaseScale = foregroundTf.localScale;
            _fgBaseLocalPos = foregroundTf.localPosition;
        }
    }

    /// <summary>
    /// 인디케이터 루트를 (로컬) 아래(-Y)가 기본인 상태에서, dir로 회전만 시켜 보여준다.
    /// 채움은 fillAxis(X/Y)에 따라 진행된다.
    /// </summary>
    public void Show(Vector2 dir, float duration)
    {
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.down;

        _duration = duration > 0f ? duration : durationDefault;
        _elapsed = 0f;
        _active = true;

        // 부모(IndicatorRoot)를 "down -> dir" 회전만 적용 (위치 변경 없음)
        transform.localRotation = Quaternion.FromToRotation(Vector3.down, dir.normalized);

        // 시작 상태: 0% (해당 축만 0으로, 반대 축/깊이는 유지)
        if (foregroundTf != null)
        {
            var s = _fgBaseScale;
            var p = _fgBaseLocalPos;

            if (fillAxis == FillAxis.X)
            {
                s.x = 0.0001f; // 0은 보정 문제 있으니 아주 작게
                p.x = 0f;      // basePos.x의 s배(=0) → 가방 쪽이 고정되는 효과
            }
            else // FillAxis.Y
            {
                s.y = 0.0001f;
                p.y = 0f;
            }

            foregroundTf.localScale = s;
            foregroundTf.localPosition = p;
        }

        gameObject.SetActive(true);
    }

    public void HideImmediate()
    {
        _active = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_active) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / _duration);

        if (foregroundTf != null)
        {
            var s = _fgBaseScale;
            var p = _fgBaseLocalPos;

            // "가방 쪽(윗변/가까운 변) 고정" 효과를 위해
            // scale과 position을 같은 비율 t로 보간한다.
            if (fillAxis == FillAxis.X)
            {
                s.x = Mathf.Max(0.0001f, _fgBaseScale.x * t);
                p.x = _fgBaseLocalPos.x * t;
            }
            else // Y축 채움
            {
                s.y = Mathf.Max(0.0001f, _fgBaseScale.y * t);
                p.y = _fgBaseLocalPos.y * t;
            }

            foregroundTf.localScale = s;
            foregroundTf.localPosition = p;
        }

        if (_elapsed >= _duration)
        {
            HideImmediate();
        }
    }
}
