using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CanvasGroup pauseMenuCanvasGroup;
    public bool isPaused = false;
    private string previousSceneName;

    private void Start()
    {
        pauseMenuCanvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void ShowDeathScreen()
    {
        // Almacenar el nombre de la escena anterior
        SceneManagerHelper.previousSceneName = SceneManager.GetActiveScene().name;

        // Cargar la escena de la pantalla de muerte
        SceneManager.LoadScene("DeathScreen");
    }

    public void Retry()
    {
        // Cargar la escena anterior por su nombre almacenado
        SceneManager.LoadScene(SceneManagerHelper.previousSceneName);
    }

    public void BackToMenu()
    {
        // Cargar la escena del menú principal por su nombre
        SceneManager.LoadScene("Menú");
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void TogglePauseMenu()
    {
        if (isPaused)
        {
            isPaused = false;
            pauseMenuCanvasGroup.alpha = 0;
            pauseMenuCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            isPaused = true;
            pauseMenuCanvasGroup.alpha = 1;
            pauseMenuCanvasGroup.blocksRaycasts = true;
        }
    }
}
