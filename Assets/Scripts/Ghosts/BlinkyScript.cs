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

    public Transform target;

    public GameObject myRoute;

    GhostScript ghost;

    private void Awake()
    {
        ghost = GetComponent<GhostScript>();
        ghost.SetDirection(Direction.none);

        target.position = controller.Xz2Coord(9, 5);
        target.position = controller.Xz2Coord(6, 12);
    }

    void Start()
    {
        //ghost.Freeze();
        //ghost.UnFreeze();

        //StartCoroutine(FirstTarget());

        ghost.ChaseTarget();
    }

    IEnumerator FirstTarget()
    {
        target.position = controller.Xz2Coord(13, 5);
        ghost.SearchTarget(target);
        yield return new WaitForSeconds(0.5f);
    }
}
