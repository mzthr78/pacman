using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("cookie trigger other = " + other);
        if (other.name == "Pacman")
        {
            Destroy(gameObject);
        }
    }
}
