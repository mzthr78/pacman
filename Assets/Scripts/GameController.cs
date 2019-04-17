using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public GameObject obstructPrefab;
    public GameObject cookiePrefab;
    public GameObject powerCookiePrefab;
    public NavMeshSurface navMeshsurfase;

    // Start is called before the first frame update
    void Start()
    {
        LoadMapData();
        navMeshsurfase.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("mouse position = " + Input.mousePosition);
        }
    }

    void LoadMapData()
    {
        TextAsset textasset = new TextAsset();
        textasset = Resources.Load("mapdata", typeof(TextAsset)) as TextAsset;
        string text = textasset.text;
        string[] lines = text.Split('\n');

        int i = 15;
        foreach (string line in lines)
        {
            for (int j = 0; j < line.Length; j++)
            {
                switch (line[j])
                {
                    case '#':
                        GameObject obstruct = Instantiate(obstructPrefab);
                        obstruct.transform.position = new Vector3(-13.5f + j, 1, i);
                        break;
                    case '.':
                        GameObject cookie = Instantiate(cookiePrefab);
                        cookie.transform.position = new Vector3(-13.5f + j, 0.5f, i);
                        break;
                    case '@':
                        GameObject powerCookie = Instantiate(powerCookiePrefab);
                        powerCookie.transform.position = new Vector3(-13.5f + j, 0.5f, i);
                        break;
                    default:
                        break;
                }
            }
            i--;
        }
    }

}
