using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    private TopDownController controller;
    private Rigidbody2D movementRigidbody;
    protected Animator animator;

    public Arrow arrow;
    private ObjectPool<Arrow> arrowPool;

    private Vector2 movementDirection = Vector2.zero;
    private bool isEvasion = false;

    private float evasionDuraion = 0.2f;
    private float evationSpeed = 16.0f;

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        animator = GetComponentInChildren<Animator>();
        movementRigidbody = GetComponent<Rigidbody2D>();
        arrowPool = new ObjectPool<Arrow>(arrow, 5, transform);
    }

    private void Start()
    {
        controller.OnMoveEvent += Move;
        controller.OnEvasionEvent += Evasion;
        controller.OnFireEvent += Fire;
    }

    private void Move(Vector2 direction)
    {
        if (isEvasion) return;
        movementDirection = direction;
    }

    private void Fire()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector2 shootDirection = (nearestEnemy.transform.position - this.transform.position).normalized;
            Arrow projectile = arrowPool.Get(this.transform.position, Quaternion.identity);
            //projectile.Initialize(shootDirection, PlayerStatus.Instance.baseDamage, State.Range);
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");
        GameObject nearestEnemy = null;
        float shortestDistance = 10f;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    private void Evasion()
    {
        if (isEvasion) return;

        //추후 스테미너를 사용하여 회피를 위한 스테미너 양 체크 코드
        if (true)
        {
            Debug.Log(movementDirection);
            StartCoroutine(EvasionCoroutine(movementDirection));
        }
    }

    private IEnumerator EvasionCoroutine(Vector2 direction)
    {
        isEvasion = true;

        float elapsed = 0f;

        while (elapsed < evasionDuraion)
        {
            movementRigidbody.velocity = direction * evationSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }
        movementRigidbody.velocity = Vector2.zero;
        isEvasion = false;
    }

    private void FixedUpdate()
    {
        if (!isEvasion) 
        {
            ApplyMovement(movementDirection);
        }
    }

    private void ApplyMovement(Vector2 direction)
    {
        if (movementRigidbody.velocity != direction)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }
        direction = direction * 5;
        
        movementRigidbody.velocity = direction;
    }
}
