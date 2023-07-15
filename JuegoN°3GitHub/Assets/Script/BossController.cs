using UnityEngine;
using System.Collections;

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
    public GameObject meleeAreaPrefab;
    public GameObject damageAreaPrefab;
    private Transform player;
    private bool isChargingAttack;
    private bool isIdle;
    private float idleTimer;
    public int currentHealth;
    private Vector3 originalPosition;
    private bool isChasing;
    public Transform meleeAttackArea;
    private Animator animator;
    private bool isAttacking = false;
    private bool isCharging = false;
    public Final fin;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = transform.position;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
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
                    isAttacking = true;
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
                    isCharging = true;
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
        GameObject meleeArea = Instantiate(meleeAreaPrefab, transform.position, Quaternion.identity);

        // Obtener el componente SpriteRenderer del área de daño
        SpriteRenderer spriteRenderer = meleeArea.GetComponent<SpriteRenderer>();

        // Cambiar gradualmente el color de amarillo a rojo
        StartCoroutine(ChangeDamageAreaColor(spriteRenderer, Color.gray, Color.gray, 1f));
    }

    private IEnumerator ChangeDamageAreaColor(SpriteRenderer spriteRenderer, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Establecer el color final de ataque
        spriteRenderer.color = endColor;

        Destroy(spriteRenderer.gameObject);
        isAttacking = false;
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
        isCharging = false;
    }

    private void Die()
    {
        Debug.Log("El jefe ha muerto");
        gameObject.SetActive(false);
        fin.destruir = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseLimit);
    }
    private void LateUpdate()
    {
        animator.SetBool("Idle", !isChasing);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsCharging", isCharging);
        animator.SetBool("IsChasing", isChasing);
    }
}
