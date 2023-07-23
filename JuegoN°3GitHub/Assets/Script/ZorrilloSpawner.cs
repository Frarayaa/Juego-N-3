using UnityEngine;

public class ZorrilloSpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo a generar
    public float spawnRadius; // Radio de distancia para comprobar si hay enemigos cerca
    public float spawnDelay; // Tiempo de espera entre generaciones de enemigos
    private bool canSpawn = true; // Bandera para controlar si se puede generar un nuevo enemigo
    private bool playerPresent = false; // Bandera para rastrear si el jugador está presente

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPresent = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPresent = false;
        }
    }

    private void Update()
    {
        if (!canSpawn || !playerPresent)
        {
            // Si no se puede generar enemigo o el jugador no está presente, salimos del método
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, spawnRadius);
        bool hasEnemiesNearby = false;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Zorrillo"))
            {
                hasEnemiesNearby = true;
                break;
            }
        }

        if (hasEnemiesNearby)
        {
            // Si hay enemigos cerca, no generamos un nuevo enemigo y reiniciamos el temporizador
            StartCoroutine(ResetSpawnDelay());
            return;
        }

        // Si no hay enemigos cerca, generamos un nuevo enemigo y reiniciamos el temporizador
        StartCoroutine(SpawnEnemy());
        StartCoroutine(ResetSpawnDelay());
    }

    private System.Collections.IEnumerator SpawnEnemy()
    {
        // Desactivamos la generación de enemigos
        canSpawn = false;

        // Generamos el enemigo en la posición del spawner
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // Esperamos un tiempo antes de activar la generación de nuevo
        yield return new WaitForSeconds(spawnDelay);

        // Activamos la generación de nuevo
        canSpawn = true;
    }

    private System.Collections.IEnumerator ResetSpawnDelay()
    {
        // Reiniciamos el temporizador
        canSpawn = false;
        yield return new WaitForSeconds(spawnDelay);
        canSpawn = true;
    }
}
