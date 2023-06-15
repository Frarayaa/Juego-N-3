using UnityEngine;

public enum GameMode
{
    TopDown,
    SideScroll
}

public class Character : MonoBehaviour
{
    public GameMode gameMode;
    public float moveSpeedTopDown = 5f;
    public float moveSpeedSideScroll = 5f;
    public int maxHealth = 10;
    public int health;
    public int damageAmount = 1;
    public float attackCooldown = 1f;
    public float jumpForce = 5f;
    public float arrowSpeed = 10f;
    public GameObject picotaPrefab;
    public GameObject hachaPrefab;
    public bool isInWoodArea = false;
    public bool isInStoneArea = false;
    private bool hasPicota = false;
    private bool hasHacha = false;
    private bool isGrounded = false;
    private Vector3 respawnPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D characterCollider;
    private Rigidbody2D rb;
    private float attackTimer = 0f;

    // Variables para el arco
    private bool hasBow = false;
    public GameObject arrowPrefab; // Corregido: ahora se declara públicamente

    // Variables para la carga de disparo
    public float cargaTiempo = 2f;
    private float tiempoPasadoCarga = 0f;
    private bool estaCargando = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        respawnPosition = transform.position;
    }

    private void Update()
    {
        // Update the attack cooldown timer
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && hasHacha && isInWoodArea)
        {
            Debug.Log("Uso hacha.");
            // Lógica de recolección de materiales con el hacha en el área de madera
            // ...
        }

        if (Input.GetKeyDown(KeyCode.Tab) && hasPicota && isInStoneArea)
        {
            Debug.Log("Uso picota.");
            // Lógica de recolección de materiales con la picota en el área de piedra
            // ...
        }

        // Set visibility and collision based on the current mode
        if (gameMode == GameMode.TopDown || gameMode == GameMode.SideScroll)
        {
            spriteRenderer.enabled = true;
            characterCollider.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;

            // Move the character based on the current mode
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            if (gameMode == GameMode.TopDown)
            {
                Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f) * moveSpeedTopDown * Time.deltaTime;
                transform.Translate(movement);
            }
            else if (gameMode == GameMode.SideScroll)
            {
                Vector3 movement = new Vector3(moveHorizontal, 0f, 0f) * moveSpeedSideScroll * Time.deltaTime;
                transform.Translate(movement);

                // Check for jump input
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    Jump();
                }
            }

            // Check for right mouse button click
            if (Input.GetMouseButtonDown(1))
            {
                // Perform attack if the attack cooldown is over
                if (attackTimer <= 0f)
                {
                    if (hasBow)
                    {
                        IniciarCargaDisparo();
                    }
                    else
                    {
                        Attack();
                    }
                    attackTimer = attackCooldown;
                }
            }

            // Check for left mouse button click
            if (Input.GetMouseButtonDown(0))
            {
                // Perform left click attack if the attack cooldown is over
                if (attackTimer <= 0f)
                {
                    LeftClickAttack();
                    attackTimer = attackCooldown;
                }
            }

            if (estaCargando)
            {
                tiempoPasadoCarga += Time.deltaTime;

                // Aquí puedes agregar visualizaciones o efectos para representar la carga, como una barra de progreso
                // También puedes bloquear el movimiento del personaje mientras se carga el disparo

                if (tiempoPasadoCarga >= cargaTiempo)
                {
                    // La carga está completa, dispara
                    Disparar();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WoodArea"))
        {
            isInWoodArea = true;
        }
        else if (other.CompareTag("StoneArea"))
        {
            isInStoneArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WoodArea"))
        {
            isInWoodArea = false;
        }
        else if (other.CompareTag("StoneArea"))
        {
            isInStoneArea = false;
        }
    }

    private void Attack()
    {
        // Perform attack logic here, such as dealing damage to enemies
        // For simplicity, let's assume enemies have a script called EnemyHealth
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D collider in hitColliders)
        {
            GameObject enemyObject = collider.gameObject;
            if (enemyObject != null)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageAmount);
                }
            }
            TrampaOso trampa = enemyObject.GetComponent<TrampaOso>();
            if (trampa != null)
            {
                trampa.TakeDamage(damageAmount);
            }
            ZorrilloController zorrillo = enemyObject.GetComponent<ZorrilloController>();
            if (zorrillo != null)
            {
                zorrillo.TakeDamage(damageAmount);
            }
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void AimAndShoot()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the character to the mouse position
        Vector2 direction = (mousePosition - transform.position).normalized;

        // Instantiate the arrow prefab at the character's position
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        // Get the Arrow component of the arrow prefab
        Arrow arrowScript = arrow.GetComponent<Arrow>();

        // Set the direction of the arrow
        arrowScript.SetDirection(direction);

        // Set the speed of the arrow
        arrowScript.speed = arrowSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Taking damage: " + damage);
        health -= damage;

        if (health <= 0)
        {
            health = maxHealth;
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = respawnPosition;
        health = maxHealth;
    }

    private void Die()
    {
        Respawn();
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        respawnPosition = checkpointPosition;
    }

    public void GetBow(GameObject bowPrefab)
    {
        hasBow = true;
        arrowPrefab = bowPrefab;
    }

    public void GetPicota(GameObject picota)
    {
        hasPicota = true;
        picotaPrefab = picota;
    }

    public void GetHacha(GameObject hacha)
    {
        hasHacha = true;
        hachaPrefab = hacha;
    }

    private void LeftClickAttack()
    {
        // Perform left click attack logic here
        // This can be a sweeping attack or any other action you want
        // For example, let's perform a simple debug log message
        Debug.Log("Performing left click attack!");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        foreach (Collider2D collider in hitColliders)
        {
            GameObject enemyObject = collider.gameObject;
            if (enemyObject != null)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageAmount);
                }
            }
            TrampaOso trampa = enemyObject.GetComponent<TrampaOso>();
            if (trampa != null)
            {
                trampa.TakeDamage(damageAmount);
            }
            ZorrilloController zorrillo = enemyObject.GetComponent<ZorrilloController>();
            if (zorrillo != null)
            {
                zorrillo.TakeDamage(damageAmount);
            }
        }
    }

    private void IniciarCargaDisparo()
    {
        // Inicia la carga del disparo
        tiempoPasadoCarga = 0f;
        estaCargando = true;
    }

    private void Disparar()
    {
        // Realiza el disparo del arco
        AimAndShoot();
        estaCargando = false;
        tiempoPasadoCarga = 0f;
    }
}
