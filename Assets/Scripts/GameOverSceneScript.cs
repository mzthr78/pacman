using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneScript : MonoBehaviour
{
    public GameObject obstructPrefab;
    public GameObject infoText;

    private List<List<mapdata>> map;

    int[] vx = { 0, 1, 0, -1 };
    int[] vz = { 1, 0, -1, 0 };

    private void Awake()
    {
        TextAsset textasset = new TextAsset();
        textasset = Resources.Load("mapdata", typeof(TextAsset)) as TextAsset;

        string text = textasset.text;
        string[] lines = text.Split('\n');

        map = new List<List<mapdata>>();

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                char[] dirobj = new char[4];
                for (int k = 0; k < 4; k++)
                {
                    int x = j + vx[k];
                    int z = i + vz[k];

                    // Debug.Log("[" + i + "][" + j + "][" + x + "][" + z + "]");
                    if (x >= 0 && x < 28 && z >= 0 && z < 31)
                    {
                        dirobj[k] = lines[z][x];
                    }
                    else
                    {
                        dirobj[k] = '*';
                    }
                }

                switch (lines[i][j])
                {
                    case '#':
                        GameObject obstruct = Instantiate(obstructPrefab);
                        obstruct.transform.position = new Vector3(-13.5f + j, 0.1f, 15 - i);

                        for (int k = 0; k < 4; k++)
                        {
                            if (dirobj[k] == '#')
                            {
                                GameObject obstruct2 = Instantiate(obstructPrefab);
                                obstruct2.transform.position = new Vector3(-13.5f + j + 0.5f * vx[k], 0.1f, 15 - i - 0.5f * vz[k]);
                            }
                        }
                        break;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        infoText.SetActive(false);
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        /*
        GameObject[] obstructs = GameObject.FindGameObjectsWithTag("Obstruct");
        Color origColor = obstructs[0].GetComponent<Renderer>().material.color;

        int count = 8;

        while (count > 0)
        {
            for (int i = 0; i < obstructs.Length; i++)
            {
                if (count % 2 == 0)
                {
                    obstructs[i].GetComponent<Renderer>().material.color = Color.white;
                }
                else
                {
                    obstructs[i].GetComponent<Renderer>().material.color = origColor;
                }
            }
            count--;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.8f);
        */

        yield return new WaitForSeconds(1f);
        infoText.SetActive(true);
    }
}
