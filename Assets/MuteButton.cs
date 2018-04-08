using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour {

    // Use this for initialization
    void Start() {
        this.GetComponent<Toggle>().isOn = !GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>().mute;
    }
}
