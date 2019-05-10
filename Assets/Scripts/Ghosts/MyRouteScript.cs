using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRouteScript : MonoBehaviour
{
    LineRenderer line;
    List<Vector3> corners;

    public GameController controller;
    public GhostScript ghost;

    public Color color;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Queue<Vector3> posQue = ghost.GetPosQue();

        Vector3[] posArray = posQue.ToArray();

        line.positionCount = posArray.Length;
        line.SetPositions(posArray);

        line.startWidth = 0.15f;
        line.endWidth = 0.15f;

        line.material.color = color;

        /*
        for (int i = 0; i < posArray.Length; i++)
        {
            Debug.Log("[" + name + "]" + "posArr = [" + i + "]" + posArray[i]);
        }
        */
    }
}
