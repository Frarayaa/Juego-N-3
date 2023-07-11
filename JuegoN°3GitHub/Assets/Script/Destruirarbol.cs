using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruir : MonoBehaviour
{
    public Character charc;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (charc.hasHacha == true)
        {
            Destroy(this.gameObject);
            charc.cantidadMadera++;
        }

        if (charc.hasPicota == true)
        {
            Destroy(this.gameObject);
            charc.cantidadPiedra++;
        }
    }
}
