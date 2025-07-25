using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpwaner : MonoBehaviour
{
    [SerializeField] private Monster monster;
    private ObjectPool<Monster> monsterPool;
    [SerializeField] private GameObject target;
    public bool isSpawn = false;

    private int count = 0;

    [SerializeField] private float spawnDelay = 1.5f; // 스폰 간격 (초)
    private float spawnTimer = 0f;

    private void Start()
    {
        monsterPool = new ObjectPool<Monster>(monster, 5, transform);
        SpwanSystem.Instance.spwanSystems.Add(this);
    }

    private void Update()
    {
        if (isSpawn)
        {
            if (count >= 5)
            {
                isSpawn = false;
                return; // 5마리 초과 시 스폰 중단
            }
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
}
