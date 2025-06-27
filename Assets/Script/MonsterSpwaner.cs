using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpwaner : MonoBehaviour
{
    [SerializeField] private Monster monster;
    private ObjectPool<Monster> monsterPool;
    [SerializeField] private GameObject target;

    private int count = 0;

    [SerializeField] private float spawnDelay = 1.5f; // ���� ���� (��)
    private float spawnTimer = 0f;

    private void Start()
    {
        monsterPool = new ObjectPool<Monster>(monster, 5, transform);
    }

    private void Update()
    {
        if (count >= 5) return; // 5���� �ʰ� �� ���� �ߴ�

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnDelay)
        {
            spawnTimer = 0f;

            Monster m = Instantiate(monster, transform.position, Quaternion.identity);
            m.Initialize(target);
            count++;
        }
    }
}
