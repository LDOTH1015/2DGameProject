using UnityEngine;

public class RangeBag : MonoBehaviour
{
    public enum FillAxis { X, Y }

    [Header("Children (Transforms)")]
    [SerializeField] private Transform backgroundTf; // ������ ��(Ǯ ������)
    [SerializeField] private Transform foregroundTf; // ���� ��(ä��)

    [Header("Fill Options")]
    [SerializeField] private FillAxis fillAxis = FillAxis.X; // �� X������ ä���
    [SerializeField] private float durationDefault = 0.5f;   // �⺻ ä�� �ð�(��)

    // foreground�� "Ǯ" ����(= background�� ����) ���ذ� ����
    private Vector3 _fgBaseScale;
    private Vector3 _fgBaseLocalPos;

    private float _duration;
    private float _elapsed;
    private bool _active;

    private void Awake()
    {
        if (foregroundTf != null)
        {
            // �����Ϳ��� "Ǯ�� �� ����"�� ��ġ�� scale/position�� ����
            _fgBaseScale = foregroundTf.localScale;
            _fgBaseLocalPos = foregroundTf.localPosition;
        }
    }

    /// <summary>
    /// �ε������� ��Ʈ�� (����) �Ʒ�(-Y)�� �⺻�� ���¿���, dir�� ȸ���� ���� �����ش�.
    /// ä���� fillAxis(X/Y)�� ���� ����ȴ�.
    /// </summary>
    public void Show(Vector2 dir, float duration)
    {
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.down;

        _duration = duration > 0f ? duration : durationDefault;
        _elapsed = 0f;
        _active = true;

        // �θ�(IndicatorRoot)�� "down -> dir" ȸ���� ���� (��ġ ���� ����)
        transform.localRotation = Quaternion.FromToRotation(Vector3.down, dir.normalized);

        // ���� ����: 0% (�ش� �ุ 0����, �ݴ� ��/���̴� ����)
        if (foregroundTf != null)
        {
            var s = _fgBaseScale;
            var p = _fgBaseLocalPos;

            if (fillAxis == FillAxis.X)
            {
                s.x = 0.0001f; // 0�� ���� ���� ������ ���� �۰�
                p.x = 0f;      // basePos.x�� s��(=0) �� ���� ���� �����Ǵ� ȿ��
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

            // "���� ��(����/����� ��) ����" ȿ���� ����
            // scale�� position�� ���� ���� t�� �����Ѵ�.
            if (fillAxis == FillAxis.X)
            {
                s.x = Mathf.Max(0.0001f, _fgBaseScale.x * t);
                p.x = _fgBaseLocalPos.x * t;
            }
            else // Y�� ä��
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
