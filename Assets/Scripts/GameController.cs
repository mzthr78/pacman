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

    private List<List<char>> map;

    private void Awake()
    {
        LoadMapData();
    }

    void Start()
    {
        navMeshsurfase.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("mouse position = " + Input.mousePosition + ":" + hit.point);
            }
        }
    }

    public List<List<char>> GetMap()
    {
        return map;
    }

    void LoadMapData()
    {
        TextAsset textasset = new TextAsset();         textasset = Resources.Load("mapdata", typeof(TextAsset)) as TextAsset; 
        string text = textasset.text;
        string[] lines = text.Split('\n');

        map = new List<List<char>>();

        foreach(string line in lines)
        {
            List<char> row = new List<char>();
            foreach(char c in line)
            {
                row.Add(c);
            }
            map.Add(row);
        }

        int lineNo = 15;
        foreach (string line in lines)
        {
            for (int j = 0; j < line.Length; j++)
            {
                switch (line[j])
                {
                    case '#':
                        GameObject obstruct = Instantiate(obstructPrefab);
                        obstruct.transform.position = new Vector3(-13.5f + j, 1f, lineNo);
                        break;
                    case '.':
                        GameObject cookie = Instantiate(cookiePrefab);
                        cookie.transform.position = new Vector3(-13.5f + j, 0.5f, lineNo);
                        break;
                    case '@':
                        GameObject powerCookie = Instantiate(powerCookiePrefab);
                        powerCookie.transform.position = new Vector3(-13.5f + j, 0.5f, lineNo);
                        break;
                    default:
                        break;
                }
            }
            lineNo--;
        }
    }
}
