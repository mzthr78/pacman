using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlinkyScript : MonoBehaviour
{
    public Transform target;

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

    }
}
