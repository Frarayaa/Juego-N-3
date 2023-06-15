using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damageAmount = 1;
    public float lifetime = 3f;
    private Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the bullet in the set direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    public void SetDamageAmount(int amount)
    {
        damageAmount = amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bullet collides with the player
        if (collision.CompareTag("Player"))
        {
            Character player = collision.GetComponent<Character>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }

            Destroy(gameObject);
        }
    }
}
