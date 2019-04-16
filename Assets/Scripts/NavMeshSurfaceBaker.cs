using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshSurfaceBaker : MonoBehaviour
{
    NavMeshSurface navMeshsurfase;

    // Start is called before the first frame update
    void Start()
    {
        navMeshsurfase = GetComponent<NavMeshSurface>();

        StartCoroutine(TimeUpdate());
    }

    IEnumerator TimeUpdate()
    {
        //yield return new WaitForSeconds(2.0f);

        while (true)
        {
            navMeshsurfase.BuildNavMesh();

            yield return new WaitForSeconds(5.0f);
        }
    }
}
