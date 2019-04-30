using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Direction {
    left,
    right,
    up,
    down
}

public struct mapdata
{
    public char objtype;
    public Vector3 coordinate;
}

public class GameController : MonoBehaviour
{
    public GameObject obstructPrefab;
    public GameObject cookiePrefab;
    public GameObject powerCookiePrefab;
    public NavMeshSurface navMeshsurfase;

    private List<List<mapdata>> map;

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
    }

    public List<List<mapdata>> GetMap()
    {
        return map;
    }

    void LoadMapData()
    {
        TextAsset textasset = new TextAsset();         textasset = Resources.Load("mapdata", typeof(TextAsset)) as TextAsset; 
        string text = textasset.text;
        string[] lines = text.Split('\n');

        map = new List<List<mapdata>>();

        int RowNo = 0;
        foreach(string line in lines)
        {
            List<mapdata> row = new List<mapdata>();
            int ColNo = 0;
            foreach(char c in line)
            {
                mapdata tmp = new mapdata();

                tmp.objtype = c;
                tmp.coordinate = new Vector3(-13.5f + ColNo , 0, 15 - RowNo);

                row.Add(tmp);
                ColNo++;
            }
            map.Add(row);
            RowNo++;
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
