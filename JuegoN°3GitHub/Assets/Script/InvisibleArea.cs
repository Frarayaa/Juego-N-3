using UnityEngine;

public class InvisibleArea : MonoBehaviour
{
    public LayerMask invisibleLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character character = collision.GetComponent<Character>();
        if (character != null)
        {
            character.gameObject.layer = invisibleLayer;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Character character = collision.GetComponent<Character>();
        if (character != null)
        {
            character.gameObject.layer = LayerMask.NameToLayer("Player"); // Cambia "Default" al nombre de la capa que desees asignarle al personaje cuando salga del área invisible.
        }
    }
}
