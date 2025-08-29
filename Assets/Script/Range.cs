using System.Collections;
using UnityEngine;

public class Range : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float growDuration = 0.55f; // 커지는 시간
    [SerializeField] private float targetScale = 3f;   // 최종 크기
    [SerializeField] private float lifeTime = 0.55f;      // 전체 생존 시간 (파괴되기 전까지)

    private void Start()
    {
        StartCoroutine(ScaleAndDestroy());
    }

    private IEnumerator ScaleAndDestroy()
    {
        // 현재 크기와 목표 크기
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * targetScale;

        float elapsed = 0f;

        // 커지기 시작
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        // lifeTime이 growDuration보다 크다면 남은 시간 대기
        if (lifeTime > growDuration)
        {
            yield return new WaitForSeconds(lifeTime - growDuration);
        }

        // 오브젝트 파괴
        Destroy(gameObject);
    }
}
