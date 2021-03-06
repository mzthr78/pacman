﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SightScript : MonoBehaviour
{
    public GameObject ghost;
    public GameObject pacman;
    GhostScript ghostScript;

    LineRenderer line;
    float distance = 30.0f;

    bool find = false;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        ghostScript = ghost.GetComponent<GhostScript>();
        GetComponent<LineRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 from = transform.position;
        Vector3 to = new Vector3(0, 0, 0);

        float sightLen = 27.0f;

        switch (ghostScript.GetDirection())
        {
            case Direction.left:
                to = from - transform.right * sightLen;
                break;
            case Direction.right:
                to = from + transform.right * sightLen;
                break;
            case Direction.up:
                to = from + transform.forward * sightLen;
                break;
            case Direction.down:
                to = from - transform.forward * sightLen;
                break;
        }

        Vector3[] positions = new Vector3[2] { from, to };

        RaycastHit hit;
        if (Physics.Linecast(from, to, out hit))
        {
            if (hit.transform.tag == "Player")
            {
                find = true;
            }
            else
            {
                if (find)
                {
                    find = false;
                }
            }
        }

        if (GetComponent<LineRenderer>().enabled)
        {
            line.startWidth = 0.2f;
            line.endWidth = 0.2f;

            line.positionCount = positions.Length;
            line.SetPositions(positions);

            line.material.color = Color.red;
        }
    }
}
