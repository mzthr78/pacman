using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCookieScript : MonoBehaviour
{
    GameObject controller;
    bool freeze = true;

    private void Start()
    {
        controller = GameObject.Find("GameController");
    }

    public void Freeze()
    {
        this.freeze = true;
        StopCoroutine(Blink());
    }

    public void UnFreeze()
    {
        this.freeze = false;
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
            Renderer rend = GetComponent<Renderer>();
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
