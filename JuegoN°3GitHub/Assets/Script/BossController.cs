using UnityEngine;

public class BossController : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float meleeAttackRange = 2f;
    public float chargeAttackRange = 5f;
    public float chargeAttackDuration = 3f;
    public float idleDuration = 2f;
    public int maxHealth = 50;
    public int normalAttackDamage = 2; // Daño del ataque normal
    public int chargedAttackDamage = 4; // Daño del ataque cargado

    private Transform player;
    private bool isChargingAttack;
    private bool isIdle;
    private float idleTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!isChargingAttack && !isIdle)
        {
            Vector3 direction = player.position - transform.position;
            direction.Normalize();
            transform.position += direction * movementSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, player.position) <= meleeAttackRange)
            {
                MeleeAttack();
            }

            if (Vector3.Distance(transform.position, player.position) <= chargeAttackRange)
            {
                ChargeAttack();
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

        for (int i = 0; i < 3; i++)
        {
            // Realiza el daño del ataque normal al jugador
            player.GetComponent<Character>().TakeDamage(normalAttackDamage);
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

        // Realiza el daño del ataque cargado al jugador
        player.GetComponent<Character>().TakeDamage(chargedAttackDamage);

        isIdle = true;
        idleTimer = idleDuration;
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;

        if (maxHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
