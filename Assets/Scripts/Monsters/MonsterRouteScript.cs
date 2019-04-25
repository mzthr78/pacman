using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterRouteScript : MonoBehaviour
{
    public Transform target;
    public GameObject master;

    LineRenderer line;
    NavMeshAgent agent;
    NavMeshPath path;

    // Update is called once per frame
    void Update()
    {
        agent = master.GetComponent<NavMeshAgent>();

        path = new NavMeshPath();
        agent.CalculatePath(target.position, path);

        line = GetComponent<LineRenderer>();

        if (path.corners.Length > 0)
        {
            line.startColor = Color.blue;
            line.endColor = Color.blue;

            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);

            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
        }
        else
        {
            // Debug.Log("path がないよ!");
        }
    }
}
