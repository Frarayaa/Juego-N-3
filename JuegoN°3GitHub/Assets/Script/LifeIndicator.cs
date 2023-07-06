using UnityEngine;
using UnityEngine.UI;

public class LifeIndicator : MonoBehaviour
{
    public Sprite[] lifeSprites; // Array de sprites de vida. Cada sprite representa un nivel de vida diferente.
    public Image lifeImage; // Componente Image del objeto "Life Indicator".
    public int maxLife = 3; // Cantidad máxima de vida del personaje.
    public Character charc;
    private int currentLife; // Cantidad actual de vida del personaje.

    private void Start()
    {
        currentLife = charc.health; // Establece la cantidad inicial de vida del personaje.
        UpdateLifeIndicator(); // Actualiza el sprite del indicador de vida.
    }

    public void Life()
    {
        currentLife = charc.health;
        UpdateLifeIndicator(); // Actualiza el sprite del indicador de vida.
    }

    private void UpdateLifeIndicator()
    {
        // Asigna el sprite de vida correspondiente al componente Image del objeto "Life Indicator".
        int lifeIndex = Mathf.Clamp(currentLife - 1, 0, lifeSprites.Length - 1);
        lifeImage.sprite = lifeSprites[lifeIndex];
    }
}
