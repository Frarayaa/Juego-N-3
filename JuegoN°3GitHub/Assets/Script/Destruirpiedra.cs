using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruirpiedra : MonoBehaviour
{
    public Character charc;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (charc.hasPicota == true)
        {
            charc.isMining = true;
            Destroy(this.gameObject);
            charc.cantidadPiedra++;
            charc.isMining = false;
        }
    }
}
