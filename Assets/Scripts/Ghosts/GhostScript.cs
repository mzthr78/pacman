using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    public GameObject leftright;
    public GameObject updown;
    float speed = 0.3f;

    private Direction dir;

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

    // 動作確認用
    void Updatexxx()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ChangeDirection(Direction.up);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            ChangeDirection(Direction.down);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            ChangeDirection(Direction.left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            ChangeDirection(Direction.right);
        }
        else
        {

        }

        transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0, Input.GetAxisRaw("Vertical") * speed));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ghost trigger enter");
    }

    public void SetDirection(Direction dir)
    {
        this.dir = dir;
    }

    public Direction GetDirection()
    {
        return this.dir;
    }

    public void ChangeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.left:
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(270, -90, -90);
                break;
            case Direction.right:
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case Direction.up:
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(270, -90, -90);
                break;
            case Direction.down:
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            default:
                break;
        }
        this.dir = dir;
    }

}
