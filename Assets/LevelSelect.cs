using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

    public int levelsPerWorld = 5;
    public int numWorlds = 3;

    [Tooltip("Number of scenes before the main levels")]
    public int buffer;

    private List<GameObject> levels = new List<GameObject>(); // List holding all the levels

    void Start() {
        foreach (Transform child in transform) {
            // For every world
            for (int i = 0; i < levelsPerWorld; i++) {
                // Skip over world text, get all children in Level Holder
                levels.Add(child.GetChild(1).GetChild(i).gameObject);
            }
        }

        UpdateSolvedLevels();
    }

    // Update is called once per frame
    void Update() {
        CheckLevelSelect();
    }

    void UpdateSolvedLevels() {
        // For every level in [levels], check if it is completed.
        string solvedLevels = PlayerPrefs.GetString("solvedLevels");
        Debug.Log(solvedLevels);
        for (int i = 0; i < (numWorlds * levelsPerWorld) - 1; i++) {
            if (solvedLevels.Substring(i, 1) == "1") {
                Debug.Log("Level " + i + " has already been completed");
                levels[i].GetComponent<Image>().color = Color.green;
            }
        }
    }

    void CheckLevelSelect() {
        if (EventSystem.current.currentSelectedGameObject == null)
            return;

        string name = EventSystem.current.currentSelectedGameObject.name;

        string ind = name.Substring(0, 1) + name.Substring(2, 1);
        int index = Convert.ToInt32(ind.Substring(0, 1)) * levelsPerWorld + Convert.ToInt32(ind.Substring(1, 1)) - 1 + buffer;
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
