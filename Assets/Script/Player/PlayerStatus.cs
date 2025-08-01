using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Singleton<PlayerStatus>
{
    public float maxHp = 200;
    public float curruntHP = 200;
    public float maxStamina = 200;
    public float curruntStamina = 200;
    public float attackPower;
    public float defense;
    public float evasion;
    public float criticalChance;
    public float criticalDamage;
    public float moveSpeed;
    public float resistance;

    private void Update()
    {
        if (curruntStamina <= maxStamina)
            curruntStamina += 0.1f;
        if (curruntHP <= maxHp)
            curruntHP += 0.1f;
    }
}
