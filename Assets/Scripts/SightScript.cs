using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightScript : MonoBehaviour
{
    LineRenderer line;
    float distance = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 from = transform.position;
        Vector3 to = from + transform.forward * distance;
        Vector3[] positions = new Vector3[2] { from, to };

        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }
}
