using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject pacman;
    public GameObject pacmano;
    public GameObject pacmanc;

    public AudioClip cookieSE;

    float speed = 0.1f;

    float moveX;
    float moveZ;

    bool freeze = true;

    Direction dir;

    public void UnFreeze()
    {
        dir = Direction.left;
        StartCoroutine(Pacpac());
        this.freeze = false;
    }

    int pac = 0;
    int incdec = 1;

    // パックマンがパクパクする
    IEnumerator Pacpac()
    {
        while (true)
        {
            pac += incdec;

            switch (pac)
            {
                case 0:
                    pacmanc.SetActive(true);
                    pacman.SetActive(false);
                    pacmano.SetActive(false);
                    incdec = 1;
                    break;
                case 1:
                    pacmanc.SetActive(false);
                    pacman.SetActive(true);
                    pacmano.SetActive(false);
                    break;
                case 2:
                    pacmanc.SetActive(false);
                    pacman.SetActive(false);
                    pacmano.SetActive(true);
                    incdec = -1;
                    break;
            }

            Debug.Log(pac);

            yield return new WaitForSeconds(0.07f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze) return;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            dir = Direction.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            dir = Direction.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir = Direction.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            dir = Direction.right;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("space!");
            GetComponent<AudioSource>().PlayOneShot(cookieSE);
        }
        else
        {

        }

        Debug.Log("x = " + moveX + " z = " + moveZ);

        Move(dir);
        //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0, Input.GetAxisRaw("Vertical") * speed));
    }

    void Move(Direction d)
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
            case Direction.right:
                pacman.transform.rotation = Quaternion.Euler(90, 0, 0);
                pacmano.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Direction.up:
                pacman.transform.rotation = Quaternion.Euler(90, 0, 90);
                pacmano.transform.rotation = Quaternion.Euler(90, 0, 90);
                break;
            case Direction.down:
                pacman.transform.rotation = Quaternion.Euler(90, 90, 0);
                pacmano.transform.rotation = Quaternion.Euler(90, 90, 0);
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
                Debug.Log("trigger collider name = " + other.name + " tag = " + other.tag);
                break;
        }

    }

}
