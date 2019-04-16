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

    void ChangeDirection(string direction)
    {
        switch (direction)
        {
            case "Up":
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(180, 90, 90);
                break;
            case "Down":
                leftright.SetActive(false);
                updown.SetActive(true);
                updown.transform.rotation = Quaternion.Euler(0, 90, 90);
                break;
            case "Left":
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(270, -90, -90);
                break;
            case "Right":
                leftright.SetActive(true);
                updown.SetActive(false);
                leftright.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        leftright.SetActive(true);
        updown.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Waiting:
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("monster trigger enter");
    }
}
