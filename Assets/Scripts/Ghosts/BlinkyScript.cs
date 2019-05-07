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

    Queue<Vector3>  posQue;

    public GameObject myRoute;

    public GameObject LeftRight;
    public GameObject TopDown;

    NavMeshAgent agent;
    NavMeshPath path;

    float speed = 5.0f;
    private float startTime;
    private float journeyLength;

    float posY = 0.5f;

    MonsterStatus status;
    private Vector3[] checkPoints;
    private Vector3 checkPoint;

    private bool freeze = true;

    GhostScript ghost;

    private void Awake()
    {
        ghost = GetComponent<GhostScript>();
        ghost.SetDirection(Direction.left);
    }

    void Start()
    {
        LeftRight.SetActive(true);
        ghost.GetComponent<GhostScript>().Freeze();
    }

}
