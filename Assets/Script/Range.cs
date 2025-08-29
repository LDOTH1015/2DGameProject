using System.Collections;
using UnityEngine;

public class Range : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float growDuration = 0.55f; // Ŀ���� �ð�
    [SerializeField] private float targetScale = 3f;   // ���� ũ��
    [SerializeField] private float lifeTime = 0.55f;      // ��ü ���� �ð� (�ı��Ǳ� ������)

    private void Start()
    {
        StartCoroutine(ScaleAndDestroy());
    }

    private IEnumerator ScaleAndDestroy()
    {
        // ���� ũ��� ��ǥ ũ��
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * targetScale;

        float elapsed = 0f;

        // Ŀ���� ����
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        // lifeTime�� growDuration���� ũ�ٸ� ���� �ð� ���
        if (lifeTime > growDuration)
        {
            yield return new WaitForSeconds(lifeTime - growDuration);
        }

        // ������Ʈ �ı�
        Destroy(gameObject);
    }
}
