using UnityEngine;

public class PicotaManager : MonoBehaviour
{
    public GameObject picotaPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null)
            {
                character.GetPicota(picotaPrefab);
                Destroy(gameObject); // Destruye el objeto que otorga la picota
            }
        }
    }
}
