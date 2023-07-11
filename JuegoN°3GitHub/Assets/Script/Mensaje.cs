using UnityEngine;
using UnityEngine.UI;

public class Mensaje : MonoBehaviour
{
    public Text texto;
    public Image imagen;
    public string mensaje;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que ha entrado en el trigger es el jugador
        if (collision.CompareTag("Player"))
        {
            // El jugador ha entrado en el trigger, muestra el texto y la imagen
            texto.text = mensaje;
            texto.gameObject.SetActive(true);
            imagen.gameObject.SetActive(true);
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verifica si el objeto que ha salido del trigger es el jugador
        if (collision.CompareTag("Player"))
        {
            // El jugador ha salido del trigger, oculta el texto y la imagen
            texto.gameObject.SetActive(false);
            imagen.gameObject.SetActive(false);
           
        }
    }
}
