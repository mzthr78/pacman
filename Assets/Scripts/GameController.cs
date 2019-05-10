using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum Direction : int {
    none = -1,
    up = 0,
    right = 1,
    down = 2,
    left = 3
}

public enum SoundEffect
{
    start,
    ghost,
    ijike
}

public struct mapdata
{
    public char objchar;
    public Vector3 coordinate;
}

public struct xz
{
    public int x;
    public int z;
}

public class GameController : MonoBehaviour
{
    int[] vx = { 0, 1, 0, -1 };
    int[] vz = { 1, 0, -1, 0 };

    public GameObject obstructPrefab;

    public GameObject cookiePrefab;
    public GameObject powerCookiePrefab;
    public NavMeshSurface navMeshsurfase;

    public GameObject pacman;

    public GameObject blinky;
    public GameObject inky;
    public GameObject pinky;
    public GameObject clyde;

    public GameObject sightBlinky;
    public GameObject navRouteBlinky;

    AudioSource aud;

    public AudioClip ghostSE;
    public AudioClip startSE;
    public AudioClip ijikeSE;

    public Text mpostext;

    private List<List<mapdata>> map;

    List<xz> passable;

    private void Awake()
    {
        passable = new List<xz>();
        LoadMapData();
    }

    void Start()
    {
        this.aud = GetComponent<AudioSource>();
        this.aud.PlayOneShot(startSE);

        navMeshsurfase.BuildNavMesh();

        blinky.SetActive(true);
        inky.SetActive(true);
        pinky.SetActive(true);
        clyde.SetActive(true);

        sightBlinky.SetActive(true);
        navRouteBlinky.SetActive(true);

        StartCoroutine(LetsStart());
    }

    IEnumerator LetsStart()
    {
        float f2 = 1.6f;
        float ma = 4.5f;

        yield return new WaitForSeconds(f2); // 開始音楽が鳴りやむまで

        Debug.Log(f2 + " seconds passed");

        yield return new WaitForSeconds((ma - f2)); // 開始音楽が鳴りやむまで

        UnFreeze();

        PlaySE(SoundEffect.ghost);

        yield return new WaitForSeconds(0.45f);

    }

    void UnFreeze()
    {
        GameObject[] powerCookies = GameObject.FindGameObjectsWithTag("PowerCookie");

        foreach (GameObject powerCookie in powerCookies)
        {
            powerCookie.GetComponent<PowerCookieScript>().UnFreeze();
        }

        blinky.GetComponent<GhostScript>().UnFreeze();
        //inky.GetComponent<GhostScript>().UnFreeze();
        //pinky.GetComponent<GhostScript>().UnFreeze();
        //clyde.GetComponent<GhostScript>().UnFreeze();

        blinky.GetComponent<GhostScript>().ChaseTarget();

        PlayerScript player = pacman.GetComponent<PlayerScript>();
        player.UnFreeze();
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

    public void PlaySE(SoundEffect se)
    {
        StartCoroutine(ControllSE(se));
    }

    IEnumerator ControllSE(SoundEffect se)
    {
        if (aud.isPlaying) aud.Stop();

        switch (se)
        {
            case SoundEffect.ijike:
                aud.PlayOneShot(ijikeSE);
                yield return new WaitForSeconds(5f);
                break;
            default: //ghost
                break;
        }

        if (aud.isPlaying) aud.Stop();
        aud.Play();
        yield return null;
    }


    public Vector3 Coord2Xz(Vector3 coord)
    {
        float x = Mathf.Floor(coord.x) + 0.5f;
        float z = Mathf.Round(coord.z);

        return new Vector3(x, 0, z);
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

        // textfile -> list<list<T>>
        int RowNo = 0;
        foreach(string line in lines)
        {
            List<mapdata> row = new List<mapdata>();
            int ColNo = 0;
            foreach(char c in line)
            {
                mapdata tmp = new mapdata();

                tmp.objchar = c;
                //tmp.coordinate = new Vector3(-13.5f + ColNo , 0, 15 - RowNo);
                tmp.coordinate = Xz2Coord(ColNo, RowNo);

                row.Add(tmp);
                ColNo++;
            }
            map.Add(row);
            RowNo++;
        }

        int[] vx = { 0, 1, 0, -1 };
        int[] vz = { -1, 0, 1, 0 };

        // List<List<T>> -> object(obstruct, cookie, etc...)
        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                char[] dirobj = new char[4];
                for (int k = 0; k < 4; k++)
                {
                    int x = j + vx[k];
                    int z = i + vz[k];

                    // Debug.Log("[" + i + "][" + j + "][" + x + "][" + z + "]");
                    if (x >= 0 && x < 28 && z >= 0 && z < 31)
                    {
                        dirobj[k] = lines[z][x];
                    } else
                    {
                        dirobj[k] = '*';
                    }
                }

                switch (lines[i][j])
                {
                    case '#':
                        GameObject obstruct = Instantiate(obstructPrefab);
                        obstruct.transform.position = new Vector3(-13.5f + j, 0.1f, 15 - i);

                        for (int k = 0; k < 4; k++)
                        {
                            if (dirobj[k] == '#')
                            {
                                GameObject obstruct2 = Instantiate(obstructPrefab);
                                obstruct2.transform.position = new Vector3(-13.5f + j + 0.5f * vx[k], 0.1f, 15 - i - 0.5f * vz[k]);
                            }
                        }

                        break;
                    case '.':
                        GameObject cookie = Instantiate(cookiePrefab);
                        cookie.transform.position = new Vector3(-13.5f + j, 0.1f, 15 - i);
                        break;
                    case '@':
                        GameObject powerCookie = Instantiate(powerCookiePrefab);
                        powerCookie.transform.position = new Vector3(-13.5f + j, 0.1f, 15 - i);
                        break;
                    default:
                        break;
                }

                if (lines[i][j] == '.' || lines[i][j] == '@' || lines[i][j] == ' ')
                {
                    xz tmp = new xz();
                    tmp.x = j;
                    tmp.z = i;
                    passable.Add(tmp);
                }
            }
        }
    }
}
