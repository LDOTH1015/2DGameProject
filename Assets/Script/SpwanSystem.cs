using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanSystem : Singleton<SpwanSystem>
{
    private int countStage;
    public List<MonsterSpwaner> spwanSystems = new List<MonsterSpwaner>();
    void Start()
    {
        countStage = 0;
    }

    public void StartSpwaner()
    {
        foreach (var spwaner in spwanSystems)
        {
            spwaner.isSpawn = true;
        }
        countStage++;
    }
}
