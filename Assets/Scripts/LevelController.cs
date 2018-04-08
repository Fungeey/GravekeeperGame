using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = gameObject.name;
	}
}
