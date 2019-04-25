using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRouteScript : MonoBehaviour
{
    LineRenderer line;
    List<Vector3> corners;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        corners = new List<Vector3>();

        corners.Add(new Vector3(0, 0, 4f));
        corners.Add(new Vector3(4.5f, 0, 4f));

        line.positionCount = corners.Count;

        for (int i = 0; i < corners.Count; i++)
        {
            line.SetPosition(i, corners[i]);
            Debug.Log("xxx");
        }

        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
