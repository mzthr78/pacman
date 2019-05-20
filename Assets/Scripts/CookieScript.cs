using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieScript : MonoBehaviour
{
    GameController controller;

    private void Awake()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("cookie trigger other = " + other);
        if (other.name == "Pacman")
        {
            controller.EatCookie();
            Destroy(gameObject);
        }
    }
}
