using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    public int maxHealth = 5;
    public int damageAmount = 1;
    public float attackCooldown = 2f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public LayerMask playerLayer;
    public float detectionRadius = 10f;
    public float attackRadius = 5f;
    public GameObject exclamacion;

    private int health;
    private float attackTimer = 0f;
    private GameObject player;
    private Animator animator;
    private bool IsShooting = false;

    private void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // Perform attack if the attack cooldown is over and the player is within the attack radius
        if (attackTimer <= 0f && DetectPlayer(attackRadius))
        {
            Attack();
            IsShooting = true;
            UpdateAnimations();

            attackTimer = attackCooldown;
        }
        else
        {
            UpdateAnimations();
            IsShooting = false; // Set isShooting to false if the player is not within the attack radius

        }

        if (DetectPlayer(attackRadius))
        {
            exclamacion.SetActive(true);
        }
        else
        {
            exclamacion.SetActive(false);
        }

    }

    private void Attack()
    {
        // Calculate the direction towards the player
        Vector2 direction = player.transform.position - transform.position;

        // Normalize the direction to get a unit vector
        direction.Normalize();

        // Instantiate a bullet prefab at the fire point position and with the calculated direction
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        // Set the bullet's damage amount and direction
        if (bulletScript != null)
        {
            bulletScript.SetDamageAmount(damageAmount);
            bulletScript.SetDirection(direction);
            IsShooting = false;
        }
    }

    private bool DetectPlayer(float radius)
    {
        if (player != null)
        {
            // Check if the player is within the detection radius
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= detectionRadius)
            {
                // Raycast from the turret to the player to check if there are any obstacles in between
                RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, distance, playerLayer);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamageFromArrow()
    {
        Destroy(gameObject);
    }
    private void UpdateAnimations()
    {
        animator.SetBool("IsShooting", IsShooting);
    }
}