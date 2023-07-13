using UnityEngine;

public class HachaManager : MonoBehaviour
{
    public GameObject hachaPrefab;
    public Character charc;

    private void Update()
    {
        if (charc.hasHacha == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            charc = collision.gameObject.GetComponent<Character>();
            if (charc != null)
            {
                charc.GetHacha(hachaPrefab);
                Destroy(gameObject); // Destruye el objeto que otorga el hacha
            }
        }
    }
}
