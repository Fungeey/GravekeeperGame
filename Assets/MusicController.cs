﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {

    [Tooltip("Main menu, credits, misc bgms")]
    public AudioClip[] bgms;
    public bool beatGame = false;

    private AudioSource audioSource;
    private int musicIndex = 2; // Music will go in order (not enough songs to shuffle, don't want to get the same one twice)

    private static bool hasInstantiated = false;
    public bool muted = false;

    // Use this for initialization
    void Start() {
        if (!hasInstantiated) {
            DontDestroyOnLoad(this.gameObject);
            audioSource = GetComponent<AudioSource>();
            hasInstantiated = true;
        } else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        audioSource.mute = muted;

        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1) {
            if (beatGame) {
                //Debug.Log("Beat game, starting cool music");
                if ((audioSource.clip != bgms[1] && audioSource.clip != bgms[2]) || !audioSource.isPlaying) {
                    audioSource.clip = bgms[2];
                    audioSource.Play();
                    audioSource.loop = true;
                }
            } else if (audioSource.clip != bgms[0] || !GetComponent<AudioSource>().isPlaying) {
                //Debug.Log("Not beaten yet");
                audioSource.clip = bgms[0];
                audioSource.Play();
                audioSource.loop = true;
            }
        } else {
            if (!GetComponent<AudioSource>().isPlaying || audioSource.clip == bgms[0]) {
                audioSource.clip = bgms[musicIndex];
                audioSource.Play();
                audioSource.loop = false;

                if (musicIndex == bgms.Length - 1) {
                    musicIndex = 3;
                } else {
                    musicIndex++;
                }
            }
        }
    }
}
