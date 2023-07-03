using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character charc;
    public CanvasGroup pauseMenuCanvasGroup;
    public bool isPaused = false;
    private string previousSceneName;
    public PlayerProgress pp;
    public bool backToLand = false;

    private void Start()
    {
        if (pauseMenuCanvasGroup) {
            pauseMenuCanvasGroup.alpha = 0;
            pauseMenuCanvasGroup.blocksRaycasts = false;
            pauseMenuCanvasGroup.interactable = false;
        }

        // Cargar el progreso del jugador al iniciar el GameManager
        pp.LoadProgress();
        charc.Load(pp);
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
        previousSceneName = SceneManager.GetActiveScene().name;

        // Cargar la escena de la pantalla de muerte
        SceneManager.LoadScene("DeathScreen");
    }

    public void Retry()
    {
        // Cargar la escena anterior por su nombre almacenado
        pp.LoadProgress();
        SceneManager.LoadScene(previousSceneName);
    }

    public void BackToMenu()
    {
        // Guardar el progreso del jugador
        pp.SaveProgress();

        // Cargar la escena del menú principal por su nombre
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(string levelName)
    {
        pp.LoadProgress();
        SceneManager.LoadScene(levelName);
    }

    public void Reset()
    {
        pp.ResetProgress();
        isPaused = false;
        pauseMenuCanvasGroup.alpha = 0;
        pauseMenuCanvasGroup.blocksRaycasts = false;
        pauseMenuCanvasGroup.interactable = false;
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
            pauseMenuCanvasGroup.interactable = false;
        }
        else
        {
            isPaused = true;
            pauseMenuCanvasGroup.alpha = 1;
            pauseMenuCanvasGroup.blocksRaycasts = true;
            pauseMenuCanvasGroup.interactable = true;
        }
    }

    public void SavePlayerProgress()
    {
        pp.SaveProgress();
    }

    public void LoadPlayerPosition()
    {
        if (backToLand == false)
        {
            pp.PlayerPosition();
        }
    }
    public void LoadPlayerProgress()
    {
        pp.LoadProgress();
    }
}
