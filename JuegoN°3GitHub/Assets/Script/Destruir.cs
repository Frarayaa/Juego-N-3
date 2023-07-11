using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruirarbol : MonoBehaviour
{
    public Character charc;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (charc.hasHacha == true)
        {
            charc.isCutting = true;
            Destroy(this.gameObject);
            charc.cantidadMadera++;
            charc.isCutting = false;
        }
    }
}
