using UnityEngine;

public class BowPickup : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Character charc;

    private void Update()
    {
        if (charc.hasSword == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            charc = collision.GetComponent<Character>();
            if (charc != null)
            {
                charc.GetBow(arrowPrefab);
                Destroy(gameObject);
            }
        }
    }
}
