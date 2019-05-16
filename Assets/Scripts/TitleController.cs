using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    public GameObject panel;
    public AudioClip creditSE;

    bool pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BlinkPressMessage());
        pressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Time.deltaTime;
        if (Input.GetMouseButton(0) || Input.anyKeyDown)
        {

            if (!pressed)
            {
                StartCoroutine(PlayCreditSE());
                pressed = true;
            }
        }
    }

    IEnumerator BlinkPressMessage()
    {
        yield return null;
    }

    IEnumerator PlayCreditSE()
    {
        GetComponent<AudioSource>().PlayOneShot(creditSE);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("GameScene");
    }
}
