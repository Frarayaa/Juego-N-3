using UnityEngine;

public class PicotaManager : MonoBehaviour
{
    public GameObject picotaPrefab;
    public Character charc;

    private void Update()
    {
        if (charc.hasPicota == true)
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
                charc.GetPicota(picotaPrefab);
                Destroy(gameObject); // Destruye el objeto que otorga la picota
            }
        }
    }
}
