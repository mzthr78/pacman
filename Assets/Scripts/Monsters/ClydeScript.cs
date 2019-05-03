using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeScript : MonoBehaviour
{
    public GameObject LeftRight;
    public GameObject TopDown;

    private void Awake()
    {
        TopDown.SetActive(true);
    }
}
