using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    // basics
    private Grid levelGrid;
    private Tilemap[] tileMaps;
    private GameObject debugLight; // Light for editing level, dimmed in game

    // player related
    public PlayerMovement playerScript;
    private GameObject player;

    // win conditions
    public List<Vector3Int> gravestoneLocations;
    // Bool to hold if player is on exit
    // Needed because:
    // From CheckWin() you don't know if the player is on an exit, so you call IsOnExit()
    // From IsOnExit() you don't know if this GameController asked for it, so you call CheckWin() just in case
    // Solution: hold data here
    public bool playerOnExit;
    public bool win = false;

    // undo feature
    public GameObject tileObjHolder;
    public Stack<LevelData> levelDataStack; // List of LevelDatas, so you can undo

    private void Awake() {
        levelDataStack = new Stack<LevelData>();
        // parent object for tile objects for easy finding
        tileObjHolder = GameObject.FindGameObjectWithTag("TileObjHolder");
    }

    void Start() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tileMaps = levelGrid.gameObject.transform.GetComponentsInChildren<Tilemap>();

        debugLight = gameObject.transform.Find("Debug Light").gameObject;
        debugLight.GetComponent<Light>().intensity = 0.5f;

        player = GameObject.FindWithTag("Player");
        FindGravestones(); // Search for gravestones

        // SAVING -- Set default completion string if it doesn't exist
        // 1 byte for every level, 0 is unsolved, 1 is solved.
        // (Assuming 15 levels for now)
        if (!PlayerPrefs.HasKey("solvedLevels") || PlayerPrefs.GetString("solvedLevels").Length != SceneManager.sceneCountInBuildSettings - 1) {
            Debug.Log("Length of string is " + PlayerPrefs.GetString("solvedLevels").Length);
            PlayerPrefs.SetString("solvedLevels", new string('0', SceneManager.sceneCountInBuildSettings - 1));
            Debug.Log("Regenerated solved levels to " + PlayerPrefs.GetString("solvedLevels"));
        }
    }

    // Update is called once per frame
    public void Update() {

        if (playerScript == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerMovement>();
        }
        //if (!win) CheckWin();
        
        if (levelDataStack.Count > 0 && Input.GetKeyDown(KeyCode.LeftArrow)) {
            LoadLevelData(levelDataStack.Pop());
            Debug.Log("Loaded state");
        }
    }

    void FindGravestones() { //Find location of gravestones
        for (int i = tileMaps[0].cellBounds.xMin; i < tileMaps[0].cellBounds.xMax; i++) {
            for (int j = tileMaps[0].cellBounds.yMin; j < tileMaps[0].cellBounds.yMax; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = tileMaps[0].GetTile(pos);
                if (tile != null) {
                    if (tile.name == "Gravestone") {
                        gravestoneLocations.Add(pos);
                    }
                }
            }
        }
    }

    public bool CheckWin() {
        if (tileMaps[0].GetTile(playerScript.tilePos).name != "Exit")
            return false;

        foreach (Vector3Int pos in gravestoneLocations) {
            // Check if there is a gravestone at that position on ground (not solid)
            if (!(tileMaps[1].HasTile(pos) && tileMaps[1].GetTile(pos).name == "Full Gravestone")) {
                // If there is a tile, return false
                //Debug.Log("Unfilled grave found!");
                return false;
            }
        }

        // Save a bool string of every level, so completion is visible in level select
        string newSave = PlayerPrefs.GetString("solvedLevels"); // Get previous save string

        // Build index of the current scene (-1 because that's how many scenes are before main levels)
        int bIndex = SceneManager.GetActiveScene().buildIndex - 1; 
        
        newSave = newSave.Substring(0, bIndex) + "1" + newSave.Substring(bIndex + 1);
        Debug.Log("Saving the string " + newSave);
        PlayerPrefs.SetString("solvedLevels", newSave);

        win = true;
        SceneManager.LoadScene(0); // Load level select scene
        return true; // Congrats, you solved this puzzle!
    }

    /// <summary> Load a LevelData to the scene.
    /// The data in <paramref name="levelData"/> will be loaded to the scene
    /// </summary>
    public void LoadLevelData(LevelData levelData) {

        //Reset tiles from given level data
        for (int i = 0; i < 2; i++) {
            tileMaps[i].SetTilesBlock(levelData.bounds, levelData.maps[i]);
            tileMaps[i].CompressBounds();
        }

        // restore from levelData
        for (int i = 0; i < tileObjHolder.transform.childCount; i++) {
            tileObjHolder.transform.GetChild(i).GetComponent<TileObject>().SetState(levelData.states[i]);

        }

    }

    public void SaveLevelData() {
        // Save data after every player move
        TileObject.TileObjectState[] tileStates = new TileObject.TileObjectState[tileObjHolder.transform.childCount];
        for (int i = 0; i < tileStates.Length; i++) {
            tileStates[i] = tileObjHolder.transform.GetChild(i).GetComponent<TileObject>().GetState();
        }
        LevelLoader levelLoader = levelGrid.GetComponent<LevelLoader>();
        LevelData snapshot = new LevelData(levelLoader.MaximumLevelBounds(), tileMaps, tileStates);
        levelDataStack.Push(snapshot); // Add the data from this move into the array
    }

    public void ResetLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToSelect() {
        SceneManager.LoadScene(0);
    }
}
