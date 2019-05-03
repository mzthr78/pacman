using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum Direction {
    none,
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

    public GameObject pacman;

    public GameObject blinky;
    public GameObject inky;
    public GameObject pinky;
    public GameObject Clyde;

    public GameObject sightBlinky;
    public GameObject navRouteBlinky;

    AudioSource aud;

    public AudioClip startSE;
    public AudioClip ijikeSE;

    public Text mpostext;

    private List<List<mapdata>> map;

    private void Awake()
    {
        LoadMapData();
    }

    void Start()
    {
        this.aud = GetComponent<AudioSource>();
        this.aud.PlayOneShot(startSE);

        navMeshsurfase.BuildNavMesh();

        Clyde.SetActive(false);

        sightBlinky.SetActive(true);
        navRouteBlinky.SetActive(false);

        StartCoroutine(LetsStart());
    }

    IEnumerator LetsStart()
    {
        yield return new WaitForSeconds(4.5f); // 開始音楽が鳴りやむまで

        // Start Processes
        pacman.GetComponent<PlayerScript>().UnFreeze();
    }

    // Update is called once per frame
    void Update()
    {
        // これだと、余韻部分で待ちすぎになる
        /*
        if (!GetComponent<AudioSource>().isPlaying)
        {
            Debug.Log("nari owattayo!");
        }
        */

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mpostext.text = "(" + hit.point.x + ", " + hit.point.z + ")";
        }
    }

    public void EatPowerCookie()
    {
        if (aud.isPlaying)
        {
            aud.Stop();
        }
        aud.volume = 0.7f;
        aud.PlayOneShot(ijikeSE);
    }

    public Vector3 Coord2Xz(Vector3 coord)
    {
        float x = Mathf.Floor(coord.x) + 0.5f;
        float z = Mathf.Round(coord.z) - 0.5f;

        return new Vector3(0, 0, 0);
    }

    public Vector3 Xz2Coord(int x, int z)
    {
        return new Vector3(-13.5f + x, 0, 15 - z);
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
                //tmp.coordinate = new Vector3(-13.5f + ColNo , 0, 15 - RowNo);
                tmp.coordinate = Xz2Coord(ColNo, RowNo);

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
