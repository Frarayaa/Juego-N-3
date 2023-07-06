using UnityEngine;

public class BossController : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float meleeAttackRange = 2f;
    public float chargeAttackRange = 5f;
    public float chargeAttackDuration = 3f;
    public float idleDuration = 2f;
    public float chaseLimit = 10f;
    public float meleeAttackCooldown = 2f; // Tiempo de enfriamiento entre ataques cuerpo a cuerpo
    private float meleeAttackTimer; // Temporizador para el tiempo transcurrido desde el último ataque cuerpo a cuerpo
    public float chargeAttackCooldown = 2f; // Tiempo de enfriamiento entre ataques cuerpo a cuerpo
    private float chargeAttackTimer;
    public int normalAttackDamage = 2;
    public int chargedAttackDamage = 4;
    public int maxHealth = 10;
    public GameObject damageAreaPrefab;
    private Transform player;
    private bool isChargingAttack;
    private bool isIdle;
    private float idleTimer;
    private int currentHealth;
    private Vector3 originalPosition;
    private bool isChasing;
    public Transform meleeAttackArea;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = transform.position;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isChargingAttack && !isIdle)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= chaseLimit)
            {
                Vector3 direction = player.position - transform.position;
                direction.Normalize();
                transform.position += direction * movementSpeed * Time.deltaTime;
                isChasing = true;
            }
            else if (isChasing)
            {
                currentHealth = maxHealth;
                isChasing = false;
                ReturnToOrigin();
            }

            if (distanceToPlayer <= meleeAttackRange)
            {
                if (meleeAttackTimer <= 0f)
                {
                    MeleeAttack();
                    meleeAttackTimer = meleeAttackCooldown; // Reinicia el temporizador de enfriamiento
                }
                else
                {
                    Debug.Log("Ataque cuerpo a cuerpo en enfriamiento. Espere un momento.");
                }
            }

            if (meleeAttackTimer > 0f)
            {
                meleeAttackTimer -= Time.deltaTime;
            }

            if (distanceToPlayer <= chargeAttackRange)
            {
                if (chargeAttackTimer <= 0f)
                {
                    ChargeAttack();
                    chargeAttackTimer = chargeAttackCooldown;
                }
                
            }
            
            if (chargeAttackTimer > 0f)
            {
                chargeAttackTimer -= Time.deltaTime;
            }
        }

        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdle = false;
            }
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("Ataque cuerpo a cuerpo");

        Vector2 areaSize = meleeAttackArea.localScale;
        Vector2 areaPosition = meleeAttackArea.position;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(areaPosition - areaSize / 2f, areaPosition + areaSize / 2f);

        foreach (Collider2D collider in colliders)
        {
            Character character = collider.GetComponent<Character>();
            if (character != null)
            {
                character.TakeDamage(normalAttackDamage);
            }
        }
    }


    private void ChargeAttack()
    {
        Debug.Log("Ataque cargado");

        isChargingAttack = true;

        Invoke("FinishChargeAttack", chargeAttackDuration);
    }

    private void FinishChargeAttack()
    {
        isChargingAttack = false;

        CreateDamageArea();

        isIdle = true;
        idleTimer = idleDuration;
    }

    private void ReturnToOrigin()
    {
        transform.position = originalPosition;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void CreateDamageArea()
    {
        // Instanciar el prefab del área de daño en la posición del enemigo
        Instantiate(damageAreaPrefab, transform.position, Quaternion.identity);
    }

    private void Die()
    {
        Debug.Log("El jefe ha muerto");
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseLimit);
    }
}
