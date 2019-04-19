using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum MonsterStatus
{
    Idle,
    Search,
    Chase

}

public class BlinkyScript : MonoBehaviour
{
    public Transform target;

    NavMeshAgent agent;
    NavMeshPath path;

    float speed = 5.0f;
    private float startTime;
    private float journeyLength;

    MonsterStatus status;
    private Vector3[] corners;
    private int seq = 0;

    private bool freeze = true;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Blinky Start");

        GameObject controller = GameObject.Find("GameController");
        List<List<char>> map = controller.GetComponent<GameController>().GetMap();

        for (int i = 0; i < map.Count; i++)
        {
            for (int  j = 0; j < map[i].Count; j++)
            {
                //Debug.Log("map[" + i + "][" + j + "]" + map[i][j]);
            }
        }
        status = MonsterStatus.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze && Input.GetMouseButtonDown(0))
        {
            SearchTarget();
            seq = 0;
            freeze = false;
        }

        if (!freeze)
        {
            Debug.Log("seq = " + seq);
            if (seq < corners.Length)
            {
                Vector3 sourcePosition = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 targetPosition = new Vector3(corners[seq].x, 0, corners[seq].z);
                float distance = Vector3.Distance(sourcePosition, targetPosition);
                //Debug.Log("transform.position = " + transform.position);
                //Debug.Log("corners[" + seq + "]" + corners[seq]);
                Debug.Log("distance = " + distance);
                if (distance > 0.1f)
                {
                    float step = Time.deltaTime * 10;
                    transform.position = Vector3.MoveTowards(transform.position, corners[seq], step);
                } else
                {
                    seq++;
                }
            }
            else
            {
                freeze = true;
            }
        }

        /*
        switch (status)
        {
            case MonsterStatus.Idle:
                if (Input.GetMouseButtonDown(0))
                {
                    status = MonsterStatus.Search;
                }
                break;
            case MonsterStatus.Search:
                SearchTarget();
                break;
            case MonsterStatus.Chase:
                float distance = Vector3.Distance(transform.position, corners[seq]);
                Debug.Log("distance = " + distance);
                if (distance > 10)
                {
                    Debug.Log("corners position" + "[" + seq + "]" + corners[seq]);
                    Debug.Log("transform.position(before) = " + transform.position);
                    float step = Time.deltaTime * 10;
                    transform.position = Vector3.MoveTowards(transform.position, corners[seq], step);
                    Debug.Log("transform.position(after) = " + transform.position);
                }
                else if (seq < corners.Length)
                {
                    Debug.Log("seq up");
                    seq++;
                } else
                {
                    status = MonsterStatus.Idle;
                }
                break;
            default:
                break;
        }
        */
    }

    void SearchTarget()
    {
        agent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();
        agent.CalculatePath(target.position, path);
        corners = path.corners;

        status = MonsterStatus.Chase;
    }

}
