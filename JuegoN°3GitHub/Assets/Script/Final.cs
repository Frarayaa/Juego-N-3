using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Final : MonoBehaviour
{
    public bool destruir = false;
    
    public void Update()
    {
        if(destruir == true)
        {
            Destroy(gameObject);
        }
    }
}
