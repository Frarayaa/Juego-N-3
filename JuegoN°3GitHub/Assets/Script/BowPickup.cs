using UnityEngine;

public class BowPickup : MonoBehaviour
{
    public GameObject arrowPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Character character = collision.GetComponent<Character>();
            if (character != null)
            {
                character.GetBow(arrowPrefab);
                Destroy(gameObject);
            }
        }
    }
}
