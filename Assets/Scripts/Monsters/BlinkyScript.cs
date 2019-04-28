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
    public Text mpostext;

    Queue<Vector3>  posQue;

    public GameObject myRoute;

    public GameObject LeftRight;
    public GameObject TopDown;

    NavMeshAgent agent;
    NavMeshPath path;

    float speed = 5.0f;
    private float startTime;
    private float journeyLength;

    int[] vx = {  0,  1,  0, -1 };
    int[] vy = {  1,  0, -1,  0 };

    MonsterStatus status;
    private Vector3[] corners;
    private int seq = 0;

    private bool freeze = true;

    private List<Vector3> mycorners;

    private void Awake()
    {
        mycorners = myRoute.GetComponent<MyRouteScript>().GetMyRoute();

        for (int i = 0; i < mycorners.Count; i++)
        {
            Debug.Log("mycorners[" + i + "]" + mycorners[i]);
        }

        seq = 0;
    }

    // Start is called before the first frame update
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
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mpostext.text = "(" + hit.point.x + ", " + hit.point.z + ")";
        }

        if (freeze && Input.GetMouseButtonDown(0))
        {
            SearchTarget();
            freeze = false;
            seq = 0;
        }

        if (!freeze)
        {
            if (posQue.Count > 0) {
                Vector3 nxt = posQue.Dequeue();
                Debug.Log("nxgt = " + nxt);
            } else
            {
                Debug.Log("もうないよ！");
                freeze = true;
            }
        }
    }
    // Update is called once per frame
    void Updatexxx()
    {
        RaycastHit hit;         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);         if (Physics.Raycast(ray, out hit))         {
            mpostext.text = "(" + hit.point.x + ", " + hit.point.z + ")";         } 
        if (freeze && Input.GetMouseButtonDown(0))
        {
            SearchTarget();
            freeze = false;
            seq = 0;
        }

        if (!freeze)
        {
            if (seq < mycorners.Count)
            {
                Vector3 sourcePosition = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 targetPosition = new Vector3(mycorners[seq].x, 0, mycorners[seq].z);
                float distance = Vector3.Distance(sourcePosition, targetPosition);
                if (distance > 0.15f)
                {
                    Debug.Log("seek " + seq);
                    float srcx = sourcePosition.x;
                    float dstx = targetPosition.x;

                    float srcz = sourcePosition.z;
                    float dstz = targetPosition.z;

                    float step = Time.deltaTime * 7;
                    //Debug.Log(seq + ":" + transform.position + " -> " + mycorners[seq] + ", " + distance);

                    //Debug.Log("srcx = " + srcx + " dstx = " + dstx + " srcz = " + srcz + " dstz = " + dstz);

                    if (dstx - srcx > 0.2)
                    {
                        GetComponent<MonsterScript>().ChangeDirection("right");
                    } else if (dstx - srcx < -0.2)
                    {
                        GetComponent<MonsterScript>().ChangeDirection("left");
                    } else if (dstz - srcz > 0.2)
                    {
                        GetComponent<MonsterScript>().ChangeDirection("up");
                    } else if (dstz - srcz < -0.2)
                    {
                        GetComponent<MonsterScript>().ChangeDirection("down");
                    }
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                }
                else
                {
                    seq++;
                }
            }
            else
            {
                freeze = true;
            }
        }
    }

    void SearchTarget()
    {
        agent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();
        agent.CalculatePath(target.position, path);
        corners = path.corners;

        mycorners = new List<Vector3>();

        float prex = transform.position.x;
        float prez = transform.position.z;

        for (int i = 0; i < corners.Length; i++)
        {
            float x = Mathf.Floor(corners[i].x) + 0.5f;
            float y = 0;
            float z = Mathf.Round(corners[i].z);

            //mycorners.Add(new Vector3(x, y, z));
            posQue.Enqueue(new Vector3(x, y, z));

            //Debug.Log(corners[i] + " -> " + mycorners[mycorners.Count - 1]);
        }

        /* botsu
        for (int i = 0; i < corners.Length; i++)
        {
            float nxtx;
            float nxtz;

            if (Mathf.Abs(corners[i].x - prex) > Mathf.Abs(corners[i].z - prez))
            {
                nxtx = Mathf.Ceil(corners[i].x) + 0.5f;
                nxtz = prez;
            }
            else
            {
                nxtx = prex;
                nxtz = Mathf.Round(corners[i].z);
            }

            mycorners.Add(new Vector3(nxtx, 0, nxtz));

            prex = nxtx;
            prez = nxtz;
        }
        */

        status = MonsterStatus.Chase;
    }

}
