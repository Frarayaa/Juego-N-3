using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruirfantasma : MonoBehaviour
{
    public Character charc;
    
    private void Update()
    {
        if (charc.hasSword == true)
        {
            Destroy(gameObject);
        }
    }
}
