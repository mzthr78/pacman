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
    public GameController controller;

    public GameObject myRoute;

    GhostScript ghost;

    private void Awake()
    {
        ghost = GetComponent<GhostScript>();
        ghost.SetDirection(Direction.none);

        //target.position = controller.Xz2Coord(9, 5);
        //target.position = controller.Xz2Coord(6, 12);
        //target.position = controller.Xz2Coord(0, 4);
    }

    void Start()
    {
        //ghost.Freeze();
        //ghost.UnFreeze();

        //StartCoroutine(FirstTarget());

        ghost.ChaseTarget();
    }

    private void Update()
    {
        /*
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                float rx = Mathf.Floor(hit.point.x) + 0.5f;
                float rz = Mathf.Round(hit.point.z);
                target.position = new Vector3(rx, target.position.y, rz);
                ghost.SearchTarget(target);
            }
        }
        */
    }
}
