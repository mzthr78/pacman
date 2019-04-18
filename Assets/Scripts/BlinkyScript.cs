using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlinkyScript : MonoBehaviour
{
    public Transform target;

    NavMeshAgent agent;
    NavMeshPath path;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RouteNav());
        }

    }

    IEnumerator　RouteNav()
    {
        agent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();
        agent.CalculatePath(target.position, path);

        Vector3 tmp = target.position;
        foreach (Vector3 corner in path.corners)
        {
            //transform.position = corner;
            transform.position = Vector3.Lerp(tmp, corner, 10);
            //yield return new WaitForSeconds(0.3f);
            tmp = corner;
            yield return new WaitForSeconds(0.1f);
        }

    }
}
