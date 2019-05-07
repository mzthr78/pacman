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

    Transform targetObj;
    public Transform pacman;

    public GameObject leftright;
    public GameObject updown;

    float speed = 0.3f;
    float moveSpeed = 0.1f;

    bool freeze = true;

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

    public void Freeze()
    {
        this.freeze = true;
    }

    public void UnFreeze()
    {
        this.freeze = false;
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

        posQue = new Queue<Vector3>();
        checkPoint = transform.position;
    }

    private void Update()
    {
        float rx = Mathf.Floor(transform.position.x) + 0.5f;
        float rz = Mathf.Round(transform.position.z);

        Vector3 coord = controller.Coord2Xz(transform.position);
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

        Vector3 currentPos = new Vector3(transform.position.x, posY, transform.position.z);
        float distance = Vector3.Distance(currentPos, checkPoint);
        float step = Time.deltaTime * 7;

        if (distance > 0.15f)
        {
            Debug.Log("distance = " + distance + " transform.position = " + transform.position + " checkpoint = " + checkPoint);
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

        // 
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

        Debug.Log("moveDir = " + moveDir);
        Move(moveDir);
    }

    void tmpFunc ()
    {
        if (posQue.Count > 0)
        {


            Vector3 currentPos = new Vector3(transform.position.x, posY, transform.position.z);
            float distance = Vector3.Distance(currentPos, checkPoint);
            float step = Time.deltaTime * 7;

            if (distance > 0.15f)
            {
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
                }
            }
        }
        else
        {
            //Move(moveDir);
        }
        Move(moveDir);
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

    void SearchTarget(Transform target)
    {
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

        //state = GhostState.chase;
    }
}
