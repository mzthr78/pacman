﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameController controller;

    public GameObject pacman;
    public GameObject pacmano;
    public GameObject pacmanc;

    public GameObject miss;

    public AudioClip cookieSE;

    List<List<mapdata>> map;

    float speed = 0.1f;

    int[] vx = { 0, 1, 0, -1 };
    int[] vz = { -1, 0, 1, 0 };

    float moveX;
    float moveZ;

    bool freeze = true;

    Direction moveDir;

    LineRenderer line;

    public Direction GetDirection()
    {
        return this.moveDir;
    }

    public void Freeze()
    {
        this.freeze = true;
    }

    public void UnFreeze()
    {
        //dir = Direction.left;
        //StartCoroutine(Pacpac());
        this.freeze = false;
    }

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        map = controller.GetMap();
    }

    public void LetsStart()
    {
        this.moveDir = Direction.left;
        this.freeze = false;
        StartCoroutine(Pacpac());
    }

    int pac = 0;
    int incdec = 1;

    // pac-man pacpac
    IEnumerator Pacpac()
    {
        while (true)
        {
            if (!this.freeze)
            {
                pac += incdec;

                switch (pac)
                {
                    case 0:
                        pacman.SetActive(false);
                        pacmano.SetActive(false);
                        pacmanc.SetActive(true);
                        incdec = 1;
                        break;
                    case 1:
                        pacmanc.SetActive(false);
                        pacmano.SetActive(false);
                        pacman.SetActive(true);
                        break;
                    case 2:
                        pacmanc.SetActive(false);
                        pacman.SetActive(false);
                        pacmano.SetActive(true);
                        incdec = -1;
                        break;
                }
            }

            yield return new WaitForSeconds(0.07f);
        }
    }

    Direction reservedir;

    // Update is called once per frame
    void Update()
    {
        if (freeze) return;

        float rx = Mathf.Floor(transform.position.x) + 0.5f;
        float rz = Mathf.Round(transform.position.z);

        // sight (if line renderer enabled)
        if (line.enabled)
        {
            Vector3 from = transform.position;
            Vector3 to = new Vector3();
            switch (this.moveDir)
            {
                case Direction.left:
                    to = from - transform.right * 3.0f;
                    break;
                case Direction.right:
                    to = from + transform.right * 3.0f;
                    break;
                case Direction.up:
                    to = from + transform.forward * 3.0f;
                    break;
                case Direction.down:
                    to = from - transform.forward * 3.0f;
                    break;
            }

            // Line Render
            Vector3[] positions = new Vector3[2] { from, to };

            line.positionCount = 2;
            line.SetPositions(positions);

            //
            RaycastHit hit;
            if (Physics.Linecast(from, to, out hit))
            {
                Transform target = hit.transform;
                //Debug.Log(target.name);
            }
        }

        Direction tmpdir = moveDir;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            tmpdir = Direction.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            tmpdir = Direction.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            tmpdir = Direction.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            tmpdir = Direction.right;
        }
        else
        {
        }

        // Debug.Log("x = " + moveX + " z = " + moveZ);

        Vector3 coord = controller.Coord2Xz(transform.position);
        int ix = (int)(coord.x + 13.5f); 
        int iz = Mathf.Abs((int)(coord.z - 15));

        /*　for confirmation
        if (ix < 0 || ix > 27 || iz < 0 || iz > 30)
        {
            //Debug.Log("out of map!!!");
        }
        */

        char[] dirobj = new char[4];

        for (int i = 0; i < 4; i++)
        {
            // exclusion out of range
            if (iz + vz[i] >= 0 && iz + vz[i] <= 30 && ix + vx[i] >= 0 && ix + vx[i] <= 27)
            {
                dirobj[i] = map[iz + vz[i]][ix + vx[i]].objchar;
            } else
            {
                // add warp script around here 
                dirobj[i] = '*';
            }
        }

        if ((int)tmpdir >= 0 && (int)tmpdir < 4)
        {
            switch (dirobj[(int)tmpdir])
            {
                case '#':
                case '-':
                    break;
                default:
                    reservedir = tmpdir;
                    break;
            }
        }

        if (reservedir != Direction.none)
        {
            switch (reservedir)
            {
                case Direction.up:
                case Direction.down:
                    if (Mathf.Abs(transform.position.x -  rx) < 0.1f)
                    {
                        moveDir = reservedir;
                    }
                    break;
                case Direction.right:
                case Direction.left:
                    if (Mathf.Abs(transform.position.z - rz) < 0.1f)
                    {
                        moveDir = reservedir;
                    }
                    break;
            }
            reservedir = Direction.none;
        }

        if ((int)moveDir >= 0 && (int)moveDir < 4)
        {
            switch (dirobj[(int)moveDir]) 
            {
                case '#':
                case '-':
                    switch (moveDir)
                    {
                        case Direction.right:
                        case Direction.left:
                            if (Mathf.Abs(transform.position.x - rx) < 0.01f)
                            {
                                moveDir = Direction.none;
                            }
                            break;
                        case Direction.up:
                        case Direction.down:
                            if (Mathf.Abs(transform.position.z - rz) < 0.01f)
                            {
                                moveDir = Direction.none;
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        if (moveDir == Direction.left && transform.position.x < -15)
        {
            transform.position = new Vector3(15, transform.position.y, transform.position.z);
        }

        if (moveDir == Direction.right && transform.position.x > 15)
        {
            transform.position = new Vector3(-15, transform.position.y, transform.position.z);
        }

        Move(moveDir);
    }

    public void Move(Direction d)
    {
        ChangeDirection(d);

        switch (d)
        {
            case Direction.left:
                moveX = -1;
                moveZ = 0;
                break;
            case Direction.up:
                moveX = 0;
                moveZ = 1;
                break;
            case Direction.right:
                moveX = 1;
                moveZ = 0;
                break;
            case Direction.down:
                moveX = 0;
                moveZ = -1;
                break;
            default:
                moveX = 0;
                moveZ = 0;
                break;
        }

        transform.Translate(moveX * speed, 0, moveZ * speed);
    }

    void ChangeDirection(Direction d)
    {
        switch (d)
        {
            case Direction.left:
                pacman.transform.rotation = Quaternion.Euler(90, 90, 270);
                pacmano.transform.rotation = Quaternion.Euler(90, 90, 270);
                break;
            case Direction.down:
                pacman.transform.rotation = Quaternion.Euler(90, 90, 0);
                pacmano.transform.rotation = Quaternion.Euler(90, 90, 0);
                break;
            case Direction.right:
                pacman.transform.rotation = Quaternion.Euler(90, 0, 0);
                pacmano.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Direction.up:
                pacman.transform.rotation = Quaternion.Euler(90, 0, 90);
                pacmano.transform.rotation = Quaternion.Euler(90, 0, 90);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("player trigger enter");
        switch (other.tag)
        {
            case "Cookie":
                //GetComponent<AudioSource>().Play();
                GetComponent<AudioSource>().PlayOneShot(cookieSE);
                break;
            default:
                //Debug.Log("trigger collider name = " + other.name + " tag = " + other.tag);
                break;
        }

    }

    public void Death()
    {
        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        controller.Freeze();
        yield return new WaitForSeconds(1.5f);

        miss.GetComponent<MissScript>().miss();

        controller.StartSE(SoundEffect.death);
        yield return new WaitForSeconds(3);

        controller.DecreasePacman();

        // ここじゃない
        if (controller.PacmanRemain() >= 0)
        {
            controller.Reset();
        }
    }

    public void Reset()
    {
        transform.position = new Vector3(0, 0.2f, -8);
        Move(Direction.left);
    }
}
