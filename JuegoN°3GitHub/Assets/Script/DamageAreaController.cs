using UnityEngine;

public class DamageAreaController : MonoBehaviour
{
    public Color damageAreaColor = Color.green;
    public float destroyDelay = 2f;
    public Vector3 damageAreaSize = new Vector3(1f, 1f, 0.2f);

    private void Start()
    {
        // Configurar el color del material del área de daño
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            if (material != null)
            {
                material.color = damageAreaColor;
            }
        }

        // Establecer el tamaño del área de daño
        transform.localScale = damageAreaSize;

        // Destruir el área de daño después de un tiempo
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Lógica para dañar al jugador cuando entre en contacto con el área de daño
            // Por ejemplo:
            Character player = collision.GetComponent<Character>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
