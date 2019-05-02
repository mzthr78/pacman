using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject pacman;

    float speed = 0.2f;

    void ChangeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.left:
                pacman.transform.rotation = Quaternion.Euler(90, 90, 270);
                break;
            case Direction.right:
                pacman.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Direction.up:
                pacman.transform.rotation = Quaternion.Euler(90, 0, 90);
                break;
            case Direction.down:
                pacman.transform.rotation = Quaternion.Euler(90, 90, 0);
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
        }
        else
        {

        }

        //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0, Input.GetAxisRaw("Vertical") * speed));
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("player trigger enter");
    }

    IEnumerator Pacpac()
    {
        yield return null;
    }
}
