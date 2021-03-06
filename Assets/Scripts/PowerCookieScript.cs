﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCookieScript : MonoBehaviour
{
    GameController controller;
    bool freeze = true;

    private void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void Freeze()
    {
        this.freeze = true;
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
            //controller.PlaySE(SoundEffect.ijike);
            controller.Ghosts2Blue();
        }
    }

    IEnumerator Blink()
    {
        while (!this.freeze)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
