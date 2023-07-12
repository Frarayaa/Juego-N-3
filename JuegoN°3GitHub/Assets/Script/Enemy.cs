using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public enum EnemyMode
    {
        TopDown,
        SideScroll
    }

    public EnemyMode enemyMode = EnemyMode.TopDown;
    public float chaseSpeed = 3f;
    public float patrolSpeed = 2f;
    public float attackDistance = 1f;
    public int damageAmount = 1;
    public float chaseDuration = 5f;
    public float damageCooldown = 1f;
    public int maxHealth = 50;
    private Animator animator;
    public GameObject triangleVisual;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.gray;

    public LayerMask playerLayer;
    public float detectionRadius = 5f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isChasing = false;
    private bool isPatrolling = false;
    private float chaseTimer = 0f;
    private float damageTimer = 0f;
    private Vector3 patrolStartPosition;
    private Vector3 patrolTargetPosition;
    private Vector2 initialScale;
    private bool isMoving = false;

    private PolygonCollider2D detectionCollider;

    // TopDown patrol area
    public float patrolAreaRadiusTopDown = 5f;
    public Vector3 patrolAreaCenterTopDown;

    // SideScroll patrol area
    public float patrolAreaWidthSideScroll = 5f;
    public float patrolAreaHeightSideScroll = 5f;
    public float patrolAreaCenterXSideScroll = 0f;
    public float patrolAreaCenterYSideScroll = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        detectionCollider = triangleVisual.GetComponent<PolygonCollider2D>();
        maxHealth = 1;
        patrolAreaCenterTopDown = transform.position;
        initialScale = transform.localScale;
        patrolStartPosition = transform.position;
        GeneratePatrolTargetPosition();
        animator = GetComponent<Animator>();

        triangleVisual.SetActive(true);
        triangleVisual.GetComponent<SpriteRenderer>().color = inactiveColor;
    }

    private void Update()
    {
        bool isPlayerVisible = detectionCollider.IsTouchingLayers(playerLayer);

        if (isPlayerVisible && damageTimer <= 0f)
        {
            ChasePlayer();
            triangleVisual.GetComponent<SpriteRenderer>().color = activeColor;
        }
        else
        {
            triangleVisual.GetComponent<SpriteRenderer>().color = inactiveColor;

            if (enemyMode == EnemyMode.TopDown)
            {
                PatrolTopDown();
            }
            else if (enemyMode == EnemyMode.SideScroll)
            {
                PatrolSideScroll();
            }
        }

        if (Vector2.Distance(transform.position, player.position) <= attackDistance)
        {
            AttackPlayer();
        }

        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }

        if (isChasing)
        {
            chaseTimer += Time.deltaTime;

            if (chaseTimer >= chaseDuration)
            {
                StopChase();
            }
        }
    }

    private void StartChase()
    {
        isChasing = true;
        spriteRenderer.color = Color.red;
        chaseTimer = 0f;
        maxHealth = 50;
    }

    private void StopChase()
    {
        isChasing = false;
        spriteRenderer.color = Color.white;
        maxHealth = 50;
    }

    private void ChasePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        transform.position += direction * chaseSpeed * Time.deltaTime;
        FlipSprite(direction.x);
        isMoving = true;
    }

    private void PatrolTopDown()
    {
        if (!isPatrolling)
        {
            StartCoroutine(StartPatrolTopDown());
        }

        Vector3 direction = patrolTargetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance <= 0.1f)
        {
            GeneratePatrolTargetPosition();
        }

        direction.Normalize();
        transform.position += direction * patrolSpeed * Time.deltaTime;
        FlipSprite(direction.x);
        isMoving = true;
    }

    private IEnumerator StartPatrolTopDown()
    {
        isPatrolling = true;

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        GeneratePatrolTargetPosition();

        isPatrolling = false;
    }

    private void PatrolSideScroll()
    {
        if (!isPatrolling)
        {
            StartCoroutine(StartPatrolSideScroll());
        }

        Vector3 direction = patrolTargetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance <= 0.1f)
        {
            GeneratePatrolTargetPosition();
        }

        direction.Normalize();
        transform.position += direction * patrolSpeed * Time.deltaTime;
        FlipSprite(direction.x);
        isMoving = true;
    }

    private IEnumerator StartPatrolSideScroll()
    {
        isPatrolling = true;

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        GeneratePatrolTargetPosition();

        isPatrolling = false;
    }

    private void GeneratePatrolTargetPosition()
    {
        if (enemyMode == EnemyMode.TopDown)
        {
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            float randomDistance = Random.Range(0f, patrolAreaRadiusTopDown);
            float randomX = patrolAreaCenterTopDown.x + Mathf.Cos(randomAngle) * randomDistance;
            float randomY = patrolAreaCenterTopDown.y + Mathf.Sin(randomAngle) * randomDistance;
            patrolTargetPosition = new Vector3(randomX, randomY, 0f);
        }
        else if (enemyMode == EnemyMode.SideScroll)
        {
            float randomX = Random.Range(patrolAreaCenterXSideScroll - patrolAreaWidthSideScroll / 2f, patrolAreaCenterXSideScroll + patrolAreaWidthSideScroll / 2f);
            float randomY = Random.Range(patrolAreaCenterYSideScroll - patrolAreaHeightSideScroll / 2f, patrolAreaCenterYSideScroll + patrolAreaHeightSideScroll / 2f);
            patrolTargetPosition = new Vector3(randomX, randomY, 0f);
        }
    }

    private void AttackPlayer()
    {
        if (damageTimer <= 0f)
        {
            Character character = player.GetComponent<Character>();
            if (character != null)
            {
                character.TakeDamage(damageAmount);
                damageTimer = damageCooldown;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;

        if (maxHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0f)
        {
            transform.localScale = new Vector2(-initialScale.x, initialScale.y);
        }
        else if (directionX > 0f)
        {
            transform.localScale = new Vector2(initialScale.x, initialScale.y);
        }
    }

    private void LateUpdate()
    {
        animator.SetBool("Idle", !isMoving);
        isMoving = false; // Reiniciar el indicador de movimiento
    }
}
