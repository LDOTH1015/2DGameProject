using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float damage;
    private Rigidbody2D arrowRigidbody;

    internal void Initialize(Vector2 shootDirection, float baseDamage, float speed)
    {
        this.damage = baseDamage;

        arrowRigidbody = GetComponent<Rigidbody2D>();

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        arrowRigidbody.rotation = angle - 90f;

        arrowRigidbody.velocity = shootDirection * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            this.gameObject.SetActive(false);
        } 
        else if(collision.gameObject.CompareTag("Topography"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
