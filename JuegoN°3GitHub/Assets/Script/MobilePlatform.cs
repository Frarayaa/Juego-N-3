using UnityEngine;

public class MobilePlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 1.0f;
    public bool ignoreReturnIfAcido = true; // Variable para ignorar el regreso si el objeto tiene el tag "Acido"

    private bool movingToEnd = true;
    private bool shouldMove = false; // Variable para determinar si la plataforma debe comenzar a moverse

    private bool jugadorTocado = false; // Variable para verificar si se tocó al jugador

    private bool esPlataforma = false; // Variable para verificar si la plataforma es del tipo "Plataforma"

    void Start()
    {
        esPlataforma = gameObject.CompareTag("Plataforma");
    }

    void FixedUpdate()
    {
        if (!shouldMove)
        {
            return; // Si no debe moverse, se sale del método y no realiza ningún movimiento
        }

        if (movingToEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed * Time.fixedDeltaTime);
            if (transform.position == endPoint.position)
            {
                movingToEnd = false;
                if (ignoreReturnIfAcido && transform.CompareTag("Acido"))
                {
                    enabled = false; // Desactiva este script si el objeto tiene el tag "Acido"
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, speed * Time.fixedDeltaTime);
            if (transform.position == startPoint.position)
                movingToEnd = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugadorTocado = true; // Marcar que se tocó al jugador
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
            shouldMove = true; // Activar la variable para que la plataforma comience a moverse
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            shouldMove = false; // Desactivar la variable para detener el movimiento de la plataforma
            jugadorTocado = false; // Reiniciar el estado del jugador
        }
    }

    private void Update()
    {
        // Verificar si solo se tocó al jugador y al objeto con el tag "Acido"
        if (jugadorTocado && transform.CompareTag("Acido"))
        {
            shouldMove = true; // Activar la variable para que la plataforma comience a moverse
        }

        // Verificar si la plataforma es del tipo "Plataforma" y moverla automáticamente
        if (esPlataforma)
        {
            shouldMove = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPoint.position, 0.5f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(startPoint.position, endPoint.position);
    }
}
