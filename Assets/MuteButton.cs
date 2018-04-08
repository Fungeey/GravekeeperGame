using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour {

    AudioSource audioSource;
    public GameObject waves;


    // Use this for initialization


    void Awake() {
        GameObject music = GameObject.FindGameObjectWithTag("MusicPlayer");
        audioSource = music.GetComponent<AudioSource>();

        waves.GetComponent<Image>().enabled = !music.GetComponent<MusicController>().muted;

        //waves.SetActive(!music.GetComponent<MusicController>().muted);
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick() {
        MusicController music = GameObject.FindWithTag("MusicPlayer").GetComponent<MusicController>();
        music.muted = !music.muted;
        waves.GetComponent<Image>().enabled = !music.muted;
    }

    /*
    void Update() {
        music.GetComponent<MusicController>().muted = GetComponent<Toggle>().isOn;
    }
    */
}
