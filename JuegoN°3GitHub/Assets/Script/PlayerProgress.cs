using UnityEngine;

[System.Serializable]
public class PlayerProgress
{
    public int health;
    public int cantidadFlechas;
    public int cantidadMadera;
    public int cantidadPiedra;
    public bool hasPicota;
    public bool hasHacha;
    public bool hasSword;
    public bool hasBow;
    public string previousSceneName;
    public Character character;

    public PlayerProgress(int health, int cantidadFlechas, int cantidadMadera, int cantidadPiedra, bool hasPicota, bool hasHacha, bool hasSword, bool hasBow)
    {
        this.health = health;
        this.cantidadFlechas = cantidadFlechas;
        this.cantidadMadera = cantidadMadera;
        this.cantidadPiedra = cantidadPiedra;
        this.hasPicota = false;
        this.hasHacha = false;
        this.hasSword = false;
        this.hasBow = false;
    }

    public void SaveProgress()
    {
        // Guardar los datos del progreso del jugador en PlayerPrefs u otro método de almacenamiento
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetInt("Flechas", cantidadFlechas);
        PlayerPrefs.SetInt("Madera", cantidadMadera);
        PlayerPrefs.SetInt("Piedra", cantidadPiedra);
        PlayerPrefs.SetInt("HasPicota", hasPicota ? 1 : 0);
        PlayerPrefs.SetInt("HasHacha", hasHacha ? 1 : 0);
        PlayerPrefs.SetInt("HasSword", hasSword ? 1 : 0);
        PlayerPrefs.SetInt("HasBow", hasBow ? 1 : 0);
        // Guardar otros datos relevantes
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        // Cargar los datos del progreso del jugador desde PlayerPrefs u otro método de almacenamiento
        if (PlayerPrefs.HasKey("Health"))
        {
            health = PlayerPrefs.GetInt("Health");
        }
        if (PlayerPrefs.HasKey("Flechas"))
        {
            cantidadFlechas = PlayerPrefs.GetInt("Flechas");
        }
        if (PlayerPrefs.HasKey("Madera"))
        {
            cantidadMadera = PlayerPrefs.GetInt("Madera");
        }
        if (PlayerPrefs.HasKey("Piedra"))
        {
            cantidadPiedra = PlayerPrefs.GetInt("Piedra");
        }
        if (PlayerPrefs.HasKey("HasPicota"))
        {
            hasPicota = PlayerPrefs.GetInt("HasPicota") == 1;
        }
        if (PlayerPrefs.HasKey("HasHacha"))
        {
            hasHacha = PlayerPrefs.GetInt("HasHacha") == 1;
        }
        if (PlayerPrefs.HasKey("HasSword"))
        {
            hasSword = PlayerPrefs.GetInt("HasSword") == 1;
        }
        if (PlayerPrefs.HasKey("HasBow"))
        {
            hasBow = PlayerPrefs.GetInt("HasBow") == 1;
        }
        // Cargar otros datos relevantes
    }

    public void ResetProgress()
    {
        // Reiniciar los datos del progreso del jugador a los valores predeterminados
        health = 10;
        cantidadFlechas = 0;
        cantidadMadera = 0;
        cantidadPiedra = 0;
        hasPicota = false;
        hasHacha = false;
        hasSword = false;
        hasBow = false;
        // Reiniciar otros datos relevantes
        SaveProgress();
    }

    public void UpdateFromCharacter(Character character)
    {
        health = character.health;
        cantidadFlechas = character.cantidadFlechas;
        cantidadMadera = character.cantidadMadera;
        cantidadPiedra = character.cantidadPiedra;
        hasPicota = character.hasPicota;
        hasHacha = character.hasHacha;
        hasSword = character.hasSword;
        hasBow = character.hasBow;
        SaveProgress();
    }
}
