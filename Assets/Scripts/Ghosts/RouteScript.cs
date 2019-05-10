using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RouteScript : MonoBehaviour
{
    public GhostScript ghost;
    public GameObject pointPrefab;

    public Color color;

    LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
    }

    void Update()
    {
        Vector3[] corners = ghost.GetCorners();

        if (corners == null) return;

        line.positionCount = corners.Length;
        line.SetPositions(corners);

        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        line.material.color = color;

        for (int i = 0; i < corners.Length; i++)
        {
            GameObject point = Instantiate(pointPrefab);
            point.transform.position = corners[i];
        }
    }
}
