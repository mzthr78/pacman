using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissScript : MonoBehaviour
{
    public GameObject pacmanp;

    public GameObject pacman;
    public GameObject pacmano;
    public GameObject pacmanc;

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

    List<GameObject> missAnim;

    public void miss()
    {
        missAnim = new List<GameObject>();

        pacman.SetActive(false);
        pacmano.SetActive(false);

        missAnim.Add(pacmanc);
        missAnim.Add(pacmand1);
        missAnim.Add(pacmand2);
        missAnim.Add(pacmand3);
        missAnim.Add(pacmand4);
        missAnim.Add(pacmand5);
        missAnim.Add(pacmand6);
        missAnim.Add(pacmand7);
        missAnim.Add(pacmand8);
        missAnim.Add(pacmand9);
        missAnim.Add(pacmand10);
        missAnim.Add(vanish);

        transform.position = pacmanp.transform.position;

        missAnim[0].SetActive(true);

        for (int i = 1; i < missAnim.Count; i++)
        {
            missAnim[i].SetActive(false);
        }

        StartCoroutine(missAnimation());
        StopCoroutine(missAnimation());
    }

    IEnumerator missAnimation()
    {
        for (int i = 1; i < missAnim.Count; i++)
        {
            missAnim[i - 1].SetActive(false);
            missAnim[i].SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        missAnim[missAnim.Count - 1].SetActive(false);
    }
}
