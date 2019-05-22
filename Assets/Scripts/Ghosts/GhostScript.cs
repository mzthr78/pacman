using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum GhostState
{
    idle,
    waiting,
    ready,
    search,
    usual,
    find,
    lost,
    sensing,
    blue,
    eyes,
    gohome,
}

public class GhostScript : MonoBehaviour
{
    public GameController controller;

    public Transform targetObj;
    public Transform pacman;

    public GameObject defaultSkin;
    public GameObject defaultlr;
    public GameObject defaultud;

    public GameObject ijike;

    public GameObject eyesSkin;
    public GameObject eyesLr;
    public GameObject eyesUd;

    public GameObject point;

    private readonly float baseSpeed = 0.1f;
    private float moveSpeed = 0.1f;

    bool freeze = true;

    bool blue = false;

    private Direction moveDir;

    GhostState state = GhostState.waiting;

    NavMeshAgent agent;
    NavMeshPath path;

    private Vector3[] checkPoints;
    private Vector3 checkPoint;

    private Queue<Vector3> posQue;

    private List<List<mapdata>> map;
    private List<xz> passable;

    private float moveX;
    private float moveZ;

    private readonly int[] vx = { 0, 1, 0, -1 };
    private readonly int[] vz = { -1, 0, 1, 0 };

    private bool next;

    public void Freeze()
    {
        this.freeze = true;
    }

    public void UnFreeze()
    {
        this.freeze = false;
    }

    public bool IsFreeze()
    {
        return this.freeze;
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
        passable = controller.GetPassable();

        agent = GetComponent<NavMeshAgent>();

        // Debug.Log("[" + name + "](Start()) agent = " + agent);

        posQue = new Queue<Vector3>();
        checkPoint = transform.position;
        next = true;

        switch (tag)
        {
            case "Inky":
                StartCoroutine(ReadyGo(6));
                break;
            case "Pinky":
                StartCoroutine(ReadyGo(12));
                break;
            case "Clyde":
                StartCoroutine(ReadyGo(24));
                break;
        }

        point.SetActive(false);

        prePos = transform.position;
    }

    private float adjustX = 0;
    private float adjustUP = 0;
    private float adjustDown = 0;

    float delta = 0;

    Vector3 prePos;
    int stagnation = 0;

    void ChangeSkin()
    {
        if (blue)
        {
            ijike.SetActive(true);

            defaultSkin.SetActive(false);
            defaultlr.SetActive(false);
            defaultud.SetActive(false);

            eyesSkin.SetActive(false);
            eyesLr.SetActive(false);
            eyesUd.SetActive(false);
        }
        else
        {
            ijike.SetActive(false);
            switch (state)
            {
                case GhostState.eyes:
                    eyesSkin.SetActive(true);
                    defaultSkin.SetActive(false);
                    break;
                default:
                    defaultSkin.SetActive(true);
                    eyesSkin.SetActive(false);
                    break;
            }
        }
    }

    bool find = false;

    private void Update()
    {
        //Debug.Log(tag + " freeze? " + this.freeze);
        if (this.freeze) return;

        if (transform.position == prePos)
        {
            stagnation++;
            //Debug.Log("[" + name + "] stagnation=" + stagnation);
        } else
        {
            stagnation = 0;
        }
        prePos = transform.position;

        if (blue)
        {
            delta += Time.deltaTime;

            if (delta > controller.GetBlueSpan() * 0.4f)
            {
                blinkSpan = 1.2f;
            }

            if (delta > controller.GetBlueSpan() * 0.5f)
            {
                blinkSpan = 0.5f;
            }

            if (delta > controller.GetBlueSpan() * 0.6f)
            {
                blinkSpan = 0.3f;
            }

            if (delta > controller.GetBlueSpan() * 0.8f)
            {
                blinkSpan = 0.125f;
            }
        } else
        {
            delta = 0;
            blinkSpan = 0;
        }

        switch (this.state)
        {
            case GhostState.waiting:
                WaitingMovement();
                break;
            case GhostState.ready:
                ReadyMovement();
                break;
            case GhostState.gohome:
                GoHomeMovement();
                break;
            default: //chase
                if (!blue)
                {
                    if (IsHitPacman())
                    {
                        Move(moveDir);
                        find = true;
                    } else
                    {
                        if (find)
                        {
                            targetObj.position = pacman.transform.position + AdjustPos(pacman.GetComponent<PlayerScript>().GetDirection());
                            ChaseTarget();
                            find = false;
                        }
                        DefaultMovement();
                    }
                }
                else
                {
                    DefaultMovement();
                }
                break;
        }

        if (state == GhostState.eyes)
        {
            if (Vector3.Distance(transform.position, targetObj.transform.position) < 2)
            {
                state = GhostState.gohome;
            }
        }

        if (Mathf.Abs(transform.position.z) > 15 || Mathf.Abs(transform.position.x) > 13.5f || stagnation > 50)
        {
            switch (state)
            {
                case GhostState.eyes:
                case GhostState.gohome:
                    controller.DecreaseGoHome();
                    break;
            }
            Reset();
        }
    }

    Vector3 AdjustPos(Direction pacmanDir)
    {
        Vector3 adjust = new Vector3(0, 0, 0);

        switch (pacmanDir)
        {
            case Direction.left:
                adjust = new Vector3(-1, 0, 0);
                break;
            case Direction.right:
                adjust = new Vector3(1, 0, 0);
                break;
            case Direction.up:
                adjust = new Vector3(0, 0, 1);
                break;
            case Direction.down:
                adjust = new Vector3(0, 0, -1);
                break;
        }

        return adjust;
    }

    bool IsHitPacman()
    {
        return IsHit("Player");
    }

    bool IsHit(string tagName)
    {
        Vector3 from = transform.position;
        Vector3 to = new Vector3(0, 0, 0);

        float sightLen = 27.0f;

        switch (moveDir)
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

        Vector3[] positions = { from, to };

        RaycastHit hit;

        if (Physics.Linecast(from, to, out hit))
        {
            if (hit.transform.tag == tagName)
            {
                return true;
            }
        }
        return false;
    }

    public void Reset()
    {
        state = GhostState.ready;

        switch (tag)
        {
            case "Blinky":
                transform.position = new Vector3(0, 0, 4);
                break;
            case "Inky":
                transform.position = new Vector3(-1.5f, 0, 1);
                break;
            case "Pinky":
                transform.position = new Vector3(0, 0, 1);
                break;
            case "Clyde":
                transform.position = new Vector3(1.5f, 0, 1);
                break;
        }
    }

    float GetSpeed()
    {
        if (blue) return 0.03f;
        if (state == GhostState.eyes) return 0.15f;

        float speed = baseSpeed;
        switch (tag)
        {
            case "Blinky":
                speed *= 1.02f;
                break;
            case "Inky":
                speed *= 1.2f;
                break;
            case "Pinky":
                speed *= 1f;
                break;
            case "Clyde":
                speed *= 0.5f;
                break;
            default:
                speed *= 1f;
                break;
        }
        return speed;
    }

    public void BeBlue(bool blue = true)
    {
        this.blue = blue;

        if (blue)
        {
            moveSpeed = 0.03f;
            blinkSpan = controller.GetBlueSpan() * 0.4f;

            this.reverse = false;
            ReverseBlue(false);

            StartCoroutine(BlinkBlue());

            IjikeMovement();
        }
        else
        {
            moveSpeed = GetSpeed();
        }
    }

    float blinkSpan = 0;
    bool reverse = false;

    void ReverseBlue(bool rev = true)
    {
        if (rev)
        {
            ijike.transform.rotation = Quaternion.Euler(90, 90, 90);
        }
        else
        {
            ijike.transform.rotation = Quaternion.Euler(-90, 90, 90);
        }
    }

    IEnumerator BlinkBlue()
    {
        while (blue)
        {
            yield return new WaitForSeconds(blinkSpan);

            if (!freeze)
            {
                this.reverse = !this.reverse;
                ReverseBlue(this.reverse);
            }
        }
    }

    IEnumerator ReadyGo(float wait)
    {
        yield return new WaitForSeconds(wait);
        SetState(GhostState.ready);
    }

    public GhostState GetState()
    {
        return this.state;
    }

    public void SetState(GhostState gs)
    {
        this.state = gs;
    }

    public void WaitingMovement()
    {
        if (moveDir == Direction.up)
        {
            if (transform.position.z > 2.0f)
            {
                ChangeDirection(Direction.down);
            }
        } else
        {
            if (transform.position.z < 0)
            {
                ChangeDirection(Direction.up);
            }
        }
        Move(moveDir);
    }

    int readyState = 0;

    public void ReadyMovement()
    {
        switch (readyState)
        {
            case 0:
                if (Mathf.Abs(transform.position.z) > 0.1f)
                {
                    if (transform.position.z > 0)
                    {
                        ChangeDirection(Direction.down);
                    }
                    else
                    {
                        ChangeDirection(Direction.up);
                    }
                }
                else
                {
                    readyState = 1;
                }
                break;
            case 1:
                if (Mathf.Abs(transform.position.x) > 0.1f)
                {
                    if (transform.position.x > 0 )
                    {
                        ChangeDirection(Direction.left);
                    }
                    else
                    {
                        ChangeDirection(Direction.right);
                    }
                }
                else
                {
                    readyState = 2;
                }
                break;
            case 2:
                ChangeDirection(Direction.up);
                if (transform.position.z >= 4)
                {
                    SetState(GhostState.usual);
                }
                break;
        }
        Move(moveDir);
    }

    int goHomeState = 0;

    void GoHomeMovement()
    {
        //Debug.Log("[" + name + "]" + "(gohome movement) state=" + goHomeState);

        switch (goHomeState)
        {
            case 0:
                if (Mathf.Abs(transform.position.z) > 5f)
                {
                    if (transform.position.z > 0)
                    {
                        ChangeDirection(Direction.down);
                    }
                    else
                    {
                        ChangeDirection(Direction.up);
                    }
                }
                else
                {
                    goHomeState++;
                }
                break;
            case 1:
                if (Mathf.Abs(transform.position.x) > 0.1f)
                {
                    if (transform.position.x > 0)
                    {
                        ChangeDirection(Direction.left);
                    }
                    else
                    {
                        ChangeDirection(Direction.right);
                    }
                }
                else
                {
                    goHomeState++;
                }
                break;
            case 2:
                if (Mathf.Abs(transform.position.z) > 1.1f)
                {
                    ChangeDirection(Direction.down);
                }
                else
                {
                    goHomeState++;
                }
                break;
            case 3:
                eyesSkin.SetActive(false);
                eyesLr.SetActive(false);
                eyesUd.SetActive(false);
                state = GhostState.ready;
                next = true;
                goHomeState = 0;
                controller.DecreaseGoHome();
                controller.StopSE();
                break;
        }
        Move(moveDir);
    }

    void IjikeMovement()
    {
        moveDir = reverseDir(moveDir);

        float rx = Mathf.Floor(transform.position.x) + 0.5f;
        float rz = Mathf.Round(transform.position.z);

        Vector3 coord = controller.Coord2Xz(transform.position);
        //Vector3 coord = controller.Coord2Xz(new Vector3(rx, 0, rz));
        int ix = (int)(coord.x + 13.5f);
        int iz = Mathf.Abs((int)(coord.z - 15));

        char objChar = map[iz][ix].objchar;
        Vector3 pos = transform.position;

        while (objChar == '.' || objChar == ' ')
        {
            pos = map[iz][ix].coordinate;

            switch (moveDir)
            {
                case Direction.left:
                    objChar = map[iz][ix - 1].objchar;
                    ix--;
                    break;
                case Direction.right:
                    objChar = map[iz][ix + 1].objchar;
                    ix++;
                    break;
                case Direction.up:
                    objChar = map[iz + 1][ix].objchar;
                    iz++;
                    break;
                case Direction.down:
                    objChar = map[iz - 1][ix].objchar;
                    iz--;
                    break;
            }
        }

        targetObj.position = pos;
        ChaseTarget();
    }

    Direction reverseDir(Direction srcDir)
    {
        Direction dstDir = Direction.none;

        switch (srcDir)
        {
            case Direction.left:
                dstDir = Direction.right;
                break;
            case Direction.right:
                dstDir = Direction.left;
                break;
            case Direction.up:
                dstDir = Direction.down;
                break;
            case Direction.down:
                dstDir = Direction.up;
                break;
        }

        return dstDir;
    }

    void DefaultMovement()
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

    int trials = 0;

    void NextTarget()
    {
        if (state != GhostState.eyes)
        {
            xz xz = passable[Random.Range(0, passable.Count)];
            targetObj.transform.position = controller.Xz2Coord(xz.x, xz.z);
        } else
        {
            if (transform.position.x > 0)
            {
                targetObj.transform.position = new Vector3(0.5f, 0, 4);
            }
            else
            {
                targetObj.transform.position = new Vector3(-0.5f, 0, 4);
            }

            trials = 0;

            if (Vector3.Distance(transform.position, targetObj.transform.position) < 2)
            {
                state = GhostState.gohome;
            } 
            else
            {
                switch (trials)
                {
                    case 0:
                        targetObj.transform.position = new Vector3(1, 0, 4);
                        break;
                    default:
                        targetObj.transform.position = new Vector3(0, 0, 4);
                        break;
                }
                trials++;
            }
        }
        ChaseTarget();
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
        ChangeSkin();

        if (!blue)
        {
            switch (dir)
            {
                case Direction.left:
                    eyesLr.SetActive(true);
                    eyesUd.SetActive(false);
                    eyesLr.transform.rotation = Quaternion.Euler(-90, 90, 90);

                    defaultlr.SetActive(true);
                    defaultud.SetActive(false);
                    defaultlr.transform.rotation = Quaternion.Euler(270, -90, -90);

                    break;

                case Direction.right:
                    eyesLr.SetActive(true);
                    eyesUd.SetActive(false);
                    eyesLr.transform.rotation = Quaternion.Euler(90, 90, 90);

                    defaultlr.SetActive(true);
                    defaultud.SetActive(false);
                    defaultlr.transform.rotation = Quaternion.Euler(90, 0, 0);

                    break;

                case Direction.up:
                    eyesLr.SetActive(false);
                    eyesUd.SetActive(true);
                    eyesUd.transform.rotation = Quaternion.Euler(-90, 90, 90);

                    defaultlr.SetActive(false);
                    defaultud.SetActive(true);
                    defaultud.transform.rotation = Quaternion.Euler(270, -90, -90);

                    break;

                case Direction.down:
                    eyesLr.SetActive(false);
                    eyesUd.SetActive(true);
                    eyesUd.transform.rotation = Quaternion.Euler(90, 90, 90);

                    defaultlr.SetActive(false);
                    defaultud.SetActive(true);
                    defaultud.transform.rotation = Quaternion.Euler(90, 0, 0);

                    break;

                default:
                    break;
            }
        }
        this.moveDir = dir;
    }

    public void Move(Direction dir)
    {
        ChangeDirection(dir);

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
        transform.Translate(moveX * GetSpeed(), 0, moveZ * GetSpeed());
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

        //posQue.Enqueue(path.corners[0]);
        //posQue.Enqueue(path.corners[1]);
        //posQue.Enqueue(path.corners[2]);

        checkPoints = path.corners;

        GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
        foreach (GameObject point in points)
        {
            Destroy(point);
        }

        // point render
        /*
        for (int i = 1; i < path.corners.Length; i++)
        {
            GameObject point = Instantiate(pointPrefab);
            point.transform.position = path.corners[i];
        }
        */

        //state = GhostState.chase;

        agent.enabled = false;
    }

    public void ChangeTarget(Vector3 pos)
    {
        targetObj.transform.position = pos;
        ChaseTarget();
    }

    public Vector3[] GetCorners()
    {
        return this.checkPoints;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        if (blue)
        {
            switch (state) {
                case GhostState.blue:
                case GhostState.eyes:
                case GhostState.gohome:
                case GhostState.waiting:
                    break;
                default:
                    StartCoroutine(Eaten());
                    break;
            }
        } else
        {
            switch (state)
            {
                case GhostState.blue:
                case GhostState.eyes:
                case GhostState.gohome:
                    break;
                default:
                    pacman.GetComponent<PlayerScript>().Death();
                    break;
            }
        }
    }

    IEnumerator Eaten()
    {
        // display score;
        controller.StartSE(SoundEffect.eatghost);

        controller.Freeze();

        ijike.SetActive(false);
        eyesSkin.SetActive(true);

        point.GetComponent<Text>().text = controller.GetEatGhostScore().ToString();
        point.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        point.SetActive(true);

        controller.StartSE(SoundEffect.eatghost);
        controller.IncreaseGoHome();

        yield return new WaitForSeconds(1);

        point.SetActive(false);

        controller.UnFreeze();

        state = GhostState.eyes;
        blue = false;
        posQue.Clear();

        controller.StartBGM();
    }
}
