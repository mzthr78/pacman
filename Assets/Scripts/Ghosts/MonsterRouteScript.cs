using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterRouteScript : MonoBehaviour
{
    public Transform target;
    public GameObject master;

    public Color color;

    LineRenderer line;
    NavMeshAgent agent;
    NavMeshPath path;

    bool called = false;

    private void Start()
    {
    }

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

            line.material.color = color;

            if (!called)
            {
                for (int i = 0; i < path.corners.Length; i++)
                {
                    //Debug.Log("[" + i + "] " + path.corners[i]);
                }
                called = true;
            }
        }
        else
        {
            // Debug.Log("path がないよ!");
        }
    }
}
