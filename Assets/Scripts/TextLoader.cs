using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLoader : MonoBehaviour
{
    public GameObject obstructPrefab;
    public GameObject cookiePrefab;
    public GameObject powerCookiePrefab;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset textasset = new TextAsset();
        textasset = Resources.Load("mapdata", typeof(TextAsset)) as TextAsset;
        string text = textasset.text;
        string[] lines = text.Split('\n');

        int i = 15;
        foreach (string line in lines)
        {
            for (int j = 0; j < line.Length; j++)
            {
                switch (line[j])
                {
                    case '#':
                        GameObject obstruct = Instantiate(obstructPrefab);
                        obstruct.transform.position = new Vector3(-13.5f + j, 1, i);
                        break;
                    case '.':
                        GameObject cookie = Instantiate(cookiePrefab);
                        cookie.transform.position = new Vector3(-13.5f + j, 0.5f, i);
                        break;
                    case '@':
                        GameObject powerCookie = Instantiate(powerCookiePrefab);
                        powerCookie.transform.position = new Vector3(-13.5f + j, 0.5f, i);
                        break;
                    default:
                        break;
                }
            }
            i--;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("mouse position = "+ Input.mousePosition);
        }
    }
}
