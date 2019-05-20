using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissScript : MonoBehaviour
{
    public GameObject pacman;

    public GameObject pacmano;

    public GameObject pacmand1;
    public GameObject pacmand2;
    public GameObject pacmand3;
    public GameObject pacmand4;
    public GameObject pacmand5;
    public GameObject pacmand6;
    public GameObject pacmand7;
    public GameObject pacmand8;
    public GameObject pacmand9;
    public GameObject pacmand10;

    public GameObject vanish;

    List<GameObject> missAnim = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        missAnim.Add(pacmano);

        transform.position = pacman.transform.position;

        pacmano.SetActive(true);

        pacmand1.SetActive(false);
        pacmand2.SetActive(false);
        pacmand3.SetActive(false);
        pacmand4.SetActive(false);
        pacmand5.SetActive(false);
        pacmand6.SetActive(false);
        pacmand7.SetActive(false);
        pacmand8.SetActive(false);
        pacmand9.SetActive(false);
        pacmand10.SetActive(false);

        vanish.SetActive(false);

        StartCoroutine(missAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator missAnimation()
    {
        pacmano.SetActive(false);
        pacmand1.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        pacman.SetActive(false);
        pacmand1.SetActive(true);
        yield return new WaitForSeconds(0.1f);


        yield return null;
    }
}
