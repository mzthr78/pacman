using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCookieScript : MonoBehaviour
{
    GameObject controller;

    private void Start()
    {
        controller = GameObject.Find("GameController");
        StartCoroutine(Blink());
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("cookie trigger other = " + other);
        if (other.name == "Pacman")
        {
            controller.GetComponent<GameController>().EatPowerCookie();
        }
    }

    IEnumerator Blink()
    {
        while (true)
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
