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

    // Start is called before the first frame update
    void Start()
    {
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

    private void Updateaaa()
    {

    }

    void SetMyCorners()
    {
        Vector3 corner = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);         if (Physics.Raycast(ray, out hit))         {
            mpostext.text = "(" + hit.point.x + ", " + hit.point.z + ")";         } 
        if (freeze && Input.GetMouseButtonDown(0))
        {
            SearchTarget();
            seq = 0;
            freeze = false;
        }

        if (!freeze)
        {
            if (seq < mycorners.Count)
            {
                Vector3 sourcePosition = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 targetPosition = new Vector3(mycorners[seq].x, 0, mycorners[seq].z); //0.5f adjust monster's position
                float distance = Vector3.Distance(sourcePosition, targetPosition);
                if (distance > 0.3f)
                {
                    float step = Time.deltaTime * 10;
                    transform.position = Vector3.MoveTowards(transform.position, mycorners[seq], step);
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
        float prey = transform.position.z;

        for (int i = 0; i < corners.Length; i++)
        {
            float posx = Mathf.Floor(corners[i].x);
            float posz = Mathf.Floor(corners[i].z + 0.5f) - 1;
            Vector3 mycorner = new Vector3(posx, 0, posz);
            mycorners.Add(mycorner);
        }

        for (int i = 0; i < corners.Length; i++)
        {
            Debug.Log("i:" + corners[i] + " -> " + mycorners[i]);
        }

        status = MonsterStatus.Chase;
    }

}
