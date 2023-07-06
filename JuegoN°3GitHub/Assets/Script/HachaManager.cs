using UnityEngine;

public class HachaManager : MonoBehaviour
{
    public GameObject hachaPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null)
            {
                character.GetHacha(hachaPrefab);
                Destroy(gameObject); // Destruye el objeto que otorga el hacha
            }
        }
    }
}
