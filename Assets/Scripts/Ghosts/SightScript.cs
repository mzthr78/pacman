using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightScript : MonoBehaviour
{
    public GameObject ghost;
    GhostScript ghostScript;

    LineRenderer line;
    float distance = 30.0f;

    bool findPacman = false;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        ghostScript = ghost.GetComponent<GhostScript>();
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
            if (hit.transform.name == "Pacman")
            {
                findPacman = true;
                ghostScript.UnFreeze();
            }
            else
            {
                if (findPacman)
                {
                    Debug.Log("Lost sight!");

                    ghostScript.ChasePacman();

                    findPacman = false;
                }
            }
        }

        line.startWidth = 0.2f;
        line.endWidth = 0.2f;

        line.positionCount = positions.Length;
        line.SetPositions(positions);

        line.material.color = Color.red;
    }
}
