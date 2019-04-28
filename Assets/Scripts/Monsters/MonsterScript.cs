using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public GameObject leftright;
    public GameObject updown;
    float speed = 0.3f;

    enum State {
        Waiting
    };

    State state = State.Waiting;

    private void Awake()
    {
        //leftright.SetActive(true);
        //updown.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Updatexxx()
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
        Debug.Log("monster trigger enter");
    }

    public void ChangeDirection(string direction)
    {
        switch (direction)
        {
            case "left":
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(270, -90, -90);
                break;
            case "right":
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case "up":
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(270, -90, -90);
                break;
            case "down":
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            default:
                break;
        }
    }

}
