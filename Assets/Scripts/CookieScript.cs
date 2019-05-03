using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("cookie trigger other = " + other);
        if (other.name == "Pacman")
        {
            Destroy(gameObject);
        }
    }
}
