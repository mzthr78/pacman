using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitScript : MonoBehaviour
{
    public GameObject GameController;
    GameController controller;
    public GameObject point;
    public AudioClip eatFruitSE;
    public GameObject fruit;

    bool ate = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameController.GetComponent<GameController>();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!ate)
            {
                StartCoroutine(GetFruit());
            }
        }
    }

    IEnumerator GetFruit()
    {
        ate = true;

        fruit.SetActive(false);

        GetComponent<AudioSource>().PlayOneShot(eatFruitSE);

        point.GetComponent<Text>().text = 200.ToString();
        point.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        point.SetActive(true);
        controller.AddScore(200);
        yield return new WaitForSeconds(1f);
        point.SetActive(false);

        Destroy(gameObject);
    }
}
