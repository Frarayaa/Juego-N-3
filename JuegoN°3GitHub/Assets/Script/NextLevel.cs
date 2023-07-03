using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextLevel : MonoBehaviour
{
    public GameManager gm;
    public string nextLevelName; // Nombre de la escena del próximo nivel
    public Animator transitionAnimator; // Referencia al componente Animator para reproducir la animación de transición

    private bool levelCompleted = false; // Variable para asegurar que la transición se reproduzca solo una vez

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true;
            StartCoroutine(LoadNextLevel());
        }
    }

    private IEnumerator LoadNextLevel()
    {
        // Reproducir la animación de transición
        if (transitionAnimator != null)
        {
            Debug.Log("AAA");
            transitionAnimator.SetTrigger("Start");
        }

        // Esperar un tiempo suficiente para que la animación se reproduzca
        yield return new WaitForSeconds(0.7f); // Ajusta el tiempo de espera según la duración de tu animación
        Debug.Log("Wait end");
        gm?.SavePlayerProgress();

 
        // Cargar la siguiente escena
        SceneManager.LoadScene(nextLevelName);
    }
}