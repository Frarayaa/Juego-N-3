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
    public float meleeAttackCooldown = 2f;
    private float meleeAttackTimer;
    public float chargeAttackCooldown = 2f;
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

    private bool isDead = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = transform.position;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead)
        {
            return; // Si el jefe está muerto, detener la lógica de actualización.
        }

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
                    meleeAttackTimer = meleeAttackCooldown;
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

        Collider2D[] colliders = Physics2D.OverlapCircleAll(meleeAttackArea.position, 3f);

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
        StartCoroutine(ChangeDamageAreaColor(spriteRenderer, Color.clear, Color.clear, 1f));
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
        isDead = true;
        Debug.Log("IsDead set to true.");
        StartCoroutine(DieWithDelay(0.8f)); // Espera 3 segundos antes de desactivar el objeto
    }

    private IEnumerator DieWithDelay(float delay)
    {
        // Desencadenar la animación de muerte
        animator.SetBool("IsDead", true);
        yield return new WaitForSeconds(delay); // Esperar el tiempo de duración de la animación de muerte
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
        animator.SetBool("IsDead", isDead); // Asegurarse de que el Animator sepa cuando el jefe está muerto.
    }
}