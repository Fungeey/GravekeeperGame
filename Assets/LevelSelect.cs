using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class LevelSelect : MonoBehaviour {

    public int levelsPerWorld = 5;
    public int buffer; 

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        checkLevelSelect();
    }

    void checkLevelSelect() {
        if (EventSystem.current.currentSelectedGameObject == null)
            return;

        string name = EventSystem.current.currentSelectedGameObject.name;

        string ind = name.Substring(0, 1) + name.Substring(2, 1);
        int index = (Convert.ToInt32(ind.Substring(1, 1)) + (Convert.ToInt32(ind.Substring(0, 1)) - 1) * levelsPerWorld) - 1 - buffer;
        // Parses level numbers (1.1, 1.5, 3.4) into indexes
        // World 1's levels go from 1-5, then world 2's levels go from 6-10, etc
        // This way we can easily load scenes using the build index
        // However this means that scenes must be put in the correct order in the build index window
        // -1 because build index is 0 indexed
        // Account for number of scenes before main levels, such as main menu and level select

        Debug.Log(index);
        SceneManager.LoadScene(index);
    }
}
