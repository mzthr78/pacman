using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum GhostState
{
    idle,
    waiting,
    search,
    chase,
}

public class GhostScript : MonoBehaviour
{
    public GameController controller;

    public Transform targetObj;
    public Transform pacman;

    public GameObject leftright;
    public GameObject updown;

    float speed = 0.1f;
    float moveSpeed = 0.1f;

    bool freeze = true;

    public GameObject pointPrefab;
    public GameObject point2Prefab;

    private Direction moveDir;

    GhostState state = GhostState.waiting;

    NavMeshAgent agent;
    NavMeshPath path;

    private Vector3[] checkPoints;
    private Vector3 checkPoint;

    float posY = 0.5f;

    Queue<Vector3> posQue;

    List<List<mapdata>> map;

    float moveX;
    float moveZ;

    int[] vx = { 0, 1, 0, -1 };
    int[] vz = { -1, 0, 1, 0 };

    bool next;
    int queSeq;

    public void Freeze()
    {
        this.freeze = true;
    }

    public void UnFreeze()
    {
        this.freeze = false;
    }

    public Queue<Vector3> GetPosQue()
    {
        return this.posQue;
    }

    private void Awake()
    {
        //leftright.SetActive(true);
        //updown.SetActive(false);
    }

    private void Start()
    {
        map = controller.GetMap();
        agent = GetComponent<NavMeshAgent>();

        Debug.Log("[" + name + "](Start()) agent = " + agent);

        posQue = new Queue<Vector3>();
        checkPoint = transform.position;
        next = true;
    }

    float adjustX = 0;
    float adjustUP = 0;
    float adjustDown = 0;

    private void Update()
    {
        //Debug.Log(tag + " freeze? " + this.freeze);

        if (this.freeze) return;

        float rx = Mathf.Floor(transform.position.x) + 0.5f;
        float rz = Mathf.Round(transform.position.z);

        Vector3 coord = controller.Coord2Xz(transform.position);
        //Vector3 coord = controller.Coord2Xz(new Vector3(rx, 0, rz));
        int ix = (int)(coord.x + 13.5f);
        int iz = Mathf.Abs((int)(coord.z - 15));

        char[] dirobj = new char[4];

        for (int i = 0; i < 4; i++)
        {
            // exclusion out of range
            if (iz + vz[i] >= 0 && iz + vz[i] <= 30 && ix + vx[i] >= 0 && ix + vx[i] <= 27)
            {
                dirobj[i] = map[iz + vz[i]][ix + vx[i]].objchar;
            }
            else
            {
                // add warp script around here 
                dirobj[i] = '*';
            }
        }

        //Debug.Log(moveDir + " pos=" + transform.position + " cp=" + checkPoint + " (rx, rz) = (" + rx + ", " + rz + ")");
        //Debug.Log("↑" + dirobj[(int)Direction.up] + "↓" + dirobj[(int)Direction.down] + "←" + dirobj[(int)Direction.left] + "→" + dirobj[(int)Direction.right]);

        // direction change judgement
        switch (moveDir)
        {
            case Direction.left:
                if (transform.position.x < checkPoint.x && Mathf.Abs(transform.position.x - rx) < 0.1f)
                {
                    //moveDir = Direction.none;
                    next = true;
                }
                break;
            case Direction.right:
                if (transform.position.x > checkPoint.x && Mathf.Abs(transform.position.x - rx) < 0.1f)
                {
                    //moveDir = Direction.none;
                    next = true;
                }
                break;
            case Direction.up:
                if (transform.position.z > checkPoint.z && Mathf.Abs(transform.position.z - rz) < 0.1f)
                {
                    //moveDir = Direction.none;
                    next = true;
                }
                break;
            case Direction.down:
                //Debug.Log("[" + name + "](judge) p=" + transform.position + " s=" + checkPoint + " r=" + rz);
                if (transform.position.z < checkPoint.z && Mathf.Abs(transform.position.z - rz) < 0.1f)
                {
                    //moveDir = Direction.none;
                    next = true;
                }
                break;
            default:
                if (posQue != null && posQue.Count > 0)
                {
                    next = true;
                }
                break;
        }

        if (next)
        {
            if (posQue != null && posQue.Count > 0)
            {
                queSeq++;

                //Debug.Log("[" + name + "] Que -> " + posQue.Peek() + " direction = " + moveDir);
                //Debug.Log("[" + name + "] position = " + transform.position);

                checkPoint = posQue.Dequeue();

                //Debug.Log("[" + name + "](" + queSeq + ") p=" + transform.position + " c=" + checkPoint);

                switch (moveDir)
                {
                    case Direction.left:
                    case Direction.right:
                        adjustX = 0.9f;
                        adjustUP = 0f;
                        adjustDown = 0;
                        break;
                    case Direction.up:
                    case Direction.down:
                        adjustX = -0.1f;
                        adjustUP = 0.9f;
                        adjustDown = 0;
                        break;
                }

                //Debug.Log("(rx, rz)=(" + rx + "," + rz + ")" + ", (ix, iz)=(" + ix + "," + iz + ") " + "↑" + dirobj[(int)Direction.up] + "↓" + dirobj[(int)Direction.down] + "←" + dirobj[(int)Direction.left] + "→" + dirobj[(int)Direction.right]);

                NextDir(dirobj);

                //Debug.Log("[" + name + "]" + "moveDir -> " + moveDir);
            }
            else
            {
                moveDir = Direction.none;
            }

            next = false;
        }

        // stop when 
        switch (moveDir)
        {
            case Direction.left:
            case Direction.right:
                if (Mathf.Abs(transform.position.x - rx) < 0.1f && dirobj[(int)moveDir] == '#')
                {
                    moveDir = Direction.none;
                }
                break;
            case Direction.up:
            case Direction.down:
                if (Mathf.Abs(transform.position.z - rz) < 0.1f && dirobj[(int)moveDir] == '#')
                {
                    moveDir = Direction.none;
                }
                break;
            default:
                break;
        }
        //Debug.Log("[" + name + "]" + " moveDir = " + moveDir);
        Move(moveDir);
    }

    void NextDir(char[] dirobj)
    {
        if (checkPoint.x > (transform.position.x + adjustX) && dirobj[(int)Direction.right] != '#')
        {
            moveDir = Direction.right;
        }
        else if (checkPoint.x < (transform.position.x - adjustX) && dirobj[(int)Direction.left] != '#')
        {
            moveDir = Direction.left;
        }
        else if (checkPoint.z > (transform.position.z + adjustUP) && dirobj[(int)Direction.up] != '#')
        {
            moveDir = Direction.up;
        }
        else if (checkPoint.z < (transform.position.z - adjustDown) && dirobj[(int)Direction.down] != '#')
        {
            moveDir = Direction.down;
        }
        else
        {
            moveDir = Direction.none;
        }
    }

    void tmpFunc() // reject
    {
        Vector3 currentPos = new Vector3(transform.position.x, 0, transform.position.z);
        float distance = Vector3.Distance(currentPos, checkPoint);
        float step = Time.deltaTime * 7;

        if (distance > 0.15f)
        {
            //Debug.Log("distance = " + distance + " transform.position = " + transform.position + " checkpoint = " + checkPoint);
            //transform.position = Vector3.MoveTowards(transform.position, checkPoint, step);
            if (transform.position.x > checkPoint.x)
            {
                moveDir = Direction.right;
            }
            else if (transform.position.x < checkPoint.x)
            {
                moveDir = Direction.left;
            }
            else if (transform.position.z > checkPoint.z)
            {
                moveDir = Direction.up;
            }
            else if (transform.position.z < checkPoint.z)
            {
                moveDir = Direction.down;
            }
            else
            {
                moveDir = Direction.none;
            }
        }
        else
        {
            if (posQue.Count > 0)
            {
                Vector3 prePoint = checkPoint;
                Vector3 nxtPoint = posQue.Peek();

                float diffX = nxtPoint.x - prePoint.x;
                float diffZ = nxtPoint.z - prePoint.z;

                if (Mathf.Abs(diffX) > 0 && Mathf.Abs(diffZ) > 0)
                {
                    switch (GetDirection())
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
                }
                else
                {
                    if (diffX > 0)
                    {
                        ChangeDirection(Direction.right);
                    }
                    else if (diffX < 0)
                    {
                        ChangeDirection(Direction.left);
                    }
                    else if (diffZ > 0)
                    {
                        ChangeDirection(Direction.up);
                    }
                    else if (diffZ < 0)
                    {
                        ChangeDirection(Direction.down);
                    }
                    else
                    {

                    }
                    checkPoint = posQue.Dequeue();
                    //Debug.Log(checkPoint);
                }
            }
            else
            {
                // when posQue is empty, set next target.
                Debug.Log("next!");


            }
        }

    }

    // 動作確認用
    void Updatexxx()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ChangeDirection(Direction.up);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            ChangeDirection(Direction.down);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            ChangeDirection(Direction.left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            ChangeDirection(Direction.right);
        }
        else
        {

        }
        transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0, Input.GetAxisRaw("Vertical") * speed));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ghost trigger enter");
    }

    public void SetDirection(Direction dir)
    {
        this.moveDir = dir;
    }

    public Direction GetDirection()
    {
        return this.moveDir;
    }

    public void ChangeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.left:
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(270, -90, -90);
                break;
            case Direction.right:
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Direction.up:
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(270, -90, -90);
                break;
            case Direction.down:
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            default:
                break;
        }
        this.moveDir = dir;
    }

    public void Move(Direction dir)
    {
        ChangeDirection(dir);

        if (freeze) return;

        switch (dir)
        {
            case Direction.left:
                moveX = -1;
                moveZ = 0;
                break;
            case Direction.up:
                moveX = 0;
                moveZ = 1;
                break;
            case Direction.right:
                moveX = 1;
                moveZ = 0;
                break;
            case Direction.down:
                moveX = 0;
                moveZ = -1;
                break;
            default:
                moveX = 0;
                moveZ = 0;
                break;
        }
        transform.Translate(moveX * moveSpeed, 0, moveZ * moveSpeed);
    }

    public void ChasePacman()
    {
        SearchTarget(pacman);
    }

    public void ChaseTarget()
    {
        SearchTarget(targetObj);
    }

    public void SearchTarget(Transform target)
    {
        queSeq = 0;
        if (posQue != null && posQue.Count > 0)
        {
            posQue.Clear();
        }

        //Debug.Log("[" + name + "](SearchTarget)target.name = " + target.name);

        path = new NavMeshPath();

        agent = GetComponent<NavMeshAgent>();

        agent.CalculatePath(target.position, path);

        int range = path.corners.Length;
        for (int i = 1; i < range; i++)
        {
            posQue.Enqueue(path.corners[i]);
        }

        //posQue.Enqueue(path.corners[0]);
        //posQue.Enqueue(path.corners[1]);
        //posQue.Enqueue(path.corners[2]);

        checkPoints = path.corners;

        GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
        foreach (GameObject point in points)
        {
            Destroy(point);
        }

        // line render
        for (int i = 1; i < path.corners.Length; i++)
        {
            GameObject point = Instantiate(pointPrefab);
            point.transform.position = path.corners[i];
        }

        //state = GhostState.chase;
    }

    public Vector3[] GetCorners()
    {
        return this.checkPoints;
    }

}
