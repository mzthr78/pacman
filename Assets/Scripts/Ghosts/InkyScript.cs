﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyScript : MonoBehaviour
{
    public GameObject LeftRight;
    public GameObject TopDown;

    private void Awake()
    {
        TopDown.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}