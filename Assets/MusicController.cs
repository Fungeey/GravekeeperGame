using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {

    public AudioClip[] bgms;
    private AudioSource audioSource;
    private int musicIndex = 1; // Music will go in order (not enough songs to shuffle, don't want to get the same one twice)

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this.gameObject);
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!GetComponent<AudioSource>().isPlaying) {
            if (SceneManager.GetActiveScene().buildIndex == 0) {
                // Main menu
                audioSource.clip = bgms[0];
                audioSource.Play();
            } else {
                audioSource.clip = bgms[musicIndex];
                audioSource.Play();

                if (musicIndex == bgms.Length - 1) {
                    musicIndex = 1;
                } else {
                    musicIndex++;
                }
            }
        }
    }
}
