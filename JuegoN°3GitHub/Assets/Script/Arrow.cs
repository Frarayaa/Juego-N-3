using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damageAmount = 1;
    public float speed = 10f;
    public float lifetime = 2f; // Tiempo de vida en segundos

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime); // Destruye la flecha después del tiempo de vida
    }

    private void Update()
    {
        // Mueve la flecha hacia adelante en la dirección establecida
        rb.velocity = direction.normalized * speed;

        // Calcula el ángulo de rotación hacia la dirección de movimiento
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica la rotación a la flecha
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
            TurretEnemy turretEnemy = collision.GetComponent<TurretEnemy>();
            if (turretEnemy != null)
            {
                turretEnemy.TakeDamage(damageAmount);
            }
            TrampaOso trampa = collision.GetComponent<TrampaOso>();
            if (trampa != null)
            {
                trampa.TakeDamage(damageAmount);
            }
            ZorrilloController zorrillo = collision.GetComponent<ZorrilloController>();
            if (zorrillo != null)
            {
                zorrillo.TakeDamage(damageAmount);
            }
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
        else
        {
            // Ignora colisiones con otros objetos y no destruye la flecha
            return;
        }
    }
}

