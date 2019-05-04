using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

enum MonsterStatus
{
    Idle,
    Search,
    Chase

}

public class BlinkyScript : MonoBehaviour
{
    public Transform target;

    Queue<Vector3>  posQue;

    public GameObject myRoute;

    public GameObject LeftRight;
    public GameObject TopDown;

    NavMeshAgent agent;
    NavMeshPath path;

    float speed = 5.0f;
    private float startTime;
    private float journeyLength;

    float posY = 0.5f;

    int[] vx = {  1,  0,  -1, 0 };
    int[] vy = {  0,  -1, 0,  1 };

    MonsterStatus status;
    private Vector3[] checkPoints;
    private Vector3 checkPoint;

    private bool freeze = true;

    GhostScript ghost;

    private void Awake()
    {
        ghost = GetComponent<GhostScript>();
        ghost.SetDirection(Direction.left);
    }

    void Start()
    {
        posQue = new Queue<Vector3>();

        LeftRight.SetActive(true);

        GameObject controller = GameObject.Find("GameController");
        List<List<mapdata>> map = controller.GetComponent<GameController>().GetMap();

        for (int i = 0; i < map.Count; i++)
        {
            for (int  j = 0; j < map[i].Count; j++)
            {
                // Debug.Log("[" + i + "][" + j + "]" + map[i][j].objtype + "(" + map[i][j].coordinate + ")");
            }
        }
        status = MonsterStatus.Idle;
        checkPoint = transform.position;
    }

    private void Update()
    {

        if (freeze && Input.GetMouseButtonDown(0))
        {
            SearchTarget();
            freeze = false;
        }

        if (!freeze)
        {
            Vector3 currentPos = new Vector3(transform.position.x, posY, transform.position.z);

            float distance = Vector3.Distance(currentPos, checkPoint);
            float step = Time.deltaTime * 7;

            if (distance > 0.15f)
            {
                transform.position = Vector3.MoveTowards(transform.position, checkPoint, step);
            } else
            {
                if (posQue.Count > 0)
                {
                    Vector3 prePoint = checkPoint;
                    Vector3 nxtPoint = posQue.Peek();

                    float diffX = nxtPoint.x - prePoint.x;
                    float diffZ = nxtPoint.z - prePoint.z;

                    if (Mathf.Abs(diffX) > 0 && Mathf.Abs(diffZ) > 0)
                    {
                        switch (ghost.GetDirection())
                        {
                            case Direction.left:
                            case Direction.right:
                                checkPoint = new Vector3(nxtPoint.x, posY, prePoint.z);
                                break;
                            case Direction.up:
                            case Direction.down:
                                checkPoint = new Vector3(prePoint.x, posY, nxtPoint.z);
                                break;
                        }
                        //Debug.Log(checkPoint);
                    } else
                    {
                        if (diffX > 0)
                        {
                            ghost.ChangeDirection(Direction.right);
                        } else if (diffX < 0)
                        {
                            ghost.ChangeDirection(Direction.left);
                        } else if (diffZ > 0)
                        {
                            ghost.ChangeDirection(Direction.up);
                        } else if (diffZ < 0)
                        {
                            ghost.ChangeDirection(Direction.down);
                        } else
                        {

                        }
                        checkPoint = posQue.Dequeue();
                        //Debug.Log(checkPoint);
                    }
                } else
                {
                    freeze = true;
                }
            }
        }
    }

    void SearchTarget()
    {
        agent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();

        //ここでtargetを変えるようにする
        agent.CalculatePath(target.position, path);

        checkPoints = path.corners;

        for (int i = 0; i < checkPoints.Length; i++)
        {
            float x = Mathf.Floor(checkPoints[i].x) + 0.5f;
            float y = posY;
            float z = Mathf.Round(checkPoints[i].z) - 0.5f;

            posQue.Enqueue(new Vector3(x, y, z));
        }

        status = MonsterStatus.Chase;
    }

}
