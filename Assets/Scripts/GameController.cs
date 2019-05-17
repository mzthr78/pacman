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

public enum GameMode
{
    usual,
    blue,
}

public enum SoundEffect
{
    start,
    ghost,
    ijike,
    eatghost,
    gohome,
    death,
    none
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
    GameMode mode = GameMode.usual;

    int[] vx = { 0, 1, 0, -1 };
    int[] vz = { 1, 0, -1, 0 };

    public GameObject obstructPrefab;

    public GameObject cookiePrefab;
    public GameObject powerCookiePrefab;
    public NavMeshSurface navMeshsurfase;

    public GameObject pacman;

    PlayerScript pacmanScript;

    public GameObject blinky;
    public GameObject inky;
    public GameObject pinky;
    public GameObject clyde;

    bool freeze = false;
    bool pause = false;

    GhostScript blinkyScript;
    GhostScript inkyScript;
    GhostScript pinkyScript;
    GhostScript clydeScript;

    public GameObject sightBlinky;
    public GameObject navRouteBlinky;

    public Text infoText;

    AudioSource aud;

    SoundEffect currentSE;

    public AudioClip ghostSE;
    public AudioClip startSE;
    public AudioClip ijikeSE;
    public AudioClip eatGhostSE;
    public AudioClip goHomeSE;
    public AudioClip deathSE;

    public Text mpostext;

    private List<List<mapdata>> map;
    List<xz> passable;

    int ijikeScore = 100;

    float span = 0.1f;
    float delta = 0;

    public Text ScoreText;
    int score = 0;

    int goHome = 0;
    int remain = 3;

    public GameObject PacmanR1;
    public GameObject PacmanR2;
    public GameObject PacmanR3;

    private void Awake()
    {
        passable = new List<xz>();
        LoadMapData();

        pacman.SetActive(false);

        blinky.SetActive(false);
        inky.SetActive(false);
        pinky.SetActive(false);
        clyde.SetActive(false);

        HideInfoText();
    }

    void Start()
    {
        this.aud = GetComponent<AudioSource>();
        //this.aud.PlayOneShot(startSE);

        navMeshsurfase.BuildNavMesh();

        pacmanScript = pacman.GetComponent<PlayerScript>();

        blinky.GetComponent<NavMeshAgent>().enabled = false;
        inky.GetComponent<NavMeshAgent>().enabled = false;
        pinky.GetComponent<NavMeshAgent>().enabled = false;
        clyde.GetComponent<NavMeshAgent>().enabled = false;

        blinkyScript = blinky.GetComponent<GhostScript>();
        inkyScript = inky.GetComponent<GhostScript>();
        pinkyScript = pinky.GetComponent<GhostScript>();
        clydeScript = clyde.GetComponent<GhostScript>();

        blinkyScript.SetState(GhostState.usual);
        inkyScript.SetState(GhostState.waiting);
        pinkyScript.SetState(GhostState.waiting);
        clydeScript.SetState(GhostState.waiting);

        sightBlinky.SetActive(true);
        navRouteBlinky.SetActive(true);

        StartCoroutine(LetsStart());
    }

    IEnumerator LetsStart()
    {
        float f2 = 1.2f;
        float f3 = 1.4f;
        float ma = 4.7f;

        yield return new WaitForSeconds(0.2f);

        aud.PlayOneShot(startSE);

        yield return new WaitForSeconds(f2); // 開始音楽が鳴りやむまで

        //Debug.Log(f2 + " seconds passed");

        blinky.SetActive(true);
        inky.SetActive(true);
        pinky.SetActive(true);
        clyde.SetActive(true);
        pacman.SetActive(true);

        pinkyScript.ChangeDirection(Direction.down);

        yield return new WaitForSeconds(f3); // 

        ShowInfoText("READY!");

        yield return new WaitForSeconds((ma - f2 - f3)); // 開始音楽が鳴りやむまで

        HideInfoText();

        UnFreeze();
        pacmanScript.LetsStart();

        PlaySE(SoundEffect.ghost);

        yield return new WaitForSeconds(0.45f);

    }

    public bool IsFreeze()
    {
        return this.freeze;
    }

    public void Freeze()
    {
        aud.enabled = false;

        GameObject[] powerCookies = GameObject.FindGameObjectsWithTag("PowerCookie");

        foreach (GameObject powerCookie in powerCookies)
        {
            powerCookie.GetComponent<PowerCookieScript>().Freeze();
        }

        blinkyScript.Freeze();
        inkyScript.Freeze();
        pinkyScript.Freeze();
        clydeScript.Freeze();

        pacmanScript.Freeze();

        this.freeze = true;
    }

    public void UnFreeze()
    {
        GameObject[] powerCookies = GameObject.FindGameObjectsWithTag("PowerCookie");

        foreach (GameObject powerCookie in powerCookies)
        {
            powerCookie.GetComponent<PowerCookieScript>().UnFreeze();
        }

        blinkyScript.UnFreeze();
        inkyScript.UnFreeze();
        pinkyScript.UnFreeze();
        clydeScript.UnFreeze();

        //blinkyScript.ChaseTarget();

        pacmanScript.UnFreeze();

        //PlaySE(currentSE);
        StartBGM();

        this.freeze = false;
    }

    float blueSpan = 10;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause)
            {
                HideInfoText();
                UnFreeze();
                pause = false;
            }
            else
            {
                ShowInfoText("PAUSE");
                Freeze();
                pause = true;
            }
        }

        if (this.freeze) return;

        ScoreText.text = score.ToString();

        switch (mode) {
            case GameMode.blue:
                delta += Time.deltaTime;

                if (delta > blueSpan)
                {
                    mode = GameMode.usual;
                    Ghosts2Blue(false);
                }
                break;
            default:
                break;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mpostext.text = "(" + hit.point.x + ", " + hit.point.z + ")";
        }
    }

    public GameMode GetGameMode()
    {
        return this.mode;
    }

    public float GetBlueSpan()
    {
        return blueSpan;
    }

    public void AddScore(int score)
    {
        this.score += score;
    }

    public void Ghosts2Blue(bool blue = true)
    {
        blinkyScript.BeBlue(blue);
        inkyScript.BeBlue(blue);
        pinkyScript.BeBlue(blue);
        clydeScript.BeBlue(blue);

        if (blue)
        {
            ijikeScore = 100;
            delta = 0;
            mode = GameMode.blue;
            StartBGM();
        }
        else
        {
            mode = GameMode.usual;
            StopSE();
        }
    }

    void HideInfoText()
    {
        infoText.enabled = false;
    }

    void ShowInfoText(string message)
    {
        infoText.text = message;
        infoText.enabled = true;
    }

    public void PlaySE(SoundEffect se)
    {
        //StartCoroutine(ControlSE(se));
    }

    public void StartBGM()
    {
        if (goHome > 0)
        {
            StartSE(SoundEffect.gohome);
        }
        else
        {
            switch (mode)
            {
                case GameMode.blue:
                    StartSE(SoundEffect.ijike);
                    break;
                default:
                    StartSE(SoundEffect.ghost);
                    break;
            }
        }
    }

    public void IncreaseGoHome()
    {
        if (this.goHome < 3)
        {
            this.goHome++;
        }
    }

    public void DecreaseGoHome()
    {
        if (this.goHome > 0)
        {
            this.goHome--;
        }
    }

    public void StopSE()
    {
        aud.Stop();
        StartBGM();
    }

    public void StartSE(SoundEffect se = SoundEffect.ghost)
    {
        aud.enabled = true;

        if (aud.isPlaying) aud.Stop();

        switch (se)
        {
            case SoundEffect.ijike:
                aud.PlayOneShot(ijikeSE);
                break;
            case SoundEffect.eatghost:
                aud.PlayOneShot(eatGhostSE);
                break;
            case SoundEffect.gohome:
                aud.PlayOneShot(goHomeSE);
                break;
            case SoundEffect.death:
                aud.PlayOneShot(deathSE);
                break;
            default:
                aud.Play();
                break;
        }
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

    public List<xz> GetPassable()
    {
        return this.passable;
    }

    public int GetEatGhostScore()
    {
        score += ijikeScore;
        ijikeScore *= 2;
        return ijikeScore;
    }

    public void DecreasePacman()
    {
        remain--;

        if (remain < 3) PacmanR3.SetActive(false);
        if (remain < 2) PacmanR2.SetActive(false);
        if (remain < 1) PacmanR1.SetActive(false);
    }

    public void Reset()
    {
        DecreasePacman();

        blinkyScript.Reset();
        inkyScript.Reset();
        pinkyScript.Reset();
        clydeScript.Reset();

        pacmanScript.Reset();

        UnFreeze();
    }
}
