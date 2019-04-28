using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject pacman;

    float speed = 0.2f;

    void ChangeDirection(string direction)
    {
        switch (direction)
        {
            case "Left":
                pacman.transform.rotation = Quaternion.Euler(90, 90, 270);
                break;
            case "Right":
                pacman.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case "Up":
                pacman.transform.rotation = Quaternion.Euler(90, 0, 90);
                break;
            case "Down":
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
            ChangeDirection("Up");
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            ChangeDirection("Down");
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            ChangeDirection("Left");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            ChangeDirection("Right");
        }
        else
        {

        }

        transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0, Input.GetAxisRaw("Vertical") * speed));
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("player trigger enter");
    }
}
