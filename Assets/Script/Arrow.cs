using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float speed;
    private float damage;
    private Vector2 shootDirection;
    private Rigidbody2D arrowRigidbody;

    internal void Initialize(Vector2 shootDirection, float baseDamage, float speed)
    {
        this.speed = speed;
        damage = baseDamage;
        this.shootDirection = shootDirection;
        arrowRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() 
    {
        arrowRigidbody.velocity = shootDirection * speed;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        arrowRigidbody.rotation = angle + -90f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            collision.gameObject.GetComponent<IDamageable>();
            this.gameObject.SetActive(false);
        }
    }
}
