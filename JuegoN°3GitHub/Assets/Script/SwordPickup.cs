using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    public GameObject swordPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null)
            {
                character.GetSword(swordPrefab);
                Destroy(gameObject);
            }
        }
    }
}
