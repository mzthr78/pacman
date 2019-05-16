using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IjikeScript : MonoBehaviour
{
    public GameObject ghost;

    public GameController controller;
    public GameObject pacman;
    public GameObject ijike;
    public GameObject EatScore;
    public Transform targetObj;

    NavMeshAgent agent;

    bool freeze = false;

    bool reverse = false;

    private Direction moveDir;

    List<List<mapdata>> map;
    List<xz> passable;

    private Queue<Vector3> posQue;
    private Vector3 checkPoint;

    float moveX;
    float moveZ;

    private readonly int[] vx = { 0, 1, 0, -1 };
    private readonly int[] vz = { -1, 0, 1, 0 };

    private readonly float moveSpeed = 0.05f;

    private void Awake()
    {
        //agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
        pacman = GameObject.Find("Pacman");

        map = controller.GetMap();
        passable = controller.GetPassable();

        agent = GetComponent<NavMeshAgent>();

        posQue = new Queue<Vector3>();
        checkPoint = transform.position;
        next = true;

        StartCoroutine(Blink());
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.IsFreeze()) return;
        BlueMovement();
    }

    public void SetGhost(GameObject obj)
    {
        this.ghost = obj;
    }

    void Reverse()
    {
        reverse = !reverse;

        if (reverse)
        {
            ijike.transform.rotation = Quaternion.Euler(90, 90, 90);
        } else
        {
            ijike.transform.rotation = Quaternion.Euler(-90, 90, 90);
        }
    }

    IEnumerator Blink()
    {
        int count = 0;

        yield return new WaitForSeconds(4);

        count = 0;
        while (count < 3)
        {
            Reverse();
            yield return new WaitForSeconds(0.6f);
            count++;
        }

        count = 0;
        while (count < 3)
        {
            Reverse();
            yield return new WaitForSeconds(0.3f);
            count++;
        }

        count = 0;
        while (count < 2)
        {
            Reverse();
            yield return new WaitForSeconds(0.2f);
            count++;
        }

        count = 0;
        while (count < 8)
        {
            Reverse();
            yield return new WaitForSeconds(0.1f);
            count++;
        }

        ghost.SetActive(true);
        ghost.transform.position = gameObject.transform.position;

        controller.UnFreeze();

        Destroy(gameObject);
    }

    private bool next = false;

    private float adjustX = 0;
    private float adjustUP = 0;
    private float adjustDown = 0;

    void BlueMovement()
    {
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
                dirobj[i] = '*';
            }
        }

        // direction change judgement
        switch (moveDir)
        {
            case Direction.left:
                if (transform.position.x < checkPoint.x && Mathf.Abs(transform.position.x - rx) < 0.1f)
                {
                    next = true;
                }
                break;
            case Direction.right:
                if (transform.position.x > checkPoint.x && Mathf.Abs(transform.position.x - rx) < 0.1f)
                {
                    next = true;
                }
                break;
            case Direction.up:
                if (transform.position.z > checkPoint.z && Mathf.Abs(transform.position.z - rz) < 0.1f)
                {
                    next = true;
                }
                break;
            case Direction.down:
                if (transform.position.z < checkPoint.z && Mathf.Abs(transform.position.z - rz) < 0.1f)
                {
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
                checkPoint = posQue.Dequeue();

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
                NextDir(dirobj);
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
                NextTarget();
                break;
        }
        Move(moveDir);
    }

    void NextTarget()
    {
        xz xz = passable[Random.Range(0, passable.Count)];
        targetObj.transform.position = controller.Xz2Coord(xz.x, xz.z);
        ChaseTarget();
    }

    public void ChaseTarget()
    {
        SearchTarget(targetObj);
    }

    public void SearchTarget(Transform target)
    {
        NavMeshPath path;

        if (posQue != null && posQue.Count > 0)
        {
            posQue.Clear();
        }

        //Debug.Log("[" + name + "](SearchTarget)target.name = " + target.name);

        path = new NavMeshPath();

        agent = GetComponent<NavMeshAgent>();

        agent.enabled = true;

        agent.CalculatePath(target.position, path);

        int range = path.corners.Length;
        for (int i = 1; i < range; i++)
        {
            posQue.Enqueue(path.corners[i]);
        }

        agent.enabled = false;
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

    public void Move(Direction dir)
    {
        if (this.freeze) return;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(Eaten());
        }
    }

    IEnumerator Eaten()
    {
        Vector3 pos = gameObject.transform.position;

        ijike.SetActive(false);

        // freeze
        controller.Freeze();

        // play se
        controller.PlaySE(SoundEffect.eatghost);

        // score
        Debug.Log("eatscore = " + EatScore);

        EatScore.transform.position = pos;
        EatScore.GetComponent<TextMesh>().text = controller.GetEatGhostScore().ToString();
        EatScore.SetActive(true);

        yield return new WaitForSeconds(1);

        EatScore.SetActive(false);

        // eyes
        // (temprary)
        ghost.SetActive(true);

        // unfreeze
        controller.UnFreeze();

        Destroy(gameObject);
    }
}
