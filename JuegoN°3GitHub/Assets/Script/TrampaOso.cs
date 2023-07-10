using System.Collections;
using UnityEngine;

public class TrampaOso : MonoBehaviour
{
    public float tiempoParaEscapar = 3f; // Tiempo que tiene el jugador para escapar
    public int cantidadDePresionesNecesarias = 3; // Cantidad de veces que debe presionar la tecla para escapar
    public KeyCode teclaEscape = KeyCode.Space; // Tecla necesaria para escapar

    public int damageAmount = 1; // Cantidad de daño que recibe el jugador al ser atrapado
    public int maxHealth = 5;

    private int presionesRealizadas = 0; // Contador de presiones realizadas
    private bool isAttacking = false; // Indicador de si el jugador está atrapado
    private float tiempoTranscurrido = 0f; // Tiempo transcurrido desde que el jugador fue atrapado
    private Character jugadorController; // Referencia al controlador del jugador
    private Vector3 posicionTrampa; // Posición de la trampa
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        // Obtener la referencia al componente PlayerController del jugador
        jugadorController = FindObjectOfType<Character>();
        posicionTrampa = transform.position;
    }

    void Update()
    {
        if (isAttacking)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Comprueba si se ha excedido el tiempo de escape
            if (tiempoTranscurrido >= tiempoParaEscapar)
            {
                // Hacer que el jugador sufra daño
                if (jugadorController != null)
                {
                    jugadorController.TakeDamage(damageAmount);
                }

                // Reiniciar la trampa
                ReiniciarTrampa();
            }
            else
            {
                // Comprueba si se presiona la tecla de escape
                if (Input.GetKeyDown(teclaEscape))
                {
                    presionesRealizadas++;

                    // Comprueba si se ha alcanzado la cantidad de presiones necesarias para escapar
                    if (presionesRealizadas >= cantidadDePresionesNecesarias)
                    {
                        // Liberar al jugador
                        ReiniciarTrampa();
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isAttacking)
        {
            isAttacking = true;
            jugadorController.enabled = false; // Desactivar el control del jugador
            jugadorController.transform.position = posicionTrampa; // Colocar al jugador en la posición de la trampa
        }
    }

    void ReiniciarTrampa()
    {
        isAttacking = false;
        tiempoTranscurrido = 0f;
        presionesRealizadas = 0;
        jugadorController.enabled = true; // Reactivar el control del jugador
    }
    
    public void TakeDamage(int damage)
    {
        maxHealth -= damage;

        if (maxHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void LateUpdate()
    {
        animator.SetBool("IsAttacking", isAttacking);
    }
}
