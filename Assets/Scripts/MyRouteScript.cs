using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRouteScript : MonoBehaviour
{
    LineRenderer line;
    List<Vector3> corners;

    public GameObject controller;

    List<List<mapdata>> map;

    public Color color;

    public List<Vector3> GetMyRoute()
    {
        return corners;
    }

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        corners = new List<Vector3>();


        //corners.Add(new Vector3(0, 0, 3.5f));
        //corners.Add(new Vector3(4.5f, 0, 3.5f));
        //corners.Add(new Vector3(4.5f, 0, -5.5f));
        //corners.Add(new Vector3(1.5f, 0, -5.5f));


    }

    // Start is called before the first frame update
    void Start()
    {
        /* blinky 動作確認用
        map = controller.GetComponent<GameController>().GetMap();

        corners.Add(map[11][18].coordinate);
        corners.Add(map[20][18].coordinate);
        corners.Add(map[20][15].coordinate);
        corners.Add(map[23][15].coordinate);
        //corners.Add(map[23][14].coordinate);
        corners.Add(map[23][12].coordinate);
        corners.Add(map[20][12].coordinate);
        corners.Add(map[20][09].coordinate);
        corners.Add(map[11][09].coordinate);
        corners.Add(new Vector3(0, 0, 4));

        line.positionCount = corners.Count;

        for (int i = 0; i < corners.Count; i++)
        {
            line.SetPosition(i, corners[i]);
        }

        line.startWidth = 0.2f;
        line.endWidth = 0.2f;

        line.material.color = color;
        */      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
