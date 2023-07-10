using UnityEngine;
using UnityEngine.UI;

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
    public int cantidadFlechas = 0;
    public int maxCantidadFlechas = 10;
    public bool hasSword = false;
    public GameObject swordPrefab;
    public GameObject picotaPrefab;
    public GameObject hachaPrefab;
    public bool isInWoodArea = false;
    public bool isInStoneArea = false;
    public bool hasPicota = false;
    public bool hasHacha = false;
    public int cantidadMadera = 0;
    public int cantidadPiedra = 0;
    public int maxMadera = 10;
    public int maxPiedra = 10;
    public int healingItem = 0;
    public int maxHealingItem = 3;
    private bool isGrounded = false;
    private Vector3 Position;
    private Vector3 respawnPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D characterCollider;
    private Rigidbody2D rb;
    private float attackTimer = 0f;
    public Text arrowText;
    public Text woodText;
    public Text stoneText;
    public Text healingItemText;
    private float moveHorizontal;
    private float moveVertical;

    public LifeIndicator lifeIndicator;
    public GameManager gm;
    
    // Variables para el arco
    public bool hasBow = false;
    public GameObject arrowPrefab; // Corregido: ahora se declara p�blicamente
    public Transform rectangleCenter; // Transform espec�fico para la posici�n del rect�ngulo
    public Vector2 dimensions = new Vector2(2.0f, 1.0f);
    // Variables para la carga de disparo
    public float cargaTiempo = 2f;
    private float tiempoPasadoCarga = 0f;
    private bool estaCargando = false;

    private bool isFacingRight = true; // Variable para almacenar la direcci�n actual del personaje
    private Animator animator;
    private bool isAttacking = false;
    private bool isShooting = false;
    private bool isMining = false;
    private bool isCutting = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = maxHealth;
        lifeIndicator.Life();
        respawnPosition = transform.position;
        gm.pp.LoadProgress();
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
            RecolectarMadera();
            isCutting = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && hasPicota && isInStoneArea)
        {
            RecolectarPiedra();
            isMining = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CrearFlecha();
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
                        if (cantidadFlechas > 0)
                        {
                            isShooting = true;
                            cantidadFlechas--;
                            UpdateArrowIndicator();
                            AimAndShoot();
                            attackTimer = attackCooldown;
                            Debug.Log("Has disparado una flecha. Flechas restantes: " + cantidadFlechas);
                        }
                        else
                        {
                            Debug.Log("No tienes m�s flechas. Debes crear m�s.");
                        }
                    }
                }
            }

            // Check for left mouse button click
            if (Input.GetMouseButtonDown(0))
            {
                // Perform left click attack if the attack cooldown is over
                if (attackTimer <= 0f)
                {
                    // Check if the character has a sword
                    if (hasSword)
                    {
                        SwordAttack();
                        isAttacking = true;
                    }

                    attackTimer = attackCooldown;
                }
            }

            if (estaCargando)
            {
                tiempoPasadoCarga += Time.deltaTime;

                // Aqu� puedes agregar visualizaciones o efectos para representar la carga, como una barra de progreso
                // Tambi�n puedes bloquear el movimiento del personaje mientras se carga el disparo

                if (tiempoPasadoCarga >= cargaTiempo)
                {
                    // La carga est� completa, dispara
                    Disparar();
                }
            }

            // Flip the character if moving in the opposite direction
            if (moveHorizontal > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveHorizontal < 0 && isFacingRight)
            {
                Flip();
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (healingItem > 0)
            {
                // Llama a una funci�n de curaci�n aqu�
                CurePlayer();
                healingItem--;
                gm.pp.healingItem = healingItem;
                gm.pp.SaveProgress();
                UpdateHealingItemIndicator();
            }
            else
            {
                Debug.Log("No tienes �tems de curaci�n disponibles.");
            }
        }
    }

    private void Flip()
    {
        // Cambia la direcci�n del personaje multiplicando la escala en el eje X por -1
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // Actualiza la variable de direcci�n del personaje
        isFacingRight = !isFacingRight;
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

        if (other.CompareTag("Ubicaci�n"))
        {
            Position = transform.position;
            gm.pp.SavePlayerPosition(Position);
            Debug.Log(Position.ToString());
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

    public void RecolectarMadera()
    {
        if (cantidadMadera < maxMadera)
        {
            cantidadMadera++;
            gm.pp.cantidadMadera = cantidadMadera;
            gm.pp.SaveProgress();
            UpdateWoodIndicator();
            isCutting = false;
            Debug.Log("Has recolectado 1 unidad de madera. Total de madera: " + cantidadMadera);
        }
    }

    public void RecolectarPiedra()
    {
        if (cantidadPiedra < maxPiedra)
        {
            cantidadPiedra++;
            gm.pp.cantidadPiedra = cantidadPiedra;
            gm.pp.SaveProgress();
            UpdateStoneIndicator();
            isMining = false;
            Debug.Log("Has recolectado 1 unidad de piedra. Total de piedra: " + cantidadPiedra);
        }
    }

    public void CrearFlecha()
    {
        if (cantidadMadera >= 1 && cantidadPiedra >= 1 && cantidadFlechas < maxCantidadFlechas)
        {
            cantidadMadera -= 1;
            cantidadPiedra -= 1;
            cantidadFlechas += 1;
            gm.pp.cantidadMadera = cantidadMadera;
            gm.pp.cantidadPiedra = cantidadPiedra;
            gm.pp.cantidadFlechas = cantidadFlechas;
            gm.pp.SaveProgress();
            UpdateArrowIndicator();
            UpdateStoneIndicator();
            UpdateWoodIndicator();

            // C�digo para crear una flecha
            // ...

            Debug.Log("Has creado una flecha. Total de flechas: " + cantidadFlechas);
        }
        else if (cantidadFlechas >= maxCantidadFlechas)
        {
            Debug.Log("No puedes crear m�s flechas. Has alcanzado el l�mite m�ximo.");
        }
        else
        {
            Debug.Log("No tienes suficientes materiales para crear una flecha.");
        }
    }

    private void UpdateArrowIndicator()
    {
        arrowText.text = "Flechas: " + cantidadFlechas;
    }

    private void UpdateWoodIndicator()
    {
        woodText.text = "Madera: " + cantidadMadera;
    }

    private void UpdateStoneIndicator()
    {
        stoneText.text = "Piedra: " + cantidadPiedra;
    }
    private void UpdateHealingItemIndicator()
    {
        healingItemText.text = "Poci�n de curaci�n: " + healingItem;
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
        isShooting = false;
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
        lifeIndicator.Life();
        gm.pp.health = health;

        if (health <= 0)
        {
            health = maxHealth;
            gm.ShowDeathScreen();
        }
    }

    private void CurePlayer()
    {
        // Aumenta la salud del jugador en una cantidad espec�fica
        int cantidadCuracion = 3; // Ajusta este valor seg�n tus necesidades
        health += cantidadCuracion;
        lifeIndicator.Life();
        gm.pp.health = health;

        // Aseg�rate de que la salud no exceda el m�ximo
        if (health > maxHealth)
        {
            health = maxHealth;
            lifeIndicator.Life();
            gm.pp.health = health;
        }
    }

    public void RecolectarItemCuracion()
    {
        if (healingItem < maxHealingItem)
        {
            healingItem++;
            gm.pp.healingItem = healingItem;
            UpdateHealingItemIndicator();
            gm.pp.SaveProgress();
            Debug.Log("Has recogido un �tem de curaci�n. Total de �tems de curaci�n: " + healingItem);
        }
        else
        {
            Debug.Log("No puedes recoger m�s �tems de curaci�n. Has alcanzado el l�mite m�ximo.");
        }
    }

    public void Respawn()
    {
        transform.position = respawnPosition;
        health = maxHealth;
        lifeIndicator.Life();
        gm.pp.health = health;
        gm.pp.LoadProgress();
        gm.pp.SaveProgress();
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        respawnPosition = checkpointPosition;
    }

    public void GetBow(GameObject bowPrefab)
    {
        hasBow = true;
        arrowPrefab = bowPrefab;
        gm.pp.hasBow = hasBow;
        gm.pp.SaveProgress();
    }

    public void GetPicota(GameObject picota)
    {
        hasPicota = true;
        picotaPrefab = picota;
        gm.pp.hasPicota = hasPicota;
        gm.pp.SaveProgress();
    }

    public void GetHacha(GameObject hacha)
    {
        hasHacha = true;
        hachaPrefab = hacha;
        gm.pp.hasHacha = hasHacha;
        gm.pp.SaveProgress();
    }

    private void SwordAttack()
    {
        // Perform sword attack logic here
        // This can be a melee attack animation or any other action you want
        // For example, let's perform a simple debug log message
        Debug.Log("Performing sword attack!");

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(rectangleCenter.position, dimensions, 0f);
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
            BossController boss = enemyObject.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damageAmount);
            }
        }
        isAttacking = false;
    }

    private void LateUpdate()
    {
        animator.SetBool("Idle", moveHorizontal == 0 && moveVertical == 0);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsShooting", isShooting);
        animator.SetBool("IsMining", isMining);
        animator.SetBool("IsCutting", isCutting);
    }

    private void OnDrawGizmos()
    {
        if (rectangleCenter != null)
        {
            Gizmos.color = Color.red;

            // Calcula las esquinas del rect�ngulo
            Vector3 bottomLeft = rectangleCenter.position - new Vector3(dimensions.x, dimensions.y, 0) * 0.5f;
            Vector3 bottomRight = rectangleCenter.position + new Vector3(dimensions.x, -dimensions.y, 0) * 0.5f;
            Vector3 topLeft = rectangleCenter.position + new Vector3(-dimensions.x, dimensions.y, 0) * 0.5f;
            Vector3 topRight = rectangleCenter.position + new Vector3(dimensions.x, dimensions.y, 0) * 0.5f;

            // Dibuja las l�neas del rect�ngulo
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }

    public void GetSword(GameObject sword)
    {
        hasSword = true;
        swordPrefab = sword;
        gm.pp.hasSword = hasSword;
        gm.pp.SaveProgress();
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

    public void Load(PlayerProgress pp)
    {
        health = gm.pp.health;
        healingItem = gm.pp.healingItem;
        cantidadMadera = gm.pp.cantidadMadera;
        cantidadPiedra = gm.pp.cantidadPiedra;
        cantidadFlechas = gm.pp.cantidadFlechas;

        if (gm.backToLand == false)
        {
            gm.LoadPlayerPosition();
        }

        if (pp.hasBow)
        {
            GetBow(arrowPrefab);

        }
        if (pp.hasHacha)
        {
            GetHacha(hachaPrefab);
        }
        if (pp.hasPicota)
        {
            GetPicota(picotaPrefab);
        }
        if (pp.hasSword)
        {
            GetSword(swordPrefab);
        }
    }
}
