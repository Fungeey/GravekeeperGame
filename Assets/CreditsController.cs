using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {

    public AudioClip creditsTheme;
    public GameObject[] creditSlides;

    IEnumerator showCreditSlides() {
        for (int i = 0; i < creditSlides.Length; i++) {
            if(i != 0) {
                creditSlides[i-1].SetActive(false);
            }
            creditSlides[i].SetActive(true);
            yield return new WaitForSeconds(4.8f);
        }
        SceneManager.LoadScene(0);
    }

	// Use this for initialization
	void Start () {
        AudioSource music = GameObject.FindWithTag("MusicPlayer").GetComponent<AudioSource>();
        music.clip = creditsTheme;
        music.Play();
        music.loop = false;
    }
	
	// Update is called once per frame
	void Update () {
        StartCoroutine(showCreditSlides());

    }
}
