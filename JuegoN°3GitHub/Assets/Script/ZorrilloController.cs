using UnityEngine;

public class ZorrilloController : MonoBehaviour
{
    public float movementSpeed = 2f;  // Velocidad de movimiento del enemigo
    public float escapeSpeed = 4f;    // Velocidad de escape después de atacar
    public float attackCooldown = 3f; // Tiempo antes de que pueda atacar nuevamente
    public int maxHealth = 5;
    public GameObject damageAreaPrefab;  // Prefab del área de daño

    private Vector3 originalPosition;  // Posición original del enemigo
    private bool isEscaping = false;   // Indicador de escape activado
    private float attackTimer = 0f;    // Temporizador para el ataque
    private Character character;       // Referencia al personaje

    // Definir el área de movimiento
    public float movementAreaSize = 4f;
    private Vector3 movementAreaCenter;
    private Vector3 targetPosition;

    private void Start()
    {
        originalPosition = transform.position; // Guardar la posición original
        character = FindObjectOfType<Character>(); // Encontrar el componente Character en el escenario

        // Calcular el centro del área de movimiento
        movementAreaCenter = originalPosition;

        // Establecer una posición de destino inicial
        targetPosition = GetRandomPositionInMovementArea();
    }

    private void Update()
    {
        if (!isEscaping)
        {
            // Moverse hacia la posición de destino dentro del área determinada
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

            // Si el enemigo llega a la posición de destino, establecer una nueva posición de destino aleatoria
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                targetPosition = GetRandomPositionInMovementArea();
            }
        }
        else
        {
            // Escapar en dirección opuesta al jugador
            Vector3 direction = (transform.position - character.transform.position).normalized;
            transform.Translate(direction * escapeSpeed * Time.deltaTime);
        }

        // Verificar si debe atacar
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            // Realizar el ataque
            if (IsPlayerNearby())
            {
                CreateDamageArea();
                attackTimer = attackCooldown;
                isEscaping = true;
                Invoke("Disappear", 2f); // Llamar a la función "Disappear" después de 2 segundos
            }
        }
    }

    private bool IsPlayerNearby()
    {
        // Verificar si el jugador está cerca del enemigo (puedes ajustar la distancia según tus necesidades)
        float distance = Vector3.Distance(transform.position, character.transform.position);
        return distance <= 2f;
    }

    private void CreateDamageArea()
    {
        // Instanciar el prefab del área de daño en la posición del enemigo
        Instantiate(damageAreaPrefab, transform.position, Quaternion.identity);
    }

    private void Disappear()
    {
        // Destruir el enemigo después de realizar el ataque y escapar
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;

        if (maxHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private Vector3 GetRandomPositionInMovementArea()
    {
        // Generar una posición aleatoria dentro del área de movimiento
        float randomX = Random.Range(-movementAreaSize, movementAreaSize);
        float randomY = Random.Range(-movementAreaSize, movementAreaSize);
        return movementAreaCenter + new Vector3(randomX, randomY, 0f);
    }
}
